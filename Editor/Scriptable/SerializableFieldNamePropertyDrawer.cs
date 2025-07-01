#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;
using Platonic.Core;
using Platonic.Scriptable;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using PopupWindow = UnityEditor.PopupWindow;

namespace Platonic.Editor.Scriptable
{
    public class NameSelectionUIPopupWindowContent : PopupWindowContent
    {
        private readonly SerializedProperty _idProperty;
        private readonly List<IFieldName> _allNames;
        private readonly List<string> _displayOptions;
        private readonly TextField _nameFieldToUpdate;
        private readonly SerializedObject _serializedObject;
        private ScrollView _scrollView = null!;

        public NameSelectionUIPopupWindowContent(SerializedProperty idProperty, List<IFieldName> allNames,
            List<string> displayOptions, TextField nameFieldToUpdate, SerializedObject serializedObject)
        {
            _idProperty = idProperty;
            _allNames = allNames;
            _displayOptions = displayOptions;
            _nameFieldToUpdate = nameFieldToUpdate;
            _serializedObject = serializedObject;
        }

        public override Vector2 GetWindowSize()
        {
            // Adjust height for the search bar
            float listHeight = Mathf.Min(300, _displayOptions.Count * (EditorGUIUtility.singleLineHeight + 2) + 10);
            float searchBarHeight = EditorGUIUtility.singleLineHeight + 5; // Approximate height for TextField
            float height = listHeight + searchBarHeight; 
            
            float width = 250;
            width = Mathf.Max(width, _nameFieldToUpdate.resolvedStyle.width);

            return new Vector2(width, height);
        }

        public override void OnOpen()
        {
            var root = editorWindow.rootVisualElement;

            var searchField = new TextField
            {
                label = "Search:", // Optional: add a label
                style = { marginBottom = 5 } // Add some spacing
            };
            searchField.RegisterValueChangedCallback(evt =>
            {
                PopulateScrollView(evt.newValue);
            });
            root.Add(searchField);

            _scrollView = new ScrollView(ScrollViewMode.Vertical);
            root.Add(_scrollView);

            PopulateScrollView(string.Empty); // Initial population
        }

        private void PopulateScrollView(string filter)
        {
            _scrollView.Clear(); // Clear existing buttons

            for (int i = 0; i < _displayOptions.Count; i++)
            {
                // Case-insensitive search
                if (string.IsNullOrEmpty(filter) || _displayOptions[i].ToLowerInvariant().Contains(filter.ToLowerInvariant()))
                {
                    int capturedIndex = i; // Capture index for the lambda
                    var button = new Button(() =>
                    {
                        _idProperty.ulongValue = _allNames[capturedIndex].ID;
                        _serializedObject.ApplyModifiedProperties();
                        _nameFieldToUpdate.value = _displayOptions[capturedIndex];

                        editorWindow.Close();
                    })
                    {
                        text = _displayOptions[capturedIndex],
                        style =
                        {
                            unityTextAlign = TextAnchor.MiddleLeft,
                            paddingLeft = 5,
                            height = EditorGUIUtility.singleLineHeight + 2
                        }
                    };
                    _scrollView.Add(button);
                }
            }
        }

        public override void OnClose()
        {
        }
    }

    [CustomPropertyDrawer(typeof(SerializableFieldName))]
    public class UntypedSerializableFieldNamePropertyDrawer : PropertyDrawer
    {
        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            var container = new VisualElement();

            var allNames = Names.Instance.GetAllNames().ToList();
            // This list is used for display in the popup
            var displayOptionsForPopup =
                allNames.Select(fieldName => $"{fieldName.Name}({fieldName.FieldType.Name})").ToList();

            var idProperty = property.FindPropertyRelative("ID");
            int index = 0;
            if (Names.Instance.TryGetName(idProperty.ulongValue, out var name))
            {
                index = allNames.IndexOf(name);
            }
            else
            {
                if (allNames.Any()) // Ensure allNames is not empty
                {
                    idProperty.ulongValue = allNames[index].ID; // index is 0
                    property.serializedObject.ApplyModifiedProperties();
                }
            }

            var nameField = new TextField("Name");
            nameField.SetEnabled(false);
            nameField.value = (index >= 0 && index < displayOptionsForPopup.Count && allNames.Any())
                ? displayOptionsForPopup[index]
                : "None";

            var selectButton = new Button();
            selectButton.clicked += () =>
            {
                var popupContent = new NameSelectionUIPopupWindowContent(
                    idProperty,
                    allNames,
                    displayOptionsForPopup,
                    nameField,
                    property.serializedObject
                );
                PopupWindow.Show(selectButton.worldBound, popupContent);
            };
            selectButton.text = "▼";

            var fieldRow = new VisualElement();
            fieldRow.style.flexDirection = FlexDirection.Row;
            nameField.style.flexGrow = 1; // Allow nameField to take available space
            fieldRow.Add(nameField);
            fieldRow.Add(selectButton);

            container.Add(fieldRow);

            return container;
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
        }
    }

    [CustomPropertyDrawer(typeof(SerializableFieldName<>))]
    public class SerializableFieldNamePropertyDrawer : PropertyDrawer
    {
        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            var container = new VisualElement();

            var nameType = fieldInfo.FieldType.GetGenericArguments()[0];

            var allNames = Names.Instance.GetAssignableToType(nameType).ToList();
            // This list is used for display in the popup
            var displayOptionsForPopup = allNames.Select(fieldName => fieldName.Name).ToList();

            var idProperty = property.FindPropertyRelative("ID");
            int index = 0;
            if (Names.Instance.TryGetName(idProperty.ulongValue, out var name))
            {
                index = allNames.IndexOf(name);
            }
            else
            {
                if (allNames.Any()) // Ensure allNames is not empty
                {
                    idProperty.ulongValue = allNames[index].ID; // index is 0
                    property.serializedObject.ApplyModifiedProperties();
                }
            }

            var nameField = new TextField(property.displayName); // Using property.displayName for the label
            nameField.SetEnabled(false);
            nameField.value = (index >= 0 && index < displayOptionsForPopup.Count && allNames.Any())
                ? displayOptionsForPopup[index]
                : "None";

            var selectButton = new Button();
            selectButton.clicked += () =>
            {
                var popupContent = new NameSelectionUIPopupWindowContent(
                    idProperty,
                    allNames,
                    displayOptionsForPopup,
                    nameField,
                    property.serializedObject
                );
                PopupWindow.Show(selectButton.worldBound, popupContent);
            };
            selectButton.text = "▼";

            var fieldRow = new VisualElement();
            fieldRow.style.flexDirection = FlexDirection.Row;
            nameField.style.flexGrow = 1; // Allow nameField to take available space
            fieldRow.Add(nameField);
            fieldRow.Add(selectButton);

            container.Add(fieldRow);

            return container;
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
        }
    }
}