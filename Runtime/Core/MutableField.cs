#nullable enable
using System;
using Platonic.Version;
using UnityEngine;

namespace Platonic.Core
{
    [Serializable]
    public class MutableField<T> : IMutableField<T>, ISerializationCallbackReceiver
    {
        public MutableField(FieldName<T> name, T value)
        {
            _value = value;
            _name = name;
        }

        IFieldName IField.Name => Name;

        object IMutableField.Value
        {
            get => Value;
            set
            {
                if (value is not T castValue)
                {
                    throw new Exception($"Cannot set value of type {value.GetType()} to type {typeof(T)}");
                }

                _value = castValue;
            }
        }

        object IField.Value => Value;

        [SerializeField] private T _value;

        public T Value
        {
            get => _value;
            set
            {
                if (Equals(_value, value)) return;
                _value = value;
                Versions.Increment(ref _version);
            }
        }

        [SerializeField] private FieldName<T> _name;

        public IFieldName<T> Name => _name;

        private ulong _version = Versions.Initial;
        public ulong Version => _version;

        /// <summary>
        /// Call in cases where the value of a referenced object may change, but the reference does not.
        /// Lists are the canonical example; their contents change, but that is otherwise invisible to the Field.
        /// </summary>
        public void ForceVersionIncrement()
        {
            _version += 1;
        }

        public static implicit operator T(MutableField<T> mutableField) => mutableField._value;


        public void OnBeforeSerialize()
        {
        }

        public void OnAfterDeserialize()
        {
            if (_version == Versions.None)
                _version = Versions.Initial;
            _version += 1;
        }

        public override string ToString()
        {
            return $"{Name.Name}: {_value}";
        }
    }
}