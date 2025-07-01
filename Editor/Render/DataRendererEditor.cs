#nullable enable
using Platonic.Render;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace Platonic.Editor.Render
{
    [CustomEditor(typeof(DataRenderer), true)]
    public class DataRendererEditor : UnityEditor.Editor
    {
        public override VisualElement CreateInspectorGUI()
        {
            var root = new VisualElement();
            root.Add(new PropertyField(serializedObject.FindProperty("FieldName")));
            
            var selectProviderButton = new Button(() =>
            {
                var dataRenderer = (DataRenderer)target;
                if (dataRenderer.TryGetComponent<DataRenderer>(out var renderer))
                {
                    var provider = renderer.GetComponent<DataProvider>();
                    if (provider != null)
                    {
                        Selection.activeObject = provider;
                    }
                }
            })
            {
                text = "Select Data Provider"
            };
            root.Add(selectProviderButton);
            
            return root;
        }
    }
}