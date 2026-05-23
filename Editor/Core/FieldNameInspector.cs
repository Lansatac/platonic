using System;
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
            var typeProperty = property.FindPropertyRelative(nameof(SerializableFieldNameDefinition.Type));

            var typeName = element.Q<TextField>("TypeName");
            SetTypeNameVisible(typeName, IsCustomType(typeProperty));

            var typeField = element.Q<EnumField>("TypeDropdown");
            var typeDropdown = CreateTypeDropdown(typeProperty);
            var typeFieldParent = typeField.parent;
            var typeFieldIndex = typeFieldParent.IndexOf(typeField);
            typeField.RemoveFromHierarchy();
            typeFieldParent.Insert(typeFieldIndex, typeDropdown);

            typeDropdown.RegisterValueChangedCallback(_ =>
            {
                SetTypeNameVisible(typeName, IsCustomType(typeProperty));
            });

            return element;
        }

        private static PopupField<string> CreateTypeDropdown(SerializedProperty typeProperty)
        {
            var fieldTypes = GetFieldTypes();
            var displayNames = fieldTypes.Select(GetDisplayName).ToList();
            var selectedIndex = Math.Max(0, Array.IndexOf(fieldTypes, GetTypeValue(typeProperty)));
            var typeDropdown = new PopupField<string>("Type:", displayNames, selectedIndex)
            {
                name = "TypeDropdown",
                style =
                {
                    flexDirection = FlexDirection.Row,
                    flexGrow = 0,
                    minWidth = 145,
                    maxWidth = StyleKeyword.None,
                    alignContent = Align.Auto,
                    width = 145
                }
            };

            typeDropdown.RegisterValueChangedCallback(evt =>
            {
                var index = displayNames.IndexOf(evt.newValue);
                if (index < 0)
                {
                    return;
                }

                typeProperty.intValue = (int)fieldTypes[index];
                typeProperty.serializedObject.ApplyModifiedProperties();
            });

            return typeDropdown;
        }

        private static SerializableFieldNameDefinition.FieldType[] GetFieldTypes()
        {
            return new[]
            {
                SerializableFieldNameDefinition.FieldType.@int,
                SerializableFieldNameDefinition.FieldType.@float,
                SerializableFieldNameDefinition.FieldType.@bool,
                SerializableFieldNameDefinition.FieldType.@string,
                SerializableFieldNameDefinition.FieldType.Vector2,
                SerializableFieldNameDefinition.FieldType.Vector3,
                SerializableFieldNameDefinition.FieldType.IData,
                SerializableFieldNameDefinition.FieldType.custom
            };
        }

        private static SerializableFieldNameDefinition.FieldType GetTypeValue(SerializedProperty typeProperty)
        {
            return (SerializableFieldNameDefinition.FieldType)typeProperty.intValue;
        }

        private static string GetDisplayName(SerializableFieldNameDefinition.FieldType fieldType)
        {
            return fieldType switch
            {
                SerializableFieldNameDefinition.FieldType.IData => "IData",
                SerializableFieldNameDefinition.FieldType.custom => "Custom",
                _ => fieldType.ToString()
            };
        }

        private static bool IsCustomType(SerializedProperty typeProperty)
        {
            return GetTypeValue(typeProperty) == SerializableFieldNameDefinition.FieldType.custom;
        }

        private static void SetTypeNameVisible(VisualElement typeName, bool visible)
        {
            typeName.style.display = visible ? DisplayStyle.Flex : DisplayStyle.None;
        }
    }
}
