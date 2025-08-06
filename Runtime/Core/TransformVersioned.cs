#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;
using Platonic.Version;

namespace Platonic.Core
{
    public abstract class BaseTransformVersioned<TTarget> : IVersionedValue<TTarget>
    {
        private TTarget _value = default!;
        private ulong _cachedVersion = Versions.None;

        private ulong _version = Versions.Initial;

        public ulong Version
        {
            get
            {
                UpdateVersionAndValue();
                return _version;
            }
        }

        private void UpdateVersionAndValue()
        {
            var currentVersion = CalculateVersion();
            if (_cachedVersion == currentVersion) return;
            
            _cachedVersion = currentVersion;
            var newValue = CalculateValue();
            if (EqualityComparer<TTarget>.Default.Equals(newValue, _value)) return;
            
            _value = newValue;
            _version += 1;
        }

        protected abstract ulong CalculateVersion();
        protected abstract TTarget CalculateValue();

        public TTarget Value
        {
            get
            {
                UpdateVersionAndValue();
                return _value;
            }
        }
    }

    public class TransformVersioned<TSource, TTarget> : BaseTransformVersioned<TTarget>
    {
        private readonly IVersionedValue<TSource> _source;
        private readonly Func<TSource, TTarget> _transform;

        public TransformVersioned(IVersionedValue<TSource> source, Func<TSource, TTarget> transform)
        {
            _source = source;
            _transform = transform;
        }

        protected override ulong CalculateVersion()
        {
            return _source.Version;
        }

        protected override TTarget CalculateValue()
        {
            return _transform(_source.Value);
        }
    }
    
    public class Transform2Versioned<TSource1, TSource2, TTarget> : BaseTransformVersioned<TTarget>
    {
        private readonly IVersionedValue<TSource1> _source1;
        private readonly IVersionedValue<TSource2> _source2;
        private readonly Func<TSource1, TSource2, TTarget> _transform;

        public Transform2Versioned(IVersionedValue<TSource1> source1, IVersionedValue<TSource2> source2,
            Func<TSource1, TSource2, TTarget> transform)
        {
            _source1 = source1;
            _source2 = source2;
            _transform = transform;
        }

        protected override ulong CalculateVersion()
        {
            return _source1.Version + _source2.Version;
        }

        protected override TTarget CalculateValue()
        {
            return _transform(_source1.Value, _source2.Value);
        }
    }
    
    public class Transform3Versioned<TSource1, TSource2, TSource3, TTarget> : BaseTransformVersioned<TTarget>
    {
        private readonly IVersionedValue<TSource1> _source1;
        private readonly IVersionedValue<TSource2> _source2;
        private readonly IVersionedValue<TSource3> _source3;
        private readonly Func<TSource1, TSource2, TSource3, TTarget> _transform;

        public Transform3Versioned(IVersionedValue<TSource1> source1, IVersionedValue<TSource2> source2,
            IVersionedValue<TSource3> source3, Func<TSource1, TSource2, TSource3, TTarget> transform)
        {
            _source1 = source1;
            _source2 = source2;
            _source3 = source3;
            _transform = transform;
        }

        protected override ulong CalculateVersion()
        {
            return _source1.Version + _source2.Version + _source3.Version;
        }

        protected override TTarget CalculateValue()
        {
            return _transform(_source1.Value, _source2.Value, _source3.Value);
        }
    }
    
    public class Transform4Versioned<TSource1, TSource2, TSource3, TSource4, TTarget> : BaseTransformVersioned<TTarget>
    {
        private readonly IVersionedValue<TSource1> _source1;
        private readonly IVersionedValue<TSource2> _source2;
        private readonly IVersionedValue<TSource3> _source3;
        private readonly IVersionedValue<TSource4> _source4;
        private readonly Func<TSource1, TSource2, TSource3, TSource4, TTarget> _transform;

        public Transform4Versioned(
            IVersionedValue<TSource1> source1,
            IVersionedValue<TSource2> source2,
            IVersionedValue<TSource3> source3,
            IVersionedValue<TSource4> source4,
            Func<TSource1, TSource2, TSource3, TSource4, TTarget> transform)
        {
            _source1 = source1;
            _source2 = source2;
            _source3 = source3;
            _source4 = source4;
            _transform = transform;
        }

        protected override ulong CalculateVersion()
        {
            return _source1.Version +
                   _source2.Version +
                   _source3.Version +
                   _source4.Version;
        }

