using UnityEngine;
using UnityEngine.UI;

namespace Platonic.Render
{
    [RequireComponent(typeof(Button))]
    public class ButtonInteractableRenderer : FieldRenderer<bool>
    {
        [SerializeField] private bool Invert;
        private Button _button;


        protected override void ProviderAwake()
        {
            _button = GetComponent<Button>();
        }

        protected override void FieldChanged(bool newValue)
        {
            _button.interactable = Invert ? !newValue : newValue;
        }
    }
}