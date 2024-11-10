#nullable enable
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
            ProviderAwake();
            UpdateProvider();
        }

        private void OnEnable()
        {
            _provider = LocateProvider();
            UpdateProvider();
        }

        protected virtual DataProvider LocateProvider()
        {
            return GetComponentInParent<DataProvider>();
        }

        protected virtual void ProviderAwake() { }

        protected void LateUpdate()
        {
            UpdateProvider();

            ProviderLateUpdate();
        }

        protected void UpdateProvider()
        {
            if (_provider == null) return;

            if (_cachedProviderVersion != _provider.Data.Version)
            {
                _cachedProviderVersion = _provider.Data.Version;
                OnDataChanged();
            }
        }

        protected virtual void ProviderLateUpdate() { }

        public IData? Data => _provider?.Data.Ref;

        protected abstract void OnDataChanged();
    }
}