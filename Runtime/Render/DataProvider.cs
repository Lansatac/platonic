﻿#nullable enable
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
        public IData? Data
        {
            get => DataReference.Ref;
            set => DataReference.Ref = value;
        }
        public readonly VersionedReference<IData?> DataReference;
        
        [SerializeField]
        private List<PreviewField> PreviewFields = null!;

        public DataProvider()
        {
            DataReference = new VersionedReference<IData?>(() => InitialData);
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