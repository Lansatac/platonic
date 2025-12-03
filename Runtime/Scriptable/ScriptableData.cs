#nullable enable
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Platonic.Core;
using UnityEngine;

namespace Platonic.Scriptable
{
    public abstract class ScriptableData : ScriptableObject, IData
    {
        private Data? _data;
        protected IData Data {
            get
            {
                if (_data == null)
                {
                    _data = new Data(GetFields());
                }

                return _data;
            }
        }

        public IEnumerable<IField> Fields => Data.Fields;
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