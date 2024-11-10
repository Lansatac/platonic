#nullable enable
using UnityEngine;

namespace Platonic.Scriptable
{
    [CreateAssetMenu(fileName = "Int Field", menuName = "Data/Fields/Int")]
    public class ScriptableIntField : ScriptableField<int>
    {
        public static void Create()
        {
            CreateInstance<ScriptableIntField>();
        }
    }
}