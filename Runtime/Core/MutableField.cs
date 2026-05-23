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

        object? IMutableField.Value
        {
            get => Value;
            set
            {
                if (value == null)
                {
                    if (default(T) == null)
                    {
                        Value = default!;
                    }
                    else
                    {
                        throw new Exception($"Cannot assign null to non-nullable field of type {typeof(T).Name}");
                    }
                }
                else
                {
                    if (value is not T castValue)
                    {
                        throw new Exception($"Cannot set value of type {value.GetType()} to type {typeof(T)}");
                    }

                    Value = castValue;
                }
            }
        }

        object? IField.Value => Value;

        [SerializeField] private T _value;

        public T Value
        {
            get => _value;
            set
            {
                if (Equals(_value, value)) return;
                _value = value;
                _cachedValueVersion = null;
                Versions.Increment(ref _version);
            }
        }

        [SerializeField] private FieldName<T> _name;

        public IFieldName<T> Name => _name;

        private ulong? _cachedValueVersion = null;
        private ulong _version = Versions.Initial;
        public ulong Version
        {
            get
            {
                if (_value is IVersioned versioned)
                {
                    if (_cachedValueVersion != versioned.Version)
                    {
                        _cachedValueVersion = versioned.Version;
                        Versions.Increment(ref _version);
                    }
                }
                return _version;
            }
        }

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
            _cachedValueVersion = null; // Ensure caching is reset after deserialization
            Versions.Increment(ref _version);
        }

        public override string ToString()
        {
            return $"{Name.Name}: {_value}";
        }
    }
}