using System;
using System.Collections.Generic;
using System.Linq;
using Platonic.Core;
using UnityEditor;
using UnityEngine.UIElements;

namespace Platonic.Editor.Core
{
    [CustomPropertyDrawer(typeof(SerializableFieldNameDefinition))]
    public class FieldNameInspector : PropertyDrawer
    {
        public VisualTreeAsset FieldNameXML;

        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            var element = FieldNameXML.Instantiate();
            var target = (SerializableFieldNameDefinition)property.boxedValue;

            var typeName = element.Q<TextField>("TypeName");
            typeName.style.visibility = target.Type == SerializableFieldNameDefinition.FieldType.custom ? Visibility.Visible : Visibility.Hidden;
            
            var dropdownField = element.Q<DropdownField>("TypeDropdown");
            dropdownField.RegisterValueChangedCallback(evt =>
            {
                typeName.style.visibility = evt.newValue == "Custom" ? Visibility.Visible : Visibility.Hidden;
            });

            return element;
        }
    }
}