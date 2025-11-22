using System.Collections;
using System.Collections.Generic;
using Platonic.Version;

namespace Platonic.Collections
{
    public interface IVersionedEnumerable : IEnumerable, IVersioned
    {
        
    }

    public interface IVersionedEnumerable<out T> : IVersionedEnumerable, IEnumerable<T>, IVersionedValue<IVersionedEnumerable<T>>
    {
        
    }
}