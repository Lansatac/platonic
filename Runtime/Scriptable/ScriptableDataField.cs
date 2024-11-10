#nullable enable
using Platonic.Core;
using UnityEngine;

namespace Platonic.Scriptable
{
    [CreateAssetMenu(fileName = "Data Field", menuName = "Data/Fields/Data")]
    public class ScriptableDataField : ScriptableField<IData>
    {
        public static void Create()
        {
            CreateInstance<ScriptableDataField>();
        }
    }
}