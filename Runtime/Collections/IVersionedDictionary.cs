using System.Collections;
using System.Collections.Generic;
using Platonic.Version;

namespace Platonic.Collections
{
    public interface IVersionedDictionary :
        IDictionary,
        IVersioned
    {
        
    }

    public interface IVersionedReadOnlyDictionary<TKey, TValue> :
        IReadOnlyDictionary<TKey, TValue>,
        IVersionedReadOnlyCollection<KeyValuePair<TKey, TValue>>,
        IVersionedValue<IVersionedReadOnlyDictionary<TKey, TValue>>
    {
        new int Count { get; }
        new IVersionedReadOnlyDictionary<TKey, TValue> Value { get; }

        IVersionedValue<TValue?> GetValue(TKey key);

    }
    
    public interface IVersionedDictionary<TKey, TValue> :
        IDictionary<TKey, TValue>,
        IVersionedCollection<KeyValuePair<TKey, TValue>>,
        IVersionedReadOnlyDictionary<TKey, TValue>
    {
        new int Count { get; }
        new IVersionedDictionary<TKey, TValue> Value { get; }
    }
}