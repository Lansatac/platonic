using System.Linq;
using Platonic.Core;
using Platonic.Scriptable;
using UnityEditor;
using UnityEngine.UIElements;

namespace Platonic.Editor.Scriptable
{
    
    [CustomPropertyDrawer(typeof(SerializableFieldName))]
    public class UntypedSerializableFieldNamePropertyDrawer : PropertyDrawer
    {

        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            var container = new VisualElement();

            var allNames = Names.Instance.GetAllNames().ToList();
            var nameOptions = allNames.Select(fieldName => $"{fieldName.Name}({fieldName.FieldType.Name})").ToList();
            
            var idProperty = property.FindPropertyRelative("ID");
            int index = 0;
            if (Names.Instance.TryGetName(idProperty.ulongValue, out var name))
            {
                index = allNames.IndexOf(name);
            }
            else
            {
                idProperty.ulongValue = allNames[index].ID;
                property.serializedObject.ApplyModifiedProperties();
            }

            var nameDropdownField = new DropdownField("Name", nameOptions, index);
            nameDropdownField.RegisterValueChangedCallback(change =>
            {
                var nameIndex = nameOptions.IndexOf(change.newValue);
                idProperty.ulongValue = allNames[nameIndex].ID;
                property.serializedObject.ApplyModifiedProperties();
            });

            container.Add(nameDropdownField);

            return container;
        }
    }
    
    [CustomPropertyDrawer(typeof(SerializableFieldName<>))]
    public class SerializableFieldNamePropertyDrawer : PropertyDrawer
    {

        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            var container = new VisualElement();

            var nameType = fieldInfo.FieldType.GetGenericArguments()[0];

            var allNames = Names.Instance.GetNamesOfType(nameType).ToList();
            var nameOptions = allNames.Select(fieldName => fieldName.Name).ToList();
            
            var idProperty = property.FindPropertyRelative("ID");
            int index = 0;
            if (Names.Instance.TryGetName(idProperty.ulongValue, out var name))
            {
                index = allNames.IndexOf(name);
            }
            else
            {
                idProperty.ulongValue = allNames[index].ID;
                property.serializedObject.ApplyModifiedProperties();
            }

            var nameDropdownField = new DropdownField("Name", nameOptions, index);
            nameDropdownField.RegisterValueChangedCallback(change =>
            {
                var nameIndex = nameOptions.IndexOf(change.newValue);
                idProperty.ulongValue = allNames[nameIndex].ID;
                property.serializedObject.ApplyModifiedProperties();
            });

            container.Add(nameDropdownField);

            return container;
        }
    }
}