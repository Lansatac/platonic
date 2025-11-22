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
        IVersionedReadOnlyCollection<KeyValuePair<TKey, TValue>>
    {
        
    }
    
    public interface IVersionedDictionary<TKey, TValue> :
        IDictionary<TKey, TValue>,
        IVersionedCollection<KeyValuePair<TKey, TValue>>
    {
        
    }
}