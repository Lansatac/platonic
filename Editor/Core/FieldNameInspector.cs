using System.Collections.Generic;
using Platonic.Core;
using UnityEditor;
using UnityEngine.UIElements;

namespace Platonic.Editor.Core
{
    [CustomPropertyDrawer(typeof(SerializableFieldNameDefinition))]
    public class FieldNameInspector : PropertyDrawer
    {
        
        public VisualTreeAsset FieldNameXML;
        
        private static readonly List<string> TypeNameChoices= new(new[]
        {
            "int",
            "float",
            "string",
            "bool",
            
            "custom"
        });

        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            var element = FieldNameXML.Instantiate();
             var fn = (SerializableFieldNameDefinition)property.boxedValue;
 
            var typeName = element.Q<TextField>("TypeName");
            var typeProperty = property.FindPropertyRelative("Type");
            
             var dropdownField = element.Q<DropdownField>("TypeDropdown");
             dropdownField.choices = TypeNameChoices;
             var dropdownFieldIndex = TypeNameChoices.IndexOf(fn.Type);
            var customIndex = TypeNameChoices.IndexOf("custom");
            dropdownField.index = dropdownFieldIndex == -1 ? customIndex : dropdownFieldIndex;
            typeName.style.visibility = dropdownField.index == customIndex ? Visibility.Visible : Visibility.Hidden;
            dropdownField.RegisterValueChangedCallback(evt =>
            {
                typeName.style.visibility = evt.newValue == "custom" ? Visibility.Visible : Visibility.Hidden;
                if (evt.newValue != "custom")
                {
                    typeProperty.stringValue = evt.newValue;
                }
            });

            return element;
        }
    }
}