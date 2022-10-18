using Platonic.Core;
using UnityEngine;

namespace Platonic.Scriptable
{
    [CreateAssetMenu(fileName = "String Field", menuName = "Data/Fields/String")]
    public class ScriptableStringField : ScriptableField<string>
    {
        public static void Create()
        {
            CreateInstance<ScriptableStringField>();
        }
    }
}