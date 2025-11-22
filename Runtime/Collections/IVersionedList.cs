using System.Collections;
using System.Collections.Generic;

namespace Platonic.Collections
{
    
    public interface IVersionedList : IList, IVersionedCollection
    {
        
    }
    
    public interface IVersionedReadOnlyList<out T> : IReadOnlyList<T>, IVersionedReadOnlyCollection<T>
    {
        
    }
    
    public interface IVersionedList<T> : IList<T>, IVersionedCollection<T>
    {
        
    }
}