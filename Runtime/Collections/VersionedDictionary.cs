using System.Collections;
using System.Collections.Generic;
using Platonic.Version;

namespace Platonic.Collections
{
    public class VersionedDictionary<TKey, TValue> : IVersionedDictionary<TKey, TValue>
    {
        private readonly Dictionary<TKey, TValue> _internalDictionary = new();

        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
        {
            return _internalDictionary.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public void Add(KeyValuePair<TKey, TValue> item)
        {
            _internalDictionary.Add(item.Key, item.Value);
            ++Version;
        }

        public void Clear()
        {
            _internalDictionary.Clear();
            ++Version;
        }

        public bool Contains(KeyValuePair<TKey, TValue> item)
        {
            return ((ICollection<KeyValuePair<TKey, TValue>>)_internalDictionary).Contains(item);
        }

        public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
        {
            ((ICollection<KeyValuePair<TKey, TValue>>)_internalDictionary).CopyTo(array, arrayIndex);
        }

        public bool Remove(KeyValuePair<TKey, TValue> item)
        {
            if (((ICollection<KeyValuePair<TKey, TValue>>)_internalDictionary).Remove(item))
            {
                ++Version;
                return true;
            }

            return false;
        }

        public int Count => _internalDictionary.Count;
        public bool IsReadOnly => ((ICollection<KeyValuePair<TKey, TValue>>)_internalDictionary).IsReadOnly;

        public void Add(TKey key, TValue value)
        {
            _internalDictionary.Add(key, value);
            ++Version;
        }

        public bool ContainsKey(TKey key)
        {
            return _internalDictionary.ContainsKey(key);
        }

        public bool Remove(TKey key)
        {
            if (_internalDictionary.Remove(key))
            {
                ++Version;
                return true;
            }

            return false;
        }

        public bool TryGetValue(TKey key, out TValue value)
        {
            return _internalDictionary.TryGetValue(key, out value);
        }

        public TValue this[TKey key]
        {
            get => _internalDictionary[key];
            set
            {
                _internalDictionary[key] = value;
                ++Version;
            }
        }

        public ICollection<TKey> Keys => _internalDictionary.Keys;
        public ICollection<TValue> Values => _internalDictionary.Values;
        public ulong Version { get; private set; } = Versions.Initial;
        public IVersionedEnumerable<KeyValuePair<TKey, TValue>> Value => this;
    }
}