#nullable enable
using System;
using Platonic.Core;

namespace Platonic.Scriptable
{
    [Serializable]
    public class SerializableFieldName
    {
        public ulong ID;

        public IFieldName AsName()
        {
            return Names.Instance.GetName(ID);
        }
    }
    
    [Serializable]
    public class SerializableFieldName<T>
    {
        public ulong ID;

        public IFieldName<T> AsName()
        {
            return Names.Instance.GetName<T>(ID);
        }
    }
}