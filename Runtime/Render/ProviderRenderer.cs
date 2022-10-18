using System;
using Platonic.Core;
using Platonic.Version;
using UnityEngine;

namespace Platonic.Render
{
    public abstract class ProviderRenderer : MonoBehaviour
    {
        
        private DataProvider? _provider;
        private ulong _cachedProviderVersion = Versions.None;

        protected void Awake()
        {
            _provider = GetComponentInParent<DataProvider>();
            ProviderAwake();
            OnDataChanged();
        }

        protected virtual void ProviderAwake() { }

        protected void Update()
        {
            if (_provider == null) return;

            if (_cachedProviderVersion != _provider.Data.Version)
            {
                _cachedProviderVersion = _provider.Data.Version;
                OnDataChanged();
            }
            
            ProviderUpdate();
        }

        protected virtual void ProviderUpdate() { }

        public IData? Data {
            get
            {

                return _provider?.Data.Ref;
            }
        }

        protected abstract void OnDataChanged();
    }
}