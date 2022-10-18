using UnityEngine;

namespace Platonic.Scriptable
{
    [CreateAssetMenu(fileName = "Bool Field", menuName = "Data/Fields/Bool")]
    public class ScriptableBoolField : ScriptableField<bool>
    {
        public static void Create()
        {
            CreateInstance<ScriptableBoolField>();
        }
    }
}