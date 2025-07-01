#nullable enable
using System;
using System.Collections.Generic;
using Platonic.Version;

namespace Platonic.Core
{
    public class FieldLookup<TSource, TTarget> : IField<TTarget>
    {
        private readonly IField<TSource> _sourceField;
        private readonly Func<TSource, IField<TTarget>?> _lookup;
        private readonly TTarget _defaultValue;

        private ulong _cachedSourceVersion = Versions.None;
        private ulong _cachedTargetVersion = Versions.None;

        private IField<TTarget>? _targetField;

        public FieldLookup(IField<TSource> sourceField, IFieldName<TTarget> targetName,
            Func<TSource, IField<TTarget>?> lookup, TTarget defaultValue)
        {
            _sourceField = sourceField;
            Name = targetName;
            _lookup = lookup;
            _defaultValue = defaultValue;

            _targetField = _lookup(sourceField.Value);
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

        IFieldName IField.Name => Name;

        public TTarget Value
        {
            get
            {
                Recalculate();

                var newValue = _targetField != null ? _targetField.Value ?? _defaultValue : _defaultValue;
                return newValue;
            }
        }

        private void Recalculate()
        {
            if (_cachedSourceVersion != _sourceField.Version)
            {
                _cachedSourceVersion = _sourceField.Version;
                _targetField = _lookup(_sourceField.Value);
                _cachedTargetVersion = Versions.None;
                ++_version;
            }

            if (_targetField == null) return;
            if (_cachedTargetVersion == _targetField?.Version) return;
            _cachedTargetVersion = _targetField?.Version ?? Versions.None;
            ++_version;
        }

        public IFieldName<TTarget> Name { get; }

        object? IField.Value => Value;
    }
    
    public class FieldLookup2Fields<TSource1, TSource2, TTarget> : IField<TTarget>
    {
        private readonly IField<TSource1> _sourceField1;
        private readonly IField<TSource2> _sourceField2;
        private readonly Func<TSource1, TSource2, IField<TTarget>?> _lookup;
        private readonly TTarget _defaultValue;

        private ulong _cachedSourceVersion = Versions.None;
        private ulong _cachedTargetVersion = Versions.None;

        private IField<TTarget>? _targetField;

        public FieldLookup2Fields(IField<TSource1> sourceField1, IField<TSource2> sourceField2, IFieldName<TTarget> targetName,
            Func<TSource1, TSource2, IField<TTarget>?> lookup, TTarget defaultValue)
        {
            _sourceField1 = sourceField1;
            _sourceField2 = sourceField2;
            Name = targetName;
            _lookup = lookup;
            _defaultValue = defaultValue;

            _targetField = _lookup(sourceField1.Value, sourceField2.Value);;
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

        IFieldName IField.Name => Name;

        public TTarget Value
        {
            get
            {
                Recalculate();

                var newValue = _targetField != null ? _targetField.Value ?? _defaultValue : _defaultValue;
                return newValue;
            }
        }

        private void Recalculate()
        {
            if (_cachedSourceVersion != _sourceField1.Version + _sourceField2.Version)
            {
                _cachedSourceVersion = _sourceField1.Version + _sourceField2.Version;
                _targetField = _lookup(_sourceField1.Value, _sourceField2.Value);
                _cachedTargetVersion = Versions.None;
                ++_version;
            }

            if (_targetField == null) return;
            if (_cachedTargetVersion == _targetField?.Version) return;
            _cachedTargetVersion = _targetField?.Version ?? Versions.None;
            ++_version;
        }

        public IFieldName<TTarget> Name { get; }

        object? IField.Value => Value;
    }

    public class Lookup<TSource, TTarget> : IField<TTarget>
    {
        private readonly IField<TSource> _sourceField;
        private readonly Func<TSource, TTarget?> _lookup;
        private readonly TTarget _defaultValue;

        private ulong _cachedSourceVersion = Versions.None;

        private TTarget? _cachedValue;

        public Lookup(IField<TSource> sourceField, IFieldName<TTarget> targetName, Func<TSource, TTarget?> lookup,
            TTarget defaultValue)
        {
            _sourceField = sourceField;
            Name = targetName;
            _lookup = lookup;
            _defaultValue = defaultValue;

            _cachedValue = _lookup(sourceField.Value) ?? defaultValue;
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

        IFieldName IField.Name => Name;

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
            if (_cachedSourceVersion != _sourceField.Version)
            {
                _cachedSourceVersion = _sourceField.Version;
                var newValue = _lookup(_sourceField.Value);
                if (!EqualityComparer<TTarget?>.Default.Equals(_cachedValue, newValue))
                {
                    _cachedValue = newValue ?? _defaultValue;
                    ++_version;
                }
            }
        }

        public IFieldName<TTarget> Name { get; }

        object? IField.Value => Value;
    }
}