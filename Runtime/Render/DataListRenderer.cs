#nullable enable

using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Platonic.Render
{
    /// <summary>
    /// Simple Data list renderer with simple pooling support.
    /// Use as a reference implementation for more complex list renderers.
    /// </summary>
    public class DataListRenderer : FieldRenderer<IEnumerable<IData>>
    {
        [SerializeField] private DataProvider _prototype = null!;

        private readonly Dictionary<IData, (DataProvider Prototype, DataProvider Instance)> _instanceMap = new();
        private readonly Dictionary<DataProvider, List<DataProvider>> _pools = new();

        private ISelector<IData, DataProvider>? _selector;

        public DataProvider GetInstance(IData data)
        {
            UpdateField();
            return _instanceMap[data].Instance;
        }

        protected override void ProviderAwake()
        {
            _selector = GetComponent<ISelector<IData, DataProvider>>();
            _prototype.gameObject.SetActive(false);
        }

        protected override void FieldChanged(IEnumerable<IData> newValue)
        {
            var dataList = newValue?.ToList() ?? new List<IData>();
            var newDataSet = new HashSet<IData>(dataList);

            // Return instances for removed data to pool
            var removed = _instanceMap.Keys.Where(k => !newDataSet.Contains(k)).ToList();
            foreach (var key in removed)
            {
                var instance = _instanceMap[key];
                _instanceMap.Remove(key);
                instance.Instance.gameObject.SetActive(false);
                if (!_pools.TryGetValue(instance.Prototype, out var instances))
                {
                    instances = new List<DataProvider>();
                }
                _pools.TryAdd(instance.Prototype, new List<DataProvider>());
                instances.Add(instance.Instance);
            }

            // Reuse or create instances for each data item, maintaining order
            for (var i = 0; i < dataList.Count; i++)
            {
                var data = dataList[i];
                if (!_instanceMap.TryGetValue(data, out var instance))
                {
                    var prototype = _selector?.SelectFor(data) ?? _prototype;
                    instance.Prototype = prototype;
                    if (_pools.TryGetValue(prototype, out var pool) && pool.Count > 0)
                    {
                        instance.Instance = pool[^1];
                        pool.RemoveAt(pool.Count - 1);
                    }
                    else
                    {
                        instance.Instance = Instantiate(prototype, transform);
                    }

                    instance.Instance.Data = data;
                    instance.Instance.gameObject.SetActive(true);
                    _instanceMap[data] = instance;
                }

                instance.Instance.transform.SetSiblingIndex(i);
            }
        }
    }
}