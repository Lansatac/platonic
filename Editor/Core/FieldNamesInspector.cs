using System.Collections.Generic;
using Platonic.Editor.Generator;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.Serialization;
using UnityEngine.UIElements;

namespace Platonic.Editor.Core
{
    [CustomEditor(typeof(FieldNames))]
    public class FieldNamesInspector : UnityEditor.Editor
    {
        public VisualTreeAsset InspectorXML;

        public override VisualElement CreateInspectorGUI()
        {
            var fieldNames = new SerializedObject((FieldNames)target);
            
            // Create a new VisualElement to be the root of our Inspector UI.
            var namesInspector = InspectorXML.Instantiate();

            namesInspector.Q<Button>("GenerateButton").clickable.clicked += GenerateFieldNames.GenCode;
            
            var usingsList = namesInspector.Q<ListView>("UsingsList");
            usingsList.makeItem = () => new TextField();

            // Return the finished Inspector UI.
            return namesInspector;
        }
    }
}