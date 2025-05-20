#nullable enable
using System;
using UnityEngine;

namespace Platonic.Core
{
    public interface IFieldName
    {
        ulong ID { get; }
        string Name { get; }
        Type FieldType { get; }
    }
    
    public interface IFieldName<out T> : IFieldName
    {
        
    }
    
    [Serializable]
    public class FieldName<T> : IFieldName<T>
    {
        [SerializeField]
        private ulong _id;

        [SerializeField]
        private string _name;

        public FieldName(ulong id, string name)
        {
            _id = id;
            _name = name;
        }

        public ulong ID => _id;

        public string Name => _name;

        public Type FieldType => typeof(T);

        public override string ToString()
        {
            return _name;
        }
    }
}