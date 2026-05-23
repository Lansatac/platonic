#nullable enable
namespace Platonic.Core
{
    public static partial class FieldExtensions
    {
        public static MutableField<T> Of<T>(this FieldName<T> name, T value)
        {
            return new MutableField<T>(name, value);
        }

    }
}