using System;

namespace Platonic.Core
{
    public interface IFieldName
    {
        ulong ID { get; }
        string Name { get; }
        Type FieldType { get; }
    }
    
    public struct FieldName<T> : IFieldName
    {
        public FieldName(ulong id, string name)
        {
            ID = id;
            Name = name;
        }

        public ulong ID { get; }

        public string Name { get; }

        public Type FieldType => typeof(T);
    }
}