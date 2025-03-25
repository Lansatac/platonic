using Platonic.Render;
using UnityEngine;

namespace Platonic
{
    public class ObjectActiveRenderer : FieldRenderer<bool>
    {
        [SerializeField] private GameObject Target;
        protected override void FieldChanged(bool newValue)
        {
            Target.SetActive(newValue);
        }
    }
}
