#nullable enable
using System.Collections.Generic;
using System.Linq;
using Platonic.Core;
using Platonic.Scriptable;
using Platonic.Version;
using UnityEngine;

namespace Platonic.Render
{
    public class DataProvider : MonoBehaviour, ISerializationCallbackReceiver
    {
        public readonly VersionedReference<IData> Data = new();

        [SerializeField]
        private ScriptableData? ScriptableData;

        [SerializeField]
        private List<ScriptableField>? Fields;

        /// <summary>
        /// Data object that represents the data as serialized on the provider, if any.
        /// </summary>
        public IData? InitialData
        {
            get
            {
                IData? newData = null;

                if (Fields is { Count: > 0 })
                {
                    IEnumerable<IField> fields = Fields.Where(field => field != null);
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

                return newData;
            }
        }

        void ISerializationCallbackReceiver.OnBeforeSerialize()
        {
        }

        void ISerializationCallbackReceiver.OnAfterDeserialize()
        {
            Data.Ref = InitialData;
        }
    }
}