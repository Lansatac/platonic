#nullable enable
using Platonic.Core;
using Platonic.Version;

namespace Platonic
{
    public interface IField: IVersioned
    {
        IFieldName Name { get; }
        object? Value { get; }
    }

    public interface IMutableField : IField
    {
        new object? Value { get; set; }
    }

    public interface IField<out T> : IField, IVersionedValue<T>
    {
        new IFieldName<T> Name { get; }
        new T Value { get; }
    }
    
    public interface IMutableField<T> : IField<T>, IMutableField
    {
        new T Value { get; set; }
    }
}