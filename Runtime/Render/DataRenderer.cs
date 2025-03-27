#nullable enable
using System;
using Platonic.Core;
using UnityEngine;

namespace Platonic.Render
{
    [RequireComponent(typeof(DataProvider))]
    public class DataRenderer : FieldRenderer<IData>
    {
        private DataProvider? _forwardedProvider;
    
        protected override void ProviderAwake()
        {
            _forwardedProvider = GetComponent<DataProvider>();
        }

        protected override DataProvider LocateProvider()
        {
            return GetComponentsInParent<DataProvider>()[1];
        }

        private void Update()
        {
            UpdateProvider();
            UpdateField();
        }

        protected override void FieldChanged(IData newValue)
        {
            if (_forwardedProvider == null) throw new ArgumentNullException(nameof(_forwardedProvider));
            _forwardedProvider.DataReference.Ref = newValue;
        }
    }
}
