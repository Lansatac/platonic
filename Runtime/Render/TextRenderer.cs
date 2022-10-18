using System.Collections.Generic;
using System.Linq;
using Platonic.Core;
using Platonic.Scriptable;
using Platonic.Version;
using TMPro;
using UnityEngine;

namespace Platonic.Render
{
    public class TextRenderer : ProviderRenderer
    {
        private TMP_Text? Text;

        private string? FormatString;

        [SerializeField]
        private List<SerializableFieldName>? FieldsToWatch;

        private readonly List<IFieldName> _fieldsToWatch = new();

        ulong _cachedFieldVersions;

        protected override void ProviderAwake()
        {
            base.ProviderAwake();
            Text = GetComponent<TMP_Text>();
            if (Text != null)
            {
                FormatString = Text.text;
            }

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

        protected override void ProviderUpdate()
        {
            if (Text == null) return;
            if(Data == null) return;

            var fieldVersions = CalculateFieldVersions();
            if (_cachedFieldVersions != fieldVersions)
            {
                _cachedFieldVersions = fieldVersions;
                Text.text = string.Format(FormatString ?? "",
                    _fieldsToWatch.Select(f => Data.GetField(f).Value).ToArray());
            }
        }

        private ulong CalculateFieldVersions()
        {
            var version = Versions.None;
            if (Data == null) return version;
            
            foreach (var field in _fieldsToWatch)
            {
                version += Data.GetField(field).Version;
            }

            return version;
        }
    }
}