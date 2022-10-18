using System;
using System.Collections.Generic;

namespace Platonic.Core
{
    public class Data : IData
    {
        public Data(params IField[] fields) : this((IEnumerable<IField>)fields)
        {
        }

        public Data(IEnumerable<IField> fields)
        {
            foreach (var field in fields)
            {
                _fields.Add(field.Name, field);
            }
        }
        
        private readonly IDictionary<IFieldName, IField> _fields = new Dictionary<IFieldName, IField>();

        public IEnumerable<IField> Fields => _fields.Values;
        public IField GetField(IFieldName name)
        {
            if (!_fields.TryGetValue(name, out var field))
            {
                throw new Exception($"Data did not contain field with name {name.ID}:{name.Name}!");
            }

            return field;
        }

        public IField<T> GetField<T>(FieldName<T> name)
        {
            var field = GetField((IFieldName)name);
            return (IField<T>)field;
        }
    }
}