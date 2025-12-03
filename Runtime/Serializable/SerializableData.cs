#nullable enable
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Platonic.Core;

namespace Platonic.Serializable
{
    public abstract class SerializableData : IData
    {
        private Data? _data;

        private Data Data
        {
            get
            {
                _data ??= new Data(GetFields());
                return _data;
            }
        }

        public IEnumerable<IField> Fields => Data;

        protected abstract IEnumerable<IField> GetFields();
        
        public bool HasField(IFieldName fieldName)
        {
            return Data.HasField(fieldName);
        }

        public IField GetField(IFieldName fieldName)
        {
            return Data.GetField(fieldName);
        }

        public IField<T> GetField<T>(IFieldName<T> fieldName)
        {
            return Data.GetField(fieldName);
        }

        public bool TryGetField<T>(IFieldName<T> fieldName, [NotNullWhen(true)] out IField<T>? field)
        {
            return Data.TryGetField(fieldName, out field);
        }
    }
}