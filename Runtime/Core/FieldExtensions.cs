#nullable enable
namespace Platonic.Core
{
    public static partial class FieldExtensions
    {
        public static Field<T> Of<T>(this FieldName<T> name, T value)
        {
            return new Field<T>(name, value);
        }

    }
}