#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;
using Platonic.Core;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;
using PopupWindow = UnityEditor.PopupWindow;

namespace Platonic.Editor.Render
{
    [CustomPropertyDrawer(typeof(PreviewField), true)]
    public class PreviewFieldPropertyDrawer : PropertyDrawer
    {
        private sealed class NameSelectionPopupWindowContent : PopupWindowContent
        {
            private readonly SerializedProperty _nameProperty;
            private readonly List<IFieldName> _allNames;
            private readonly List<string> _displayOptions;
            private readonly SerializedObject _serializedObject;
            private readonly Action<string> _onSelected;
            private readonly VisualElement _widthReference;
            private ScrollView _scrollView = null!;

            public NameSelectionPopupWindowContent(
                SerializedProperty nameProperty,
                List<IFieldName> allNames,
                List<string> displayOptions,
                SerializedObject serializedObject,
                Action<string> onSelected,
                VisualElement widthReference)
            {
                _nameProperty = nameProperty;
                _allNames = allNames;
                _displayOptions = displayOptions;
                _serializedObject = serializedObject;
                _onSelected = onSelected;
                _widthReference = widthReference;
            }

            public override Vector2 GetWindowSize()
            {
                float listHeight = Mathf.Min(300, _displayOptions.Count * (EditorGUIUtility.singleLineHeight + 2) + 10);
                float searchBarHeight = EditorGUIUtility.singleLineHeight + 5;
                float height = listHeight + searchBarHeight;

                float width = 350;
                width = Mathf.Max(width, _widthReference.resolvedStyle.width);

                return new Vector2(width, height);
            }

            public override void OnOpen()
            {
                var root = editorWindow.rootVisualElement;

                var searchField = new TextField
                {
                    label = "Search:",
                    style = { marginBottom = 5 }
                };
                searchField.RegisterValueChangedCallback(evt => { PopulateScrollView(evt.newValue); });
                root.Add(searchField);
                
                // Auto-focus the search field
                searchField.schedule.Execute(() => searchField.Focus());

                _scrollView = new ScrollView(ScrollViewMode.Vertical);
                root.Add(_scrollView);

                PopulateScrollView(string.Empty);
            }

            private void PopulateScrollView(string filter)
            {
                _scrollView.Clear();

                for (int i = 0; i < _displayOptions.Count; i++)
                {
                    if (string.IsNullOrEmpty(filter) ||
                        _displayOptions[i].ToLowerInvariant().Contains(filter.ToLowerInvariant()))
                    {
                        int capturedIndex = i;
                        var button = new Button(() =>
                        {
                            var selectedName = _allNames[capturedIndex].Name;
                            _nameProperty.stringValue = selectedName;
                            _serializedObject.ApplyModifiedProperties();
                            _onSelected(selectedName);
                            editorWindow.Close();
                        })
                        {
                            style =
                            {
                                flexDirection = FlexDirection.Row,
                                justifyContent = Justify.SpaceBetween,
                                alignItems = Align.Center,
                                unityTextAlign = TextAnchor.MiddleLeft,
                                paddingLeft = 5,
                                height = EditorGUIUtility.singleLineHeight + 2
                            }
                        };
                        var nameLabel = new Label(_allNames[capturedIndex].Name)
                        {
                            style =
                            {
                                flexGrow = 1,
                                unityTextAlign = TextAnchor.MiddleLeft
                            }
                        };
                        var typeLabel = new Label(_allNames[capturedIndex].FieldType.Name)
                        {
                            style =
                            {
                                flexShrink = 0,
                                marginLeft = 8,
                                unityTextAlign = TextAnchor.MiddleRight
                            }
                        };
                        button.Add(nameLabel);
                        button.Add(typeLabel);
                        _scrollView.Add(button);
                    }
                }
            }
        }

        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            var root = new VisualElement
            {
                style =
                {
                    flexGrow = 1,
                    flexDirection = FlexDirection.Row
                }
            };

            var fieldNameProperty = property.FindPropertyRelative("_fieldName");
            var allNames = Names.Instance.GetAllNames().ToList();
            var displayOptionsForPopup = allNames
                .Select(fieldName => $"{fieldName.Name}({fieldName.FieldType.Name})")
                .ToList();

            var nameRow = new VisualElement { style = { flexDirection = FlexDirection.Row } };
            var nameLabel = new Label { style = { flexGrow = 1 } };

            var selectButton = new Button { text = "▼" };

            nameRow.Add(nameLabel);
            nameRow.Add(selectButton);
            root.Add(nameRow);

            var intField = new IntegerField { name = "IntValue" };
            intField.BindProperty(property.FindPropertyRelative("_intValue"));

            var floatField = new FloatField { name = "FloatValue" };
            floatField.BindProperty(property.FindPropertyRelative("_floatValue"));

            var boolField = new Toggle { name = "BoolValue" };
            boolField.BindProperty(property.FindPropertyRelative("_boolValue"));

            var stringField = new TextField { name = "StringValue" };
            stringField.style.flexGrow = 1;
            stringField.BindProperty(property.FindPropertyRelative("_stringValue"));

            root.Add(intField);
            root.Add(floatField);
            root.Add(boolField);
            root.Add(stringField);

            string GetDisplayName(string fieldName)
            {
                return Names.Instance.TryGetName(fieldName, out var resolvedFieldName)
                    ? $"{resolvedFieldName.Name}({resolvedFieldName.FieldType.Name})"
                    : (string.IsNullOrEmpty(fieldName) ? "None" : fieldName);
            }

            void UpdateUI(string fieldName)
            {
                if (Names.Instance.TryGetName(fieldName, out var resolvedFieldName))
                {
                    root.Q("StringValue").visible = resolvedFieldName.FieldType == typeof(string);
                    root.Q("IntValue").visible = resolvedFieldName.FieldType == typeof(int);
                    root.Q("FloatValue").visible = resolvedFieldName.FieldType == typeof(float);
                    root.Q("BoolValue").visible = resolvedFieldName.FieldType == typeof(bool);
                }

                nameLabel.text = GetDisplayName(fieldName);
            }

            if (string.IsNullOrEmpty(fieldNameProperty.stringValue) && allNames.Count > 0)
            {
                fieldNameProperty.stringValue = allNames[0].Name;
                property.serializedObject.ApplyModifiedProperties();
            }

            selectButton.clicked += () =>
            {
                var popupContent = new NameSelectionPopupWindowContent(
                    fieldNameProperty,
                    allNames,
                    displayOptionsForPopup,
                    property.serializedObject,
                    UpdateUI,
                    nameRow
                );
                PopupWindow.Show(selectButton.worldBound, popupContent);
            };

            UpdateUI(fieldNameProperty.stringValue);

            return root;
        }
    }
}
