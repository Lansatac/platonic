#nullable enable
using System;
using System.Collections.Generic;
using Platonic.Core;
using UnityEngine;

namespace Platonic.Scriptable
{
    [CreateAssetMenu(fileName = "Data", menuName = "Data/Scriptable Data")]
    public class GenericScriptableData : ScriptableData
    {
        [SerializeField] private List<ScriptableField> SerializedFields = new();

        protected override IEnumerable<IField> GetFields()
        {
            return SerializedFields;
        }
    }
}