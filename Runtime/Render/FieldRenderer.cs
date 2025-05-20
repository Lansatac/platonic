#nullable enable
using System;
using Platonic.Core;
using Platonic.Scriptable;
using Platonic.Version;
using UnityEngine;

namespace Platonic.Render
{
    public abstract class FieldRenderer<T> : ProviderRenderer
    {
        private ulong _cachedFieldVersion = Versions.None;
        protected IField<T>? Field { get; private set; }
        [SerializeField] private SerializableFieldName<T> FieldName = new();

        protected sealed override void OnDataChanged()
        {
            _cachedFieldVersion = Versions.None;
            IFieldName<T> fieldName;

            try
            {
                fieldName = FieldName.AsName();
            }
            catch (Exception e)
            {
                Debug.LogException(e, this);
                return;
            }

            if (Data != null)
            {
                if (Data.HasField(fieldName))
                {
                    Field = Data.GetField(fieldName);
                    UpdateField();
                }
                else if(Provider?.IsUsingPreviewData == false)
                {
                    Debug.LogWarning($"Data did not contain field {fieldName.Name}!", this);
                }
            }
        }

        protected sealed override void ProviderLateUpdate()
        {
            UpdateField();
            
            FieldLateUpdate();
        }

        protected void UpdateField()
        {
            if (Field == null) return;
            
            if (_cachedFieldVersion != Field.Version)
            {
                _cachedFieldVersion = Field.Version;
                FieldChanged(Field.Value);
            }
        }

        protected virtual void FieldLateUpdate() { }

        protected abstract void FieldChanged(T newValue);
    }
}