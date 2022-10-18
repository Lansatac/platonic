using System.Collections.Generic;

namespace Platonic.Core
{
    public interface IData
    {
        IEnumerable<IField> Fields { get; }

        IField GetField(IFieldName name);
        IField<T> GetField<T>(FieldName<T> name);
    }
}