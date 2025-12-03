#nullable enable

using System;
using System.Collections;
using System.Collections.Generic;
using Platonic.Version;

namespace Platonic.Collections
{
    public class VersionedList<T> : IVersionedList<T>, IVersionedList, IVersionedReadOnlyList<T>
    {
        private readonly List<T> _internalList = new();

        public ulong Version { get; private set; } = Versions.Initial;
        public IVersionedEnumerable<T> Value => this;

        public IEnumerator<T> GetEnumerator() => _internalList.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public VersionedList()
        {
        }

        public VersionedList(IEnumerable<T> collection)
        {
            _internalList.AddRange(collection);
        }
        

        public void Add(T item)
        {
            _internalList.Add(item);
            ++Version;
        }

        public int Add(object? value)
        {
            var index = ((IList)_internalList).Add(value);
            ++Version;
            return index;
        }

        public bool Contains(object? value) => ((IList)_internalList).Contains(value);

        public int IndexOf(object value) => ((IList)_internalList).IndexOf(value);

        public void Insert(int index, object value)
        {
            ((IList)_internalList).Insert(index, value);
            ++Version;
        }

        public void Remove(object value)
        {
            var count = _internalList.Count;
            ((IList)_internalList).Remove(value);
            if (_internalList.Count != count)
            {
                ++Version;
            }
        }

        public void RemoveAt(int index)
        {
            _internalList.RemoveAt(index);
            ++Version;
        }

        public bool IsFixedSize => ((IList)_internalList).IsFixedSize;

        bool IList.IsReadOnly => ((IList)_internalList).IsReadOnly;

        object IList.this[int index]
        {
            get => _internalList[index]!;
            set
            {
                ((IList)_internalList)[index] = value;
                ++Version;
            }
        }

        public void Clear()
        {
            _internalList.Clear();
            ++Version;
        }

        public bool Contains(T item) => _internalList.Contains(item);

        public void CopyTo(T[] array, int arrayIndex) => _internalList.CopyTo(array, arrayIndex);

        public bool Remove(T item)
        {
            if (_internalList.Remove(item))
            {
                ++Version;
                return true;
            }

            return false;
        }
        
        public void Sort(IComparer<T> comparison)
        {
            _internalList.Sort(comparison);
            ++Version;
        }

        public void Sort(Comparison<T> comparison)
        {
            _internalList.Sort(comparison);
            ++Version;
        }
        
        public T? Find(Predicate<T> match) => _internalList.Find(match);
        public List<T> FindAll(Predicate<T> match) => _internalList.FindAll(match);

        public void CopyTo(Array array, int index) => ((ICollection)_internalList).CopyTo(array, index);

        public bool IsSynchronized => ((ICollection)_internalList).IsSynchronized;
        public object SyncRoot => ((ICollection)_internalList).SyncRoot;
        
        bool ICollection<T>.IsReadOnly => ((ICollection<T>)_internalList).IsReadOnly;

        public int IndexOf(T item) => _internalList.IndexOf(item);

        public void Insert(int index, T item)
        {
            _internalList.Insert(index, item);
            ++Version;
        }

        public T this[int index]
        {
            get => _internalList[index];
            set
            {
                _internalList[index] = value;
                ++Version;
            }
        }
        
        public int Count => _internalList.Count;
    }
}