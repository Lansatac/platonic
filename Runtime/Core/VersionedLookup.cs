#nullable enable
using System;
using System.Collections.Generic;
using Platonic.Version;

namespace Platonic.Core
{
    public class VersionedLookup<TSource, TVersionedTarget, TFinalTarget> : IVersionedValue<TFinalTarget>
    {
        private readonly IVersionedValue<TSource> _source;
        private readonly Func<TSource, IVersionedValue<TVersionedTarget>?> _lookup;
        private readonly Func<TVersionedTarget?, TFinalTarget> _transform;

        private ulong _cachedSourceVersion = Versions.None;
        private ulong _cachedTargetVersion = Versions.None;

        private IVersionedValue<TVersionedTarget>? _target;
        private TFinalTarget? _cachedValue;

        public VersionedLookup(IVersionedValue<TSource> source,
            Func<TSource, IVersionedValue<TVersionedTarget>?> lookup,
            Func<TVersionedTarget?, TFinalTarget> transform)
        {
            _source = source;
            _lookup = lookup;
            _transform = transform;
        }

        private ulong _version = Versions.Initial;

        public ulong Version
        {
            get
            {
                Recalculate();
                return _version;
            }
        }

        public TFinalTarget Value => Recalculate();

        private TFinalTarget Recalculate()
        {
            if (_cachedSourceVersion != _source.Version)
            {
                _cachedSourceVersion = _source.Version;
                _target = _lookup(_source.Value);
                _cachedTargetVersion = Versions.None;
                ++_version;
            }

            if (_cachedTargetVersion == _target?.Version) return _cachedValue!;
            _cachedTargetVersion = _target?.Version ?? Versions.None;
            _cachedValue = _transform(_target != null ? _target.Value : default);
            ++_version;
            return _cachedValue;
        }
    }
    
    public class VersionedLookup2<TSource1, TSource2, TVersionedTarget, TFinalTarget> : IVersionedValue<TFinalTarget>
    {
        private readonly IVersionedValue<TSource1> _source1;
        private readonly IVersionedValue<TSource2> _source2;
        private readonly Func<TSource1, TSource2, IVersionedValue<TVersionedTarget>?> _lookup;
        private readonly Func<TVersionedTarget?, TFinalTarget> _transform;

        private ulong _cachedSourceVersion = Versions.None;
        private ulong _cachedTargetVersion = Versions.None;

        private IVersionedValue<TVersionedTarget>? _target;
        private TFinalTarget? _cachedValue;

        public VersionedLookup2(IVersionedValue<TSource1> source1, IVersionedValue<TSource2> source2,
            Func<TSource1, TSource2, IVersionedValue<TVersionedTarget>?> lookup, Func<TVersionedTarget?, TFinalTarget> transform)
        {
            _source1 = source1;
            _source2 = source2;
            _lookup = lookup;
            _transform = transform;
        }

        private ulong _version = Versions.Initial;

        public ulong Version
        {
            get
            {
                Recalculate();
                return _version;
            }
        }

        public TFinalTarget Value => Recalculate();

        private TFinalTarget Recalculate()
        {
            if (_cachedSourceVersion != _source1.Version + _source2.Version)
            {
                _cachedSourceVersion = _source1.Version + _source2.Version;
                _target = _lookup(_source1.Value, _source2.Value);
                _cachedTargetVersion = Versions.None;
                ++_version;
            }

            if (_cachedTargetVersion == _target?.Version) return _cachedValue!;
            _cachedTargetVersion = _target?.Version ?? Versions.None;
            _cachedValue = _transform(_target != null ? _target.Value : default);
            ++_version;
            
            return _cachedValue;
        }
    }

    public class Lookup<TSource, TTarget> : IVersionedValue<TTarget>
    {
        private readonly IVersionedValue<TSource> _source;
        private readonly Func<TSource, TTarget?> _lookup;
        private readonly TTarget _defaultValue;

        private ulong _cachedSourceVersion = Versions.None;

        private TTarget? _cachedValue;

        public Lookup(IVersionedValue<TSource> source, Func<TSource, TTarget?> lookup,
            TTarget defaultValue)
        {
            _source = source;
            _lookup = lookup;
            _defaultValue = defaultValue;

            _cachedValue = _lookup(source.Value) ?? defaultValue;
        }

        private ulong _version = Versions.Initial;

        public ulong Version
        {
            get
            {
                Recalculate();
                return _version;
            }
        }

        public TTarget Value
        {
            get
            {
                Recalculate();
                return _cachedValue ?? _defaultValue;
            }
        }

        private void Recalculate()
        {
            if (_cachedSourceVersion != _source.Version)
            {
                _cachedSourceVersion = _source.Version;
                var newValue = _lookup(_source.Value);
                if (!EqualityComparer<TTarget?>.Default.Equals(_cachedValue, newValue))
                {
                    _cachedValue = newValue ?? _defaultValue;
                    ++_version;
                }
            }
        }
    }
}