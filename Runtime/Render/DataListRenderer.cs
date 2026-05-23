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

        private readonly List<DataProvider> _instances = new();
        private readonly List<DataProvider> _pool = new();

        protected override void ProviderAwake()
        {
            _prototype.gameObject.SetActive(false);
        }

        protected override void FieldChanged(IEnumerable<IData> newValue)
        {
            var dataList = newValue?.ToList() ?? new List<IData>();

            // Deactivate surplus instances and add them to the pool
            while (_instances.Count > dataList.Count)
            {
                var instance = _instances[^1];
                _instances.RemoveAt(_instances.Count - 1);
                instance.gameObject.SetActive(false);
                _pool.Add(instance);
            }

            // Reuse or create instances for the data
            for (var i = 0; i < dataList.Count; i++)
            {
                var data = dataList[i];
                DataProvider instance;
                if (i < _instances.Count)
                {
                    // Reuse existing instance
                    instance = _instances[i];
                    instance.Data = data;
                    instance.gameObject.SetActive(true);
                }
                else
                {
                    // Get from pool or create new
                    if (_pool.Count > 0)
                    {
                        instance = _pool[0];
                        _pool.RemoveAt(0);
                    }
                    else
                    {
                        instance = Instantiate(_prototype, transform);
                    }

                    instance.Data = data;
                    instance.gameObject.SetActive(true);
                    _instances.Add(instance);
                }

                // Ensure the hierarchy order matches the data order
                instance.transform.SetAsLastSibling();
            }
        }
    }
}