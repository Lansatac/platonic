using Platonic.Version;

namespace Platonic.Core
{
    public interface IField: IVersioned
    {
        IFieldName Name { get; }
        object? Value { get; }
    }

    public interface IField<T> : IField
    {
        new FieldName<T> Name { get; }
        new T Value { get; }
    }
}