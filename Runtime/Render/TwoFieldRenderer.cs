#nullable enable
using System;
using Platonic.Core;
using Platonic.Scriptable;
using Platonic.Version;
using UnityEngine;

namespace Platonic.Render
{
    public abstract class TwoFieldRenderer<T1, T2> : ProviderRenderer
    {
        private ulong _cachedFieldVersion = Versions.None;
        protected IField<T1>? Field1 { get; private set; }
        protected IField<T2>? Field2 { get; private set; }
        
        [SerializeField] private SerializableFieldName<T1> FieldName1 = new();
        [SerializeField] private SerializableFieldName<T2> FieldName2 = new();

        protected sealed override void OnDataChanged()
        {
            _cachedFieldVersion = Versions.None;
            IFieldName<T1> fieldName1;
            IFieldName<T2> fieldName2;

            try
            {
                fieldName1 = FieldName1.AsName();
                fieldName2 = FieldName2.AsName();
            }
            catch (Exception e)
            {
                Debug.LogException(e, this);
                return;
            }

            if (Data != null)
            {
                Field1 = Data.GetField(fieldName1);
                Field2 = Data.GetField(fieldName2);
            }
        }

        protected sealed override void ProviderLateUpdate()
        {
            if (Field1 == null || Field2 == null) return;

            if (_cachedFieldVersion != Field1.Version + Field2.Version)
            {
                _cachedFieldVersion = Field1.Version + Field2.Version;
                FieldsChanged(Field1.Value, Field2.Value);
            }
            
            FieldLateUpdate();
        }

        protected virtual void FieldLateUpdate() { }

        protected abstract void FieldsChanged(T1 newValue1, T2 newValue2);
    }
}