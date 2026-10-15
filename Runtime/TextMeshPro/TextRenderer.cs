#nullable enable
using System.Collections.Generic;
using System.Globalization;
using Platonic.Core;
using Platonic.Scriptable;
using Platonic.Version;
using TMPro;
using UnityEngine;

namespace Platonic.Render
{
    public class TextRenderer : ProviderRenderer
    {
        private TMP_Text? Text => _text ??= GetComponent<TMP_Text>();
        private TMP_Text? _text;

        [SerializeField] private string FormatString = "{0}";

        [SerializeField] private List<SerializableFieldName>? FieldsToWatch;

        private readonly List<IFieldName> _fieldsToWatch = new();

        ulong _cachedFieldVersions;

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
            if (Text == null) return;
            if (Data == null) return;

            var fieldVersions = CalculateFieldVersions();
            if (_cachedFieldVersions != fieldVersions)
            {
                _cachedFieldVersions = fieldVersions;
                var args = new List<object?>();
                foreach (var fieldName in _fieldsToWatch)
                {
                    if (Data.HasField(fieldName))
                    {
                        var value = Data.GetField(fieldName).Value;
                        args.Add(value is int intValue
                            ? intValue.ToString("N0", CultureInfo.CurrentCulture)
                            : value);
                    }
                    else
                    {
                        Debug.LogWarning(
                            $"Found no field '{fieldName.Name}' in data at {GetGameObjectPath(Provider?.gameObject!)}, render at {GetGameObjectPath(gameObject)}",
                            this);
                        args.Add("<missing>");
                    }
                }

                Text.text = string.Format(FormatString ?? "", args.ToArray());
            }
        }

        private static string GetGameObjectPath(GameObject gameObject)
        {
            var path = gameObject.name;
            var parent = gameObject.transform.parent;

            while (parent != null)
            {
                path = parent.name + "/" + path;
                parent = parent.parent;
            }

            return path;
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