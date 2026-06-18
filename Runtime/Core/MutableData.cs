#nullable enable
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Platonic.Core;
using UnityEngine;

namespace Platonic
{
    public class MutableData : IData, IEnumerable<IMutableField>
    {
        public MutableData(IEnumerable<IMutableField> fields, params IMutableField[] paramFields) : this(
            fields.Concat(paramFields))
        {
        }

        public MutableData(params IMutableField[] fields) : this((IEnumerable<IMutableField>)fields)
        {
        }

        public MutableData(IEnumerable<IMutableField> fields)
        {
            foreach (var field in fields)
            {
                try
                {
                    _fields.Add(field.Name.Id, field);
                }
                catch (ArgumentException)
                {
                    Debug.LogWarning($"Duplicate field '{field.Name.Name}' added!");
                }
                catch (Exception e)
                {
                    Debug.LogException(e);
                }
            }
        }

        private readonly IDictionary<ulong, IMutableField> _fields = new Dictionary<ulong, IMutableField>();

        public IEnumerable<IField> Fields => _fields.Values;

        public bool HasField(IFieldName fieldName)
        {
            return _fields.ContainsKey(fieldName.Id);
        }

        public IMutableField GetField(IFieldName fieldName)
        {
            if (!_fields.TryGetValue(fieldName.Id, out var field))
            {
                throw new Exception($"Data did not contain field with name {fieldName.Id}:{fieldName.Name}!");
            }

            return field;
        }

        IField IData.GetField(IFieldName fieldName)
        {
            return GetField(fieldName);
        }

        IField<T> IData.GetField<T>(IFieldName<T> fieldName)
        {
            return GetField(fieldName);
        }
        
        public MutableField<T> GetField<T>(IFieldName<T> fieldName)
        {
            var field = GetField((IFieldName)fieldName);
            return (MutableField<T>)field;
        }

        bool IData.TryGetField<T>(IFieldName<T> fieldName, [NotNullWhen(true)] out IField<T>? field)
        {
            var contains = TryGetField(fieldName, out var mutableField);
            field = mutableField;
            return contains;
        }
        
        public bool TryGetField<T>(IFieldName<T> fieldName, [NotNullWhen(true)] out MutableField<T>? field)
        {
            field = null;
            var has = _fields.TryGetValue(fieldName.Id, out var untypedField);
            if (has)
            {
                field = untypedField as MutableField<T>;
            }

            return has;
        }

        IEnumerator<IMutableField> IEnumerable<IMutableField>.GetEnumerator()
        {
            return _fields.Values.GetEnumerator();
        }

        public override string ToString()
        {
            return
                $"MutableData [Fields: {string.Join("\n", _fields.Values.Select(field => $"{field.Name.Name}={field.Value}"))}]";
        }
    }
}