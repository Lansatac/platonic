using Platonic.Version;

namespace Platonic.Core
{
    public class Field<T> : IField<T>
    {
        public Field(FieldName<T> name, T value)
        {
            _value = value;
            Name = name;
        }

        IFieldName IField.Name => Name;
        object IField.Value => Value!;

        private T _value;
        public T Value
        {
            get => _value;
            set
            {
                if ((_value == null && value != null) || (_value != null && !_value.Equals(value)))
                {
                    _value = value;
                    Versions.Increment(ref _version);
                }
            }
        }
        public FieldName<T> Name { get; }

        private ulong _version = Versions.Initial;
        public ulong Version => _version;
    }
}