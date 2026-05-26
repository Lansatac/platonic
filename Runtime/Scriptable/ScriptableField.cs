#nullable enable
using System;
using System.Collections.Generic;
using Platonic.Core;
using Platonic.Version;
using UnityEngine;

namespace Platonic.Scriptable
{
    public abstract class ScriptableField : ScriptableObject, IField
    {
        ulong IVersioned.Version => GetVersion();
        protected abstract ulong GetVersion();

        IFieldName IField.Name => GetName();
        protected abstract IFieldName GetName();

        object? IField.Value => GetValue();
        protected abstract object? GetValue();
    }

    public abstract class ScriptableField<T> : ScriptableField, IMutableField<T>, ISerializationCallbackReceiver
    {
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
                        throw new Exception(
                            $"Cannot assign a value of type {value.GetType().Name} to field of type {typeof(T).Name}");
                    Value = castValue;
                }
            }
        }


        private T? _setValue;
        [SerializeField] private T _serializedValue = default!;

        protected virtual T GetSerializedValue()
        {
            return _serializedValue;
        }

        public T Value
        {
            get => _setValue ?? GetSerializedValue();
            set
            {
                if (!EqualityComparer<T?>.Default.Equals(_setValue, value))
                {
                    _setValue = value;
                    Version += 1;
                }
            }
        }

        [SerializeField] private SerializableFieldName<T>? _name;
        public IFieldName<T> Name => _name!.AsName();
        
        public ulong Version { get; private set; } = Versions.Initial;

        protected override ulong GetVersion()
        {
            return Version;
        }

        protected override object? GetValue()
        {
            return Value;
        }

        protected override IFieldName GetName()
        {
            return Name;
        }

        public void OnBeforeSerialize()
        {
        }

        public void OnAfterDeserialize()
        {
            Value = GetSerializedValue()!;
        }
    }
}