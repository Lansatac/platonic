using Platonic.Render;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace Platonic.Editor.Render
{
    [CustomEditor(typeof(TextRenderer))]
    public class TextRendererEditor : UnityEditor.Editor
    {
        
        public override VisualElement CreateInspectorGUI()
        {
            var root = new VisualElement();
            root.Add(new PropertyField(serializedObject.FindProperty("FormatString")));
            root.Add(new PropertyField(serializedObject.FindProperty("FieldsToWatch")));
            return root;
        }
    }
}