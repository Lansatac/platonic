#nullable enable
using System;
using System.Collections.Generic;
using Platonic.Version;

namespace Platonic.Core
{
    public class VersionedLookup<TSource, TTarget> : IVersionedValue<TTarget>
    {
        private readonly IVersionedValue<TSource> _source;
        private readonly Func<TSource, IVersionedValue<TTarget>?> _lookup;
        private readonly TTarget _defaultValue;

        private ulong _cachedSourceVersion = Versions.None;
        private ulong _cachedTargetVersion = Versions.None;

        private IVersionedValue<TTarget>? _target;

        public VersionedLookup(IVersionedValue<TSource> source,
            Func<TSource, IVersionedValue<TTarget>?> lookup,
            TTarget defaultValue)
        {
            _source = source;
            _lookup = lookup;
            _defaultValue = defaultValue;

            _target = _lookup(source.Value);
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

                var newValue = _target != null ? _target.Value ?? _defaultValue : _defaultValue;
                return newValue;
            }
        }

        private void Recalculate()
        {
            if (_cachedSourceVersion != _source.Version)
            {
                _cachedSourceVersion = _source.Version;
                _target = _lookup(_source.Value);
                _cachedTargetVersion = Versions.None;
                ++_version;
            }

            if (_target == null) return;
            if (_cachedTargetVersion == _target?.Version) return;
            _cachedTargetVersion = _target?.Version ?? Versions.None;
            ++_version;
        }
    }
    
    public class VersionedLookup2<TSource1, TSource2, TTarget> : IVersionedValue<TTarget>
    {
        private readonly IVersionedValue<TSource1> _source1;
        private readonly IVersionedValue<TSource2> _source2;
        private readonly Func<TSource1, TSource2, IVersionedValue<TTarget>?> _lookup;
        private readonly TTarget _defaultValue;

        private ulong _cachedSourceVersion = Versions.None;
        private ulong _cachedTargetVersion = Versions.None;

        private IVersionedValue<TTarget>? _target;

        public VersionedLookup2(IVersionedValue<TSource1> source1, IVersionedValue<TSource2> source2,
            Func<TSource1, TSource2, IVersionedValue<TTarget>?> lookup, TTarget defaultValue)
        {
            _source1 = source1;
            _source2 = source2;
            _lookup = lookup;
            _defaultValue = defaultValue;

            _target = _lookup(source1.Value, source2.Value);;
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

                var newValue = _target != null ? _target.Value ?? _defaultValue : _defaultValue;
                return newValue;
            }
        }

        private void Recalculate()
        {
            if (_cachedSourceVersion != _source1.Version + _source2.Version)
            {
                _cachedSourceVersion = _source1.Version + _source2.Version;
                _target = _lookup(_source1.Value, _source2.Value);
                _cachedTargetVersion = Versions.None;
                ++_version;
            }

            if (_target == null) return;
            if (_cachedTargetVersion == _target?.Version) return;
            _cachedTargetVersion = _target?.Version ?? Versions.None;
            ++_version;
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