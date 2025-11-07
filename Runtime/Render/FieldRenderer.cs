#nullable enable
using System;
using System.Linq;
using Platonic.Core;
using Platonic.Scriptable;
using Platonic.Version;
using UnityEngine;
using UnityEngine.Profiling;

namespace Platonic.Render
{
    public abstract class FieldRenderer<T> : ProviderRenderer
    {
        private readonly string _fieldSample;

        private ulong _cachedFieldVersion = Versions.None;
        protected IField<T>? Field { get; private set; }
        [SerializeField] private SerializableFieldName<T> FieldName = new();

        protected FieldRenderer()
        {
            _fieldSample = $"{TypeName} UpdateField";
        }

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
                else if (Provider?.IsUsingPreviewData == false)
                {
                    Debug.LogWarning(
                        $"Data did not contain field {fieldName.Name}!" +
                        " Game Object: " +
                        $"{string.Join("/", transform.GetComponentsInParent<Transform>(true).Select(t => t.name).Reverse())}",
                        this);
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
                Profiler.BeginSample(_fieldSample, this);
                FieldChanged(Field.Value);
                Profiler.EndSample();
            }
        }

        protected virtual void FieldLateUpdate()
        {
        }

        protected abstract void FieldChanged(T newValue);

        //TODO: come back to this, maybe?
        // protected static T2 AddRenderer<T2>(GameObject gameObject, FieldName<T> fieldName) where T2 : FieldRenderer<T> 
        // {
        //     var component = gameObject.AddComponent<T2>();
        //     component.FieldName.ID = fieldName.ID;
        //     return component;
        // }
    }
}