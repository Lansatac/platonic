#nullable enable
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Platonic.Core;

namespace Platonic
{
    public interface IData : IEnumerable<IField>
    {
        IEnumerable<IField> Fields { get; }

        bool HasField(IFieldName fieldName);
        
        IField GetField(IFieldName fieldName);
        IField<T> GetField<T>(IFieldName<T> fieldName);
        
        bool TryGetField<T>(IFieldName<T> fieldName, [NotNullWhen(true)] out IField<T>? field);

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