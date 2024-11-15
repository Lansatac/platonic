﻿#nullable enable
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

    public abstract class ScriptableField<T> : ScriptableField, IField<T>, ISerializationCallbackReceiver
    {
        IFieldName IField.Name => Name;
        object IField.Value => Value!;

        private T? _cachedValue;
        [SerializeField] private T? _value;

        protected virtual T? GetSerializedValue()
        {
            return _value;
        }

        public T Value
        {
            get => _cachedValue ?? GetSerializedValue()!;
            set
            {
                if (!EqualityComparer<T?>.Default.Equals(_cachedValue, value))
                {
                    _cachedValue = value;
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