using System;
using System.Collections.Generic;
using System.Linq;
using Platonic.Core;
using Platonic.Scriptable;
using Platonic.Version;
using UnityEngine;

namespace Platonic.Render
{
    public class DataProvider : MonoBehaviour
    {
        public readonly VersionedReference<IData> Data = new VersionedReference<IData>();

        [SerializeField]
        private ScriptableData? ScriptableData;

        [SerializeField]
        private List<ScriptableField>? Fields;

        private void OnValidate()
        {
            IData? newData = null;

            if (Fields != null)
            {
                IEnumerable<IField> fields = Fields;
                if (ScriptableData != null)
                {
                    fields = fields.Concat(ScriptableData.Fields);
                }

                newData = new Data(fields);
            }
            else if (ScriptableData != null)
            {
                newData = ScriptableData;
            }

            if (newData != null)
            {
                Data.Ref = newData;
            }
        }
    }
}