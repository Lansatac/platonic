#nullable enable
using System;
using Platonic.Version;

namespace Platonic.Core
{
    public class FieldLookup<TSource, TTarget> : IField<TTarget>
    {
        private readonly IField<TSource> _sourceField;
        private readonly IFieldName<TTarget> _targetName;
        private readonly Func<TSource, IField<TTarget>> _lookup;

        private ulong _cachedSourceVersion = Versions.None;
        private ulong _cachedTargetVersion = Versions.None;

        private IField<TTarget> _targetField;

        public FieldLookup(IField<TSource> sourceField, IFieldName<TTarget> targetName, Func<TSource, IField<TTarget>> lookup)
        {
            _sourceField = sourceField;
            _targetName = targetName;
            _lookup = lookup;

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

        public TTarget Value {
            get
            {
                Recalculate();

                return _targetField.Value;
            }
            
        }

        private void Recalculate()
        {
            if (_cachedSourceVersion != _sourceField.Version)
            {
                _cachedSourceVersion = _sourceField.Version;
                _targetField = _lookup(_sourceField.Value);
                _cachedTargetVersion = Versions.None;
            }

            if (_cachedTargetVersion != _targetField.Version)
            {
                _cachedTargetVersion = _targetField.Version;
                ++_version;
            }
        }

        public IFieldName<TTarget> Name => _targetName;

        object? IField.Value => Value;
    }
}