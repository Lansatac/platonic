#nullable enable
using System.Collections.Generic;
using Platonic.Core;
using Platonic.Scriptable;
using Platonic.Version;
using TMPro;
using UnityEngine;

namespace Platonic.Render
{
    [RequireComponent(typeof(TMP_InputField))]
    public class InputFieldRenderer : ProviderRenderer
    {
        private TMP_InputField? InputField => _inputField ??= GetComponent<TMP_InputField>();
        private TMP_InputField? _inputField;

        [SerializeField] private string FormatString = "{0}";
        [SerializeField] private List<SerializableFieldName>? FieldsToWatch;

        private readonly List<IFieldName> _fieldsToWatch = new();
        private ulong _cachedFieldVersions;

        protected override void ProviderAwake()
        {
            base.ProviderAwake();

            if (FieldsToWatch == null) return;

            foreach (var fieldName in FieldsToWatch)
            {
                _fieldsToWatch.Add(fieldName.AsName());
            }
        }

        protected override void OnDataChanged()
        {
            _cachedFieldVersions = Versions.None;
        }

        protected override void ProviderLateUpdate()
        {
            if (InputField == null) return;
            if (Data == null) return;

            var fieldVersions = CalculateFieldVersions();
            if (_cachedFieldVersions == fieldVersions) return;

            _cachedFieldVersions = fieldVersions;
            var args = new List<object?>();
            foreach (var fieldName in _fieldsToWatch)
            {
                if (Data.HasField(fieldName))
                {
                    args.Add(Data.GetField(fieldName).Value);
                }
                else
                {
                    Debug.LogWarning(
                        $"Found no field '{fieldName.Name}' in data, render at {gameObject.name}",
                        this);
                    args.Add("<missing>");
                }
            }

            InputField.text = string.Format(FormatString ?? "", args.ToArray());
        }

        private ulong CalculateFieldVersions()
        {
            var version = Versions.None;
            if (Data == null) return version;

            foreach (var field in _fieldsToWatch)
            {
                if (Data.HasField(field))
                {
                    version += Data.GetField(field).Version;
                }
            }

            return version;
        }
    }
}
