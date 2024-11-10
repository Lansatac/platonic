#nullable enable
using System;
using System.Collections.Generic;
using Platonic.Version;

namespace Platonic.Core
{
    public class TransformField<TSource, TTarget> : IField<TTarget>
    {
        private readonly IField<TSource> _sourceField;
        private readonly Func<TSource, TTarget> _transform;
        private TTarget _value;
        private ulong _cachedVersion = Versions.None;

        public TransformField(IField<TSource> sourceField, IFieldName<TTarget> targetName,
            Func<TSource, TTarget> transform)
        {
            _sourceField = sourceField;
            _transform = transform;

            _value = _transform(_sourceField.Value);
            _version = Versions.Initial;

            Name = targetName;
        }

        private ulong _version;

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
            if (_cachedVersion != _sourceField.Version)
            {
                _cachedVersion = _sourceField.Version;
                var newValue = _transform(_sourceField.Value);
                if (!EqualityComparer<TTarget>.Default.Equals(newValue, _value))
                {
                    _value = newValue;
                    _version += 1;
                }
            }
        }

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
    }


    public class TransformField<TSource1, TSource2, TTarget> : IField<TTarget>
    {
        private readonly IField<TSource1> _sourceField1;
        private readonly IField<TSource2> _sourceField2;
        private readonly Func<TSource1, TSource2, TTarget> _transform;
        private TTarget _value;
        private ulong _cachedVersion = Versions.None;

        public TransformField(IField<TSource1> sourceField1, IField<TSource2> sourceField2,
            IFieldName<TTarget> targetName, Func<TSource1, TSource2, TTarget> transform)
        {
            _sourceField1 = sourceField1;
            _sourceField2 = sourceField2;
            _transform = transform;

            _value = _transform(_sourceField1.Value, _sourceField2.Value);
            _version = Versions.Initial;

            Name = targetName;
        }

        private ulong _version;

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
            if (_cachedVersion != _sourceField1.Version + _sourceField2.Version)
            {
                _cachedVersion = _sourceField1.Version + _sourceField2.Version;
                var newValue = _transform(_sourceField1.Value, _sourceField2.Value);
                if (!EqualityComparer<TTarget>.Default.Equals(newValue, _value))
                {
                    _value = newValue;
                    _version += 1;
                }
            }
        }

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
    }
}