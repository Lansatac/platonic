#nullable enable
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Platonic.Core;
using Platonic.Scriptable;
using Platonic.Version;
using UnityEngine;

namespace Platonic.Render
{
    public class DataProvider : MonoBehaviour
    {
        public readonly VersionedReference<IData> Data;
        
        [SerializeField]
        private List<PreviewField> PreviewFields = null!;

        public DataProvider()
        {
            Data = new VersionedReference<IData>(() => InitialData);
        }

        /// <summary>
        /// Data object that represents the data as serialized on the provider, if any.
        /// </summary>
        public IData InitialData => new Data(PreviewFields ?? new List<PreviewField>());

        // void ISerializationCallbackReceiver.OnBeforeSerialize()
        // {
        // }
        //
        // void ISerializationCallbackReceiver.OnAfterDeserialize()
        // {
        //     Data.Ref = InitialData;
        // }
    }
}