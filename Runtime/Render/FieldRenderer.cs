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
            Field = Data?.GetField(FieldName.AsName());
        }

        protected sealed override void ProviderUpdate()
        {
            if (Field == null) return;

            if (_cachedFieldVersion != Field.Version)
            {
                _cachedFieldVersion = Field.Version;
                FieldChanged(Field);
            }
            
            FieldUpdate();
        }

        protected virtual void FieldUpdate() { }

        protected abstract void FieldChanged(IField<T> field);
    }
}