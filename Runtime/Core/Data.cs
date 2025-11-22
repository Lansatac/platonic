#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;
using Platonic.Core;
using UnityEngine;

namespace Platonic
{
    public class Data : IData
    {
        
        public Data(IEnumerable<IField> fields, params IField[] paramFields) : this(fields.Concat(paramFields))
        {
        }
        public Data(params IField[] fields) : this((IEnumerable<IField>)fields)
        {
        }

        public Data(IEnumerable<IField> fields)
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
        
        private readonly IDictionary<ulong, IField> _fields = new Dictionary<ulong, IField>();

        public IEnumerable<IField> Fields => _fields.Values;
        public bool HasField(IFieldName fieldName)
        {
            return _fields.ContainsKey(fieldName.Id);
        }

        public IField GetField(IFieldName fieldName)
        {
            if (!_fields.TryGetValue(fieldName.Id, out var field))
            {
                throw new Exception($"Data did not contain field with name {fieldName.Id}:{fieldName.Name}!");
            }

            return field;
        }

        public IField<T> GetField<T>(IFieldName<T> fieldName)
        {
            var field = GetField((IFieldName)fieldName);
            return (IField<T>)field;
        }
    }
}