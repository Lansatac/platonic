#nullable enable
using UnityEngine;
using UnityEngine.UI;

namespace Platonic.Render
{
    [RequireComponent(typeof(Selectable))]
    public class InteractableRenderer : FieldRenderer<bool>
    {
        private Selectable? _selectable;

        public bool Invert;

        protected override void ProviderAwake()
        {
            _selectable = GetComponent<Selectable>();
        }

        protected override void FieldChanged(bool newValue)
        {
            if (_selectable != null) _selectable.interactable = Invert ? !newValue : newValue;
        }
    }
}