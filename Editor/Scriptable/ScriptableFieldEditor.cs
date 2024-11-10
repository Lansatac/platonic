using Platonic.Scriptable;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace Platonic.Editor.Scriptable
{
    public class ScriptableFieldEditor : UnityEditor.Editor
    {
        //public VisualTreeAsset InspectorXML;
        public override VisualElement CreateInspectorGUI()
        {
            var root = new VisualElement();
            //root.Add(new Label(target.GetType().GetGenericArguments()[0].Name));
            root.Add(new PropertyField(serializedObject.FindProperty("_name")));
            root.Add(new PropertyField(serializedObject.FindProperty("_value")));
            return root;
        }
    }

    [CustomEditor(typeof(ScriptableIntField))]
    public class ScriptableIntFieldEditor : ScriptableFieldEditor { }
    
    [CustomEditor(typeof(ScriptableFloatField))]
    public class ScriptableFloatFieldEditor : ScriptableFieldEditor { }
    
    [CustomEditor(typeof(ScriptableStringField))]
    public class ScriptableStringFieldEditor : ScriptableFieldEditor { }
    
    [CustomEditor(typeof(ScriptableBoolField))]
    public class ScriptableBoolFieldEditor : ScriptableFieldEditor { }
    
    [CustomEditor(typeof(ScriptableDataField))]
    public class ScriptableDataFieldEditor : ScriptableFieldEditor { }

}