#nullable enable
using Platonic.Core;
using Platonic.Version;
using UnityEngine;
using UnityEngine.Profiling;

namespace Platonic.Render
{
    public abstract class ProviderRenderer : MonoBehaviour
    {
        protected readonly string TypeName;
        private readonly string _providerSample;
        
        private DataProvider? _provider;
        protected DataProvider? Provider => _provider;
        
        private ulong _cachedProviderVersion = Versions.None;

        protected ProviderRenderer()
        {
            TypeName = GetType().Name;
            _providerSample = $"{TypeName} OnDataChanged";
        }

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
                Profiler.BeginSample(_providerSample, this);
                OnDataChanged();
                Profiler.EndSample();
            }
        }

        protected virtual void ProviderLateUpdate() { }

        public IData? Data => _provider?.Data;

        protected abstract void OnDataChanged();
    }
}