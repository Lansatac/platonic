#nullable enable
using Platonic.Core;
using Platonic.Version;
using UnityEngine;

namespace Platonic.Render
{
    public abstract class ProviderRenderer : MonoBehaviour
    {
        private DataProvider? _provider;
        protected DataProvider? Provider => _provider;
        
        private ulong _cachedProviderVersion = Versions.None;

        protected void Awake()
        {
            ProviderAwake();
            UpdateProvider();
        }

        protected void OnEnable()
        {
            _provider = LocateProvider();
            UpdateProvider();
            ProviderLateUpdate();
            ProviderOnEnable();
        }

        protected virtual DataProvider LocateProvider()
        {
            return GetComponentInParent<DataProvider>();
        }

        protected virtual void ProviderAwake() { }
        protected virtual void ProviderOnEnable() { }
        protected void LateUpdate()
        {
            UpdateProvider();

            ProviderLateUpdate();
        }

        protected void UpdateProvider()
        {
            if (_provider == null) return;

            if (_cachedProviderVersion != _provider.DataReference.Version)
            {
                _cachedProviderVersion = _provider.DataReference.Version;
                OnDataChanged();
            }
        }

        protected virtual void ProviderLateUpdate() { }

        public IData? Data => _provider?.Data;

        protected abstract void OnDataChanged();
    }
}