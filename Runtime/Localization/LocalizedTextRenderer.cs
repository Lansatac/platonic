#nullable enable
using System;
using System.Collections.Generic;
using Platonic.Core;
using Platonic.Render;
using Platonic.Scriptable;
using Platonic.Version;
using TMPro;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.SmartFormat.PersistentVariables;

namespace Platonic.Localization
{
    public class LocalizedTextRenderer : ProviderRenderer
    {
        private TMP_Text? Text => _text ??= GetComponent<TMP_Text>();
        private TMP_Text? _text;

        [SerializeField] private LocalizedString? _localizedString;

        [SerializeField] private List<FieldVariableBinding> _bindings = new();

        [Serializable]
        private class FieldVariableBinding
        {
            public SerializableFieldName FieldName = new();
            public string VariableName = "";
        }

        private struct ResolvedBinding
        {
            public IFieldName FieldName;
            public IVariable Variable;
        }

        private readonly List<ResolvedBinding> _resolved = new();
        private ulong _cachedFieldVersions;

        protected override void ProviderAwake()
        {
            base.ProviderAwake();

            if (Text != null)
                Text.text = string.Empty;
        }

        protected override void ProviderOnEnable()
        {
            if (_localizedString != null)
            {
                _localizedString.StringChanged += OnStringChanged;
                _localizedString.RefreshString();
            }
        }

        private void OnDisable()
        {
            if (_localizedString != null)
                _localizedString.StringChanged -= OnStringChanged;
        }

        protected override void OnDataChanged()
        {
            _cachedFieldVersions = Versions.None;
            _resolved.Clear();

            if (_localizedString == null || Data == null) return;

            foreach (var binding in _bindings)
            {
                IFieldName fieldName;
                try
                {
                    fieldName = binding.FieldName.AsName();
                }
                catch (Exception e)
                {
                    Debug.LogException(e, this);
                    continue;
                }

                if (!Data.HasField(fieldName))
                {
                    Debug.LogWarning($"Data did not contain field for variable '{binding.VariableName}'.", this);
                    continue;
                }

                var field = Data.GetField(fieldName);
                var variable = CreateVariable(field.Value);
                _localizedString[binding.VariableName] = variable;
                _resolved.Add(new ResolvedBinding { FieldName = fieldName, Variable = variable });
            }
        }

        protected override void ProviderLateUpdate()
        {
            if (_localizedString == null || Data == null) return;

            var versions = CalculateFieldVersions();
            if (_cachedFieldVersions == versions) return;

            _cachedFieldVersions = versions;

            foreach (var binding in _resolved)
            {
                var field = Data.GetField(binding.FieldName);
                UpdateVariable(binding.Variable, field.Value);
            }

            _localizedString.RefreshString();
        }

        private void OnStringChanged(string value)
        {
            if (Text != null)
                Text.text = value;
        }

        private ulong CalculateFieldVersions()
        {
            var version = Versions.None;
            foreach (var binding in _resolved)
            {
                if (Data!.HasField(binding.FieldName))
                    version += Data.GetField(binding.FieldName).Version;
            }
            return version;
        }

        private static IVariable CreateVariable(object? value) => value switch
        {
            int v    => new IntVariable { Value = v },
            float v  => new FloatVariable { Value = v },
            bool v   => new BoolVariable { Value = v },
            _        => new StringVariable { Value = value?.ToString() ?? "" },
        };

        private static void UpdateVariable(IVariable variable, object? value)
        {
            switch (variable)
            {
                case IntVariable iv    when value is int i:    iv.Value = i; break;
                case FloatVariable fv  when value is float f:  fv.Value = f; break;
                case BoolVariable bv   when value is bool b:   bv.Value = b; break;
                case StringVariable sv:                        sv.Value = value?.ToString() ?? ""; break;
            }
        }
    }
}
