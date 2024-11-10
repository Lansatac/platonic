using Platonic.Version;

namespace Platonic.Core
{
    public interface IField: IVersioned
    {
        IFieldName Name { get; }
        object? Value { get; }
    }

    public interface IField<out T> : IField
    {
        new IFieldName<T> Name { get; }
        new T Value { get; }
    }
}