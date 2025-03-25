#nullable enable
using Platonic.Core;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace Platonic.Editor.Render
{
    [CustomPropertyDrawer(typeof(PreviewField), true)]
    public class PreviewFieldPropertyDrawer : PropertyDrawer
    {
        public VisualTreeAsset? InspectorXML;
        
        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            if (InspectorXML == null) return new VisualElement();
            
            var root = InspectorXML.Instantiate();
            
            var nameMenu = root.Q<ToolbarMenu>("NameMenu");
            foreach (var name in Names.Instance.GetAllNames())
            {
                nameMenu.menu.AppendAction(name.Name, action =>
                {
                    property.FindPropertyRelative("_fieldName").stringValue = action.name;
                    property.serializedObject.ApplyModifiedProperties();
                });
            }

            root.Q<Label>().RegisterValueChangedCallback(change =>
            {
                if (Names.Instance.TryGetName(change.newValue, out var fieldName))
                {
                    root.Q("StringValue").visible = fieldName.FieldType == typeof(string);
                    root.Q("IntValue").visible = fieldName.FieldType == typeof(int);
                    root.Q("FloatValue").visible = fieldName.FieldType == typeof(float);
                    root.Q("BoolValue").visible = fieldName.FieldType == typeof(bool);
                }
            });
            
            return root;
        }
    }
}