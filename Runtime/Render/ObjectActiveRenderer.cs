using Platonic.Render;
using UnityEngine;

namespace Platonic
{
    public class ObjectActiveRenderer : FieldRenderer<bool>
    {
        [SerializeField] private GameObject Target;
        [SerializeField] private bool Invert;
        protected override void FieldChanged(bool newValue)
        {
            if (Invert)
            {
                Target.SetActive(!newValue);
            }
            else
            {
                Target.SetActive(newValue);    
            }
        }
    }
}
