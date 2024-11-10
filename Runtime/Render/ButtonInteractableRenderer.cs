#nullable enable
using Platonic.Core;
using UnityEngine;
using UnityEngine.UI;

namespace Platonic.Render
{
    [RequireComponent(typeof(Button))]
    public class ButtonInteractableRenderer : FieldRenderer<bool>
    {
        private Button? _button;

        public bool Invert;

        protected override void ProviderAwake()
        {
            _button = GetComponent<Button>();
        }

        protected override void FieldChanged(bool newValue)
        {
            if (_button != null) _button.interactable = Invert ? !newValue : newValue;
        }
    }
}