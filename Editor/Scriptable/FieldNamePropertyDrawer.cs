#nullable enable
using System.Linq;
using Platonic.Core;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace Platonic.Editor.Scriptable
{
     
     [CustomPropertyDrawer(typeof(IFieldName), true)]
     public class FieldNamePropertyDrawer : PropertyDrawer
     {
         public override VisualElement CreatePropertyGUI(SerializedProperty property)
         {
             var container = new VisualElement();

             var allNames = Names.Instance.GetAllNames().ToList();
             var nameOptions = allNames.Select(fieldName => $"{fieldName.Name}({fieldName.FieldType.Name})").ToList();
             
             var idProperty = property.FindPropertyRelative("_id");
             int index = 0;
             if (Names.Instance.TryGetName(idProperty.ulongValue, out var name))
             {
                 index = allNames.IndexOf(name);
             }
             else
             {
                 idProperty.ulongValue = allNames[index].Id;
                 property.serializedObject.ApplyModifiedProperties();
             }

             var nameDropdownField = new DropdownField("Name", nameOptions, index);
             nameDropdownField.RegisterValueChangedCallback(change =>
             {
                 var nameIndex = nameOptions.IndexOf(change.newValue);
                 idProperty.ulongValue = allNames[nameIndex].Id;
                 property.serializedObject.ApplyModifiedProperties();
             });

             container.Add(nameDropdownField);

             return container;
         }
         
         
         public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
         {
             var nameType = fieldInfo.FieldType.GetGenericArguments()[0];

             var allNames = Names.Instance.GetAssignableToType(nameType).ToList();
             var nameOptions = allNames.Select(fieldName => fieldName.Name).ToArray();
            
             var idProperty = property.FindPropertyRelative("_id");
             int index = 0;
             if (Names.Instance.TryGetName(idProperty.ulongValue, out var name))
             {
                 index = allNames.IndexOf(name);
             }
             else
             {
                 idProperty.ulongValue = allNames[index].Id;
                 property.serializedObject.ApplyModifiedProperties();
             }
             var newIndex = EditorGUILayout.Popup("Name", index, nameOptions);
             idProperty.ulongValue = allNames[newIndex].Id;
         }
     }
    
}