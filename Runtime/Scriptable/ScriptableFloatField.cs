#nullable enable
using UnityEngine;

namespace Platonic.Scriptable
{
    [CreateAssetMenu(fileName = "Float Field", menuName = "Data/Fields/Float")]
    public class ScriptableFloatField : ScriptableField<float>
    {
        public static void Create()
        {
            CreateInstance<ScriptableFloatField>();
        }
    }
}