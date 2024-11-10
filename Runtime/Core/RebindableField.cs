#nullable enable
using Platonic.Version;

namespace Platonic.Core
{
    public class RebindableField<T> : IField<T>
    {
        private IField<T>? _sourceField;
        private ulong _cachedFieldVersion = Versions.None;
        private ulong _version = Versions.Initial;

        public RebindableField(IFieldName<T> name)
        {
            Name = name;
        }
        public RebindableField(IFieldName<T> name, IField<T> sourceField) : this(name)
        {
            _sourceField = sourceField;
        }

        public void Rebind(IField<T> field)
        {
            _sourceField = field;
            _cachedFieldVersion = Versions.None;
        }

        public ulong Version
        {
            get
            {
                Recalculate();
                return _version;
            }
        }

        private void Recalculate()
        {
            if (_sourceField == null) return;
            if (_cachedFieldVersion != _sourceField.Version)
            {
                _cachedFieldVersion = _sourceField.Version;
                _version += 1;
            }
        }

        IFieldName IField.Name => Name;

        public T Value => _sourceField == null ? default! : _sourceField.Value;
        public IFieldName<T> Name { get; }

        object? IField.Value => Value;
    }
}