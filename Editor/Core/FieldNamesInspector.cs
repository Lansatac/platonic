using System.Collections.Generic;
using Platonic.Editor.Generator;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace Platonic.Editor.Core
{
    [CustomEditor(typeof(FieldNames))]
    public class FieldNamesInspector : UnityEditor.Editor
    {
        public VisualTreeAsset m_InspectorXML;
        public VisualTreeAsset FieldNameXML;
        private static readonly List<string> TypeNameChoices= new(new[]
        {
            "int",
            "float",
            "string",
            "bool",
            
            "custom"
        });

        public override VisualElement CreateInspectorGUI()
        {
            var fieldNames = new SerializedObject((FieldNames)target);
            var serializableNames = fieldNames.FindProperty("Names");
            
            // Create a new VisualElement to be the root of our Inspector UI.
            var namesInspector = m_InspectorXML.Instantiate();

            namesInspector.Q<Button>("GenerateButton").clickable.clicked += GenerateFieldNames.GenCode;
            var nameList = namesInspector.Q<ListView>("NameList");
            nameList.bindItem += (element, i) =>
            {
                if (i >= serializableNames.arraySize)
                    serializableNames.arraySize = i+1;
                var serializableElement = serializableNames.GetArrayElementAtIndex(i);
                if (serializableElement == null) return;
                var fn = (FieldNames.FieldName)serializableElement.boxedValue;
                
                var fieldName = element.Q<TextField>("FieldName");
                fieldName.BindProperty(serializableElement.FindPropertyRelative("Name"));
                
                var typeName = element.Q<TextField>("TypeName");
                var typeProperty = serializableElement.FindPropertyRelative("Type");
                typeName.BindProperty(typeProperty);
                
                var dropdownField = element.Q<DropdownField>("TypeDropdown");
                dropdownField.choices = TypeNameChoices;
                var dropdownFieldIndex = TypeNameChoices.IndexOf(fn.Type);
                dropdownField.index = dropdownFieldIndex == -1 ? TypeNameChoices.IndexOf("custom") : dropdownFieldIndex;
                typeName.style.visibility = typeProperty.stringValue == "custom" ? Visibility.Visible : Visibility.Hidden;
                dropdownField.RegisterValueChangedCallback(evt =>
                {
                    typeName.style.visibility = evt.newValue == "custom" ? Visibility.Visible : Visibility.Hidden;
                    if (evt.newValue != "custom")
                    {
                        typeProperty.stringValue = evt.newValue;
                        fieldNames.ApplyModifiedProperties();
                    }
                });
                
            };
            
            nameList.makeItem += () =>
            {
                var element = FieldNameXML.CloneTree();
                // var dropdown = element.Q<DropdownField>(name: "TypeDropdown");
                // dropdown.RegisterValueChangedCallback(evt =>
                // {
                //     var hpColor = element.Q<VisualElement>("hpColor");
                //     var i = (int)slider.userData;
                //     var characterInfo = items[i];
                //     characterInfo.currentHp = evt.newValue;
                //     SetHp(slider, hpColor, characterInfo);
                // });
                // var serializableElement = serializableNames.GetArrayElementAtIndex(i);
                // var fn = (FieldNames.FieldName)serializableElement.boxedValue;
                // var dropdownField = tree.Q<DropdownField>("TypeDropdown");
                // dropdownField.choices = TypeNameChoices;
                // var dropdownFieldIndex = TypeNameChoices.IndexOf(fn.Type);
                // dropdownField.index = dropdownFieldIndex == -1 ? TypeNameChoices.IndexOf("custom") : dropdownFieldIndex;
                // dropdownField.RegisterValueChangedCallback(evt => Debug.Log(evt.newValue));
                return element;
            };
                


            // Return the finished Inspector UI.
            return namesInspector;
        }
    }
}