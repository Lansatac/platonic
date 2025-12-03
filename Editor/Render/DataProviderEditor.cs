#nullable enable
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Platonic.Core;
using Platonic.Render;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;
using Cursor = UnityEngine.UIElements.Cursor;

namespace Platonic.Editor.Render
{
    [CustomEditor(typeof(DataProvider))]
    public class DataProviderEditor : UnityEditor.Editor
    {
        public VisualTreeAsset? InspectorXML;
        public VisualTreeAsset? PreviewField;

        private struct RequestedFieldInfo
        {
            public string FieldName;
            public string FieldType;
            public string ComponentName;
            public ProviderRenderer Renderer;
        }

        private void GetProviderRenderers(Transform root, List<ProviderRenderer> renderers)
        {
            renderers.AddRange(root.GetComponents<ProviderRenderer>());

            foreach (Transform child in root)
            {
                // Exclude other DataProviders, their children aren't ours
                if (child.GetComponent<DataProvider>() != null)
                    continue;

                GetProviderRenderers(child, renderers);
            }
        }

        private List<RequestedFieldInfo> GetRequestedFields()
        {
            var fields = new List<RequestedFieldInfo>();
            var dataProvider = (DataProvider)target;
            var renderers = new List<ProviderRenderer>();
            GetProviderRenderers(dataProvider.transform, renderers);

            foreach (var renderer in renderers)
            {
                var so = new SerializedObject(renderer);
                var prop = so.GetIterator();
                var componentName = $"{renderer.GetType().Name} ({renderer.gameObject.name})";

                while (prop.NextVisible(true))
                {
                    // Check if this is a SerializableFieldName field
                    if (prop.type.Contains("SerializableFieldName"))
                    {
                        var idProp = prop.FindPropertyRelative("ID");
                        if (idProp != null && idProp.ulongValue != 0)
                        {
                            try
                            {
                                var fieldName = Names.Instance.GetName(idProp.ulongValue);
                                fields.Add(new RequestedFieldInfo
                                {
                                    FieldName = fieldName.Name,
                                    FieldType = fieldName.FieldType.Name,
                                    ComponentName = componentName,
                                    Renderer = renderer
                                });
                            }
                            catch
                            {
                                // Field ID not found in Names registry
                            }
                        }
                    }
                }
            }

            return fields;
        }

        public override VisualElement CreateInspectorGUI()
        {
            var dataProvider = (DataProvider)target;

            var root = new VisualElement();

            if (InspectorXML == null) return root;

            InspectorXML.CloneTree(root);

            // Setup Requested Fields Preview
            var requestedFieldsList = root.Q<ListView>("RequestedFieldsPreview");
            var requestedFields = GetRequestedFields();

            if (requestedFieldsList != null)
            {
                requestedFieldsList.makeItem = () =>
                {
                    var container = new VisualElement
                    {
                        style =
                        {
                            flexDirection = new StyleEnum<FlexDirection>(FlexDirection.Row),
                            cursor = new StyleCursor(new Cursor()) // Hand cursor
                        }
                    };
                    container.Add(new Label
                    {
                        name = "FieldLabel"
                    });
                    container.Add(new Label
                    {
                        name = "ComponentLabel",
                        style =
                        {
                            unityFontStyleAndWeight = FontStyle.Italic,
                            marginLeft = Length.Auto()
                        }
                    });
                    return container;
                };
                requestedFieldsList.bindItem = (element, i) =>
                {
                    var fieldInfo = requestedFields[i];
                    element.Q<Label>("FieldLabel").text = $"{fieldInfo.FieldName} ({fieldInfo.FieldType})";
                    element.Q<Label>("ComponentLabel").text = fieldInfo.ComponentName;

                    // Add click handler to highlight the GameObject
                    element.RegisterCallback<ClickEvent>(evt =>
                    {
                        Selection.activeGameObject = fieldInfo.Renderer.gameObject;
                        EditorGUIUtility.PingObject(fieldInfo.Renderer.gameObject);
                    });
                };
                requestedFieldsList.itemsSource = requestedFields;
                requestedFieldsList.focusable = false;
            }

            // Setup Copy Button
            var copyButton = root.Q<Button>("CopyFieldNamesButton");
            if (copyButton != null)
            {
                copyButton.clicked += () =>
                {
                    var fieldNames = requestedFields.Select(f => f.FieldName).Distinct();
                    var copyText = string.Join("\n", fieldNames);
                    GUIUtility.systemCopyBuffer = copyText;
                };
            }

            if (dataProvider.Data != null)
            {
                var list = root.Q<ListView>("DataFieldsPreview");
                list.makeItem = () =>
                {
                    var previewField = new VisualElement
                    {
                        style =
                        {
                            flexDirection = new StyleEnum<FlexDirection>(FlexDirection.Row)
                        }
                    };
                    previewField.Add(new Label
                    {
                        name = "NameLabel"
                    });
                    previewField.Add(new Label
                    {
                        name = "ValueLabel"
                    });
                    previewField.Add(new Label
                    {
                        name = "VersionLabel",
                        style =
                        {
                            marginLeft = Length.Auto()
                        }
                    });
                    return previewField;
                };
                list.bindItem = (element, i) =>
                {
                    var field = dataProvider.Data.Fields.ElementAt(i);
                    element.Q<Label>("NameLabel").text = $"{field.Name.Name}({field.Name.FieldType.Name}):";
                    element.Q<Label>("ValueLabel").text = $"{FieldValueString(field.Value)}";
                    element.Q<Label>("VersionLabel").text = $"Version: {field.Version}";
                };
                list.itemsSource = dataProvider.Data.Fields.ToList();
                list.focusable = false;
            }

            return root;
        }

        string FieldValueString(object? fieldValue)
        {
            if (fieldValue == null)
            {
                return "null";
            }

            if (fieldValue is string value)
            {
                return value;
            }

            if (fieldValue is IEnumerable list)
            {
                var elements = new List<string>();
                foreach (var element in list)
                {
                    elements.Add(FieldValueString(element));
                }

                return $"[{string.Join(", ", elements)}]";
            }

            return $"{fieldValue}";
        }
    }
}