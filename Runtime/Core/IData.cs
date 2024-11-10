#nullable enable
using System.Collections;
using System.Collections.Generic;

namespace Platonic.Core
{
    public interface IData : IEnumerable<IField>
    {
        IEnumerable<IField> Fields { get; }

        bool HasField(IFieldName fieldName);
        
        IField GetField(IFieldName fieldName);
        IField<T> GetField<T>(IFieldName<T> fieldName);

        IEnumerator<IField> IEnumerable<IField>.GetEnumerator()
        {
            return Fields.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}