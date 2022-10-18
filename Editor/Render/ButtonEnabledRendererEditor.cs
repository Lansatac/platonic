using Platonic.Render;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace Platonic.Editor.Render
{
    [CustomEditor(typeof(ButtonInteractableRenderer))]
    public class ButtonEnabledRendererEditor : UnityEditor.Editor
    {
        public override VisualElement CreateInspectorGUI()
        {
            var root = new VisualElement();
            root.Add(new PropertyField(this.serializedObject.FindProperty("FieldName")));
            root.Add(new PropertyField(this.serializedObject.FindProperty("Invert")));
            return root;
        }
    }
}