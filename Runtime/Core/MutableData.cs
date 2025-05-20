#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;
using Platonic.Core;
using UnityEngine;

namespace Platonic
{
    public class MutableData : IData
    {
        public MutableData(IEnumerable<IField> fields, params IField[] paramFields) : this(fields.Concat(paramFields))
        {
        }

        public MutableData(params IField[] fields) : this((IEnumerable<IField>)fields)
        {
        }

        public MutableData(IEnumerable<IField> fields)
        {
            foreach (var field in fields)
            {
                try
                {
                    _fields.Add(field.Name.ID, field);
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

        private readonly IDictionary<ulong, IField> _fields = new Dictionary<ulong, IField>();

        public IEnumerable<IField> Fields => _fields.Values;

        public bool HasField(IFieldName fieldName)
        {
            return _fields.ContainsKey(fieldName.ID);
        }

        public IField GetField(IFieldName fieldName)
        {
            if (!_fields.TryGetValue(fieldName.ID, out var field))
            {
                throw new Exception($"Data did not contain field with name {fieldName.ID}:{fieldName.Name}!");
            }

            return field;
        }

        IField<T> IData.GetField<T>(IFieldName<T> fieldName)
        {
            return GetField(fieldName);
        }

        public Field<T> GetField<T>(IFieldName<T> fieldName)
        {
            var field = GetField((IFieldName)fieldName);
            return (Field<T>)field;
        }
        
        public bool TryGetField<T>(ulong fieldNameId, out Field<T>? field)
        {
            if (!_fields.TryGetValue(fieldNameId, out var iField))
            {
                field = null;
                return false;
            }
            
            field = (Field<T>)iField;
            return field != null;
        }

        public override string ToString()
        {
            return
                $"MutableData [Fields: {string.Join("\n", _fields.Values.Select(field => $"{field.Name.Name}={field.Value}"))}]";
        }
    }
}