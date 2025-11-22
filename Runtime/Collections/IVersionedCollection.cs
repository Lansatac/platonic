using System.Collections;
using System.Collections.Generic;

namespace Platonic.Collections
{
    public interface IVersionedCollection : ICollection, IVersionedEnumerable
    {
        
    }
    
    public interface IVersionedReadOnlyCollection<out T> : IReadOnlyCollection<T>, IVersionedEnumerable<T>
    {
        
    }
    
    public interface IVersionedCollection<T> : ICollection<T>, IVersionedEnumerable<T>
    {
        
    }
}