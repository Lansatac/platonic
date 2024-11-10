using System.Collections.Generic;
using Platonic.Core;
using UnityEngine;

namespace Platonic.Scriptable
{
    [CreateAssetMenu(fileName = "List Of Data Field", menuName = "Data/Fields/List Of Data")]
    public class ScriptableListOfDataField: ScriptableField<IEnumerable<IData>>
    {
        [SerializeField] private List<ScriptableData>? Data = null;
        protected override IEnumerable<IData>? GetSerializedValue()
        {
            return Data;
        }
        
    }
}