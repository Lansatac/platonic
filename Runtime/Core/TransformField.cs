#nullable enable
using System;
using System.Collections.Generic;
using Platonic.Version;

namespace Platonic.Core
{
    public abstract class BaseTransform<TTarget> : IField<TTarget>
    {
        private TTarget _value = default!;
        private ulong _cachedVersion = Versions.None;

        protected BaseTransform(IFieldName<TTarget> targetName)
        {
            Name = targetName;
        }

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
            ulong currentVersion = CalculateVersion();
            if (_cachedVersion != currentVersion)
            {
                _cachedVersion = currentVersion;
                var newValue = CalculateValue();
                if (!EqualityComparer<TTarget>.Default.Equals(newValue, _value))
                {
                    _value = newValue;
                    _version += 1;
                }
            }
        }

        protected abstract ulong CalculateVersion();
        protected abstract TTarget CalculateValue();

        IFieldName IField.Name => Name;

        public TTarget Value
        {
            get
            {
                UpdateVersionAndValue();

                return _value;
            }
        }

        public IFieldName<TTarget> Name { get; }

        object? IField.Value => Value;

        public override string ToString()
        {
            return $"{Name}: {Value}";
        }
    }

    public class TransformField<TSource, TTarget> : BaseTransform<TTarget>
    {
        private readonly IField<TSource> _sourceField;
        private readonly Func<TSource, TTarget> _transform;

        public TransformField(IField<TSource> sourceField, IFieldName<TTarget> targetName,
            Func<TSource, TTarget> transform) : base(targetName)
        {
            _sourceField = sourceField;
            _transform = transform;
        }

        protected override ulong CalculateVersion()
        {
            return _sourceField.Version;
        }

        protected override TTarget CalculateValue()
        {
            return _transform(_sourceField.Value);
        }
    }


    public class Transform2Fields<TSource1, TSource2, TTarget> : BaseTransform<TTarget>
    {
        private readonly IField<TSource1> _sourceField1;
        private readonly IField<TSource2> _sourceField2;
        private readonly Func<TSource1, TSource2, TTarget> _transform;

        public Transform2Fields(IField<TSource1> sourceField1, IField<TSource2> sourceField2,
            IFieldName<TTarget> targetName, Func<TSource1, TSource2, TTarget> transform) : base(targetName)
        {
            _sourceField1 = sourceField1;
            _sourceField2 = sourceField2;
            _transform = transform;
        }

        protected override ulong CalculateVersion()
        {
            return _sourceField1.Version + _sourceField2.Version;
        }

        protected override TTarget CalculateValue()
        {
            return _transform(_sourceField1.Value, _sourceField2.Value);
        }
    }


    public class Transform3Fields<TSource1, TSource2, TSource3, TTarget> : BaseTransform<TTarget>
    {
        private readonly IField<TSource1> _sourceField1;
        private readonly IField<TSource2> _sourceField2;
        private readonly IField<TSource3> _sourceField3;
        private readonly Func<TSource1, TSource2, TSource3, TTarget> _transform;

        public Transform3Fields(IField<TSource1> sourceField1, IField<TSource2> sourceField2,
            IField<TSource3> sourceField3,
            IFieldName<TTarget> targetName, Func<TSource1, TSource2, TSource3, TTarget> transform) : base(targetName)
        {
            _sourceField1 = sourceField1;
            _sourceField2 = sourceField2;
            _sourceField3 = sourceField3;
            _transform = transform;
        }

        protected override ulong CalculateVersion()
        {
            return _sourceField1.Version + _sourceField2.Version + _sourceField3.Version;
        }

        protected override TTarget CalculateValue()
        {
            return _transform(_sourceField1.Value, _sourceField2.Value, _sourceField3.Value);
        }
    }


    public class Transform4Fields<TSource1, TSource2, TSource3, TSource4, TTarget> : BaseTransform<TTarget>
    {
        private readonly IField<TSource1> _sourceField1;
        private readonly IField<TSource2> _sourceField2;
        private readonly IField<TSource3> _sourceField3;
        private readonly IField<TSource4> _sourceField4;
        private readonly Func<TSource1, TSource2, TSource3, TSource4, TTarget> _transform;
    
        public Transform4Fields(
            IField<TSource1> sourceField1,
            IField<TSource2> sourceField2,
            IField<TSource3> sourceField3,
            IField<TSource4> sourceField4,
            IFieldName<TTarget> targetName,
            Func<TSource1, TSource2, TSource3, TSource4, TTarget> transform)
            : base(targetName)
        {
            _sourceField1 = sourceField1;
            _sourceField2 = sourceField2;
            _sourceField3 = sourceField3;
            _sourceField4 = sourceField4;
            _transform = transform;
        }
    
        protected override ulong CalculateVersion()
        {
            return _sourceField1.Version +
                   _sourceField2.Version +
                   _sourceField3.Version +
                   _sourceField4.Version;
        }
    
        protected override TTarget CalculateValue()
        {
            return _transform(
                _sourceField1.Value,
                _sourceField2.Value,
                _sourceField3.Value,
                _sourceField4.Value);
        }
    }


    public class Transform5Fields<TSource1, TSource2, TSource3, TSource4, TSource5, TTarget> : BaseTransform<TTarget>
    {
        private readonly IField<TSource1> _sourceField1;
        private readonly IField<TSource2> _sourceField2;
        private readonly IField<TSource3> _sourceField3;
        private readonly IField<TSource4> _sourceField4;
        private readonly IField<TSource5> _sourceField5;
        private readonly Func<TSource1, TSource2, TSource3, TSource4, TSource5, TTarget> _transform;
    
        public Transform5Fields(
            IField<TSource1> sourceField1,
            IField<TSource2> sourceField2,
            IField<TSource3> sourceField3,
            IField<TSource4> sourceField4,
            IField<TSource5> sourceField5,
            IFieldName<TTarget> targetName,
            Func<TSource1, TSource2, TSource3, TSource4, TSource5, TTarget> transform)
            : base(targetName)
        {
            _sourceField1 = sourceField1;
            _sourceField2 = sourceField2;
            _sourceField3 = sourceField3;
            _sourceField4 = sourceField4;
            _sourceField5 = sourceField5;
            _transform = transform;
        }
    
        protected override ulong CalculateVersion()
        {
            return _sourceField1.Version +
                   _sourceField2.Version +
                   _sourceField3.Version +
                   _sourceField4.Version +
                   _sourceField5.Version;
        }
    
        protected override TTarget CalculateValue()
        {
            return _transform(
                _sourceField1.Value,
                _sourceField2.Value,
                _sourceField3.Value,
                _sourceField4.Value,
                _sourceField5.Value);
        }
    }
    
    public class Transform6Fields<TSource1, TSource2, TSource3, TSource4, TSource5, TSource6, TTarget> : BaseTransform<TTarget>
    {
        private readonly IField<TSource1> _sourceField1;
        private readonly IField<TSource2> _sourceField2;
        private readonly IField<TSource3> _sourceField3;
        private readonly IField<TSource4> _sourceField4;
        private readonly IField<TSource5> _sourceField5;
        private readonly IField<TSource6> _sourceField6;
        private readonly Func<TSource1, TSource2, TSource3, TSource4, TSource5, TSource6, TTarget> _transform;
    
        public Transform6Fields(
            IField<TSource1> sourceField1,
            IField<TSource2> sourceField2,
            IField<TSource3> sourceField3,
            IField<TSource4> sourceField4,
            IField<TSource5> sourceField5,
            IField<TSource6> sourceField6,
            IFieldName<TTarget> targetName,
            Func<TSource1, TSource2, TSource3, TSource4, TSource5, TSource6, TTarget> transform)
            : base(targetName)
        {
            _sourceField1 = sourceField1;
            _sourceField2 = sourceField2;
            _sourceField3 = sourceField3;
            _sourceField4 = sourceField4;
            _sourceField5 = sourceField5;
            _sourceField6 = sourceField6;
            _transform = transform;
        }
    
        protected override ulong CalculateVersion()
        {
            return _sourceField1.Version +
                   _sourceField2.Version +
                   _sourceField3.Version +
                   _sourceField4.Version +
                   _sourceField5.Version +
                   _sourceField6.Version;
        }
    
        protected override TTarget CalculateValue()
        {
            return _transform(
                _sourceField1.Value,
                _sourceField2.Value,
                _sourceField3.Value,
                _sourceField4.Value,
                _sourceField5.Value,
                _sourceField6.Value);
        }
    }
}