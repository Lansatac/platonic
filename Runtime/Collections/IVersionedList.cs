using System.Collections;
using System.Collections.Generic;
using Platonic.Version;

namespace Platonic.Collections
{
    
    public interface IVersionedList : IList, IVersionedCollection
    {
        
    }
    
    public interface IVersionedReadOnlyList<out T> : IReadOnlyList<T>, IVersionedReadOnlyCollection<T>,
        IVersionedValue<IVersionedReadOnlyList<T>>
    {
        
    }
    
    public interface IVersionedList<T> : IList<T>, IVersionedCollection<T>
    {
        
    }
}