        protected override TTarget CalculateValue()
        {
            return _transform(
                _source1.Value,
                _source2.Value,
                _source3.Value,
                _source4.Value);
        }
    }
    
    public class Transform5Versioned<TSource1, TSource2, TSource3, TSource4, TSource5, TTarget> : BaseTransformVersioned<TTarget>
    {
        private readonly IVersionedValue<TSource1> _source1;
        private readonly IVersionedValue<TSource2> _source2;
        private readonly IVersionedValue<TSource3> _source3;
        private readonly IVersionedValue<TSource4> _source4;
        private readonly IVersionedValue<TSource5> _source5;
        private readonly Func<TSource1, TSource2, TSource3, TSource4, TSource5, TTarget> _transform;
    
        public Transform5Versioned(
            IVersionedValue<TSource1> source1,
            IVersionedValue<TSource2> source2,
            IVersionedValue<TSource3> source3,
            IVersionedValue<TSource4> source4,
            IVersionedValue<TSource5> source5,
            Func<TSource1, TSource2, TSource3, TSource4, TSource5, TTarget> transform)
        {
            _source1 = source1;
            _source2 = source2;
            _source3 = source3;
            _source4 = source4;
            _source5 = source5;
            _transform = transform;
        }
    
        protected override ulong CalculateVersion()
        {
            return _source1.Version +
                   _source2.Version +
                   _source3.Version +
                   _source4.Version +
                   _source5.Version;
        }
    
        protected override TTarget CalculateValue()
        {
            return _transform(
                _source1.Value,
                _source2.Value,
                _source3.Value,
                _source4.Value,
                _source5.Value);
        }
    }
    
    public class Transform6Versioned<TSource1, TSource2, TSource3, TSource4, TSource5, TSource6, TTarget> : BaseTransformVersioned<TTarget>
    {
        private readonly IVersionedValue<TSource1> _source1;
        private readonly IVersionedValue<TSource2> _source2;
        private readonly IVersionedValue<TSource3> _source3;
        private readonly IVersionedValue<TSource4> _source4;
        private readonly IVersionedValue<TSource5> _source5;
        private readonly IVersionedValue<TSource6> _source6;
        private readonly Func<TSource1, TSource2, TSource3, TSource4, TSource5, TSource6, TTarget> _transform;
    
        public Transform6Versioned(
            IVersionedValue<TSource1> source1,
            IVersionedValue<TSource2> source2,
            IVersionedValue<TSource3> source3,
            IVersionedValue<TSource4> source4,
            IVersionedValue<TSource5> source5,
            IVersionedValue<TSource6> source6,
            Func<TSource1, TSource2, TSource3, TSource4, TSource5, TSource6, TTarget> transform)
        {
            _source1 = source1;
            _source2 = source2;
            _source3 = source3;
            _source4 = source4;
            _source5 = source5;
            _source6 = source6;
            _transform = transform;
        }
    
        protected override ulong CalculateVersion()
        {
            return _source1.Version +
                   _source2.Version +
                   _source3.Version +
                   _source4.Version +
                   _source5.Version +
                   _source6.Version;
        }
    
        protected override TTarget CalculateValue()
        {
            return _transform(
                _source1.Value,
                _source2.Value,
                _source3.Value,
                _source4.Value,
                _source5.Value,
                _source6.Value);
        }
    }
    
    public class TransformNVersioned<TSource, TTarget> : BaseTransformVersioned<TTarget>
    {
        private readonly IVersionedValue<IEnumerable<IVersionedValue<TSource>>> _sources;
        private IVersionedValue<TSource>[]? _cachedSources;
        private ulong _cachedSourcesVersion = Versions.None;
        private readonly Func<IEnumerable<TSource>, TTarget> _transform;

        public TransformNVersioned(
            IVersionedValue<IEnumerable<IVersionedValue<TSource>>> sources,
            Func<IEnumerable<TSource>, TTarget> transform)
        {
            _sources = sources;
            _transform = transform;
        }

        protected override ulong CalculateVersion()
        {
            if (_cachedSourcesVersion != _sources.Version)
            {
                _cachedSources = _sources.Value.ToArray();
                _cachedSourcesVersion = _sources.Version;
            }
            ulong result = 0;
            foreach (var field in _cachedSources!) result += field.Version;
            return result;
        }

        protected override TTarget CalculateValue()
        {
            CalculateVersion();
            return _transform(_cachedSources!.Select(f => f.Value));
        }
    }
}