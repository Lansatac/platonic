using System.Linq;
using Platonic.Core;
using Platonic.Render;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace Platonic.Editor.Render
{
    [CustomEditor(typeof(DataProvider))]
    public class DataProviderEditor : UnityEditor.Editor
    {
        
        public VisualTreeAsset? InspectorXML;
        public VisualTreeAsset? FieldPreview;
        public override VisualElement CreateInspectorGUI()
        {
            var dataProvider = (DataProvider)target;
            
            var root = new VisualElement();
            
            if (InspectorXML == null) return root;
            
            InspectorXML.CloneTree(root);
            
            if (dataProvider.Data.Ref != null)
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
                    var nameLabel = new Label();
                    nameLabel.name = "NameLabel";
                    var valueLabel = new Label();
                    valueLabel.name = "ValueLabel";
                    previewField.Add(nameLabel);
                    previewField.Add(valueLabel);
                    return previewField;
                };
                list.bindItem = (element, i) =>
                {
                    var field = dataProvider.Data.Ref.Fields.ElementAt(i);
                    element.Q<Label>("NameLabel").text = $"{field.Name.Name}({field.Name.FieldType.Name}):";
                    element.Q<Label>("ValueLabel").text = $"{field.Value}";
                };
                list.itemsSource = dataProvider.Data.Ref.Fields.ToList();
                list.focusable = false;
            }
            return root;
        }
    }
}