#nullable enable

using System;

namespace Platonic.Core
{
    [Serializable]
    public struct SerializableFieldNameDefinition : IEquatable<SerializableFieldNameDefinition>
    {
        public enum FieldType
        {
            @int = 0,
            @float = 1,
            @bool = 2,
            @string = 3,
            Vector2 = 4,
            Vector3 = 5,
            IEnumerable_int = 7,
            IEnumerable_float = 8,
            IEnumerable_bool = 9,
            IEnumerable_string = 10,
            IData = 11,
            IEnumerable_IData = 12,
            custom = 6
        }

        public string Name;
        public FieldType Type;
        public string CustomTypeName;

        public override int GetHashCode()
        {
            unchecked
            {
                return Name.GetHashCode() * 37 + Type.GetHashCode() * 37 +
                       (Type == FieldType.custom ? 0 : CustomTypeName.GetHashCode() * 37);
            }
        }

        public bool Equals(SerializableFieldNameDefinition other)
        {
            return Name == other.Name && Type == other.Type;
        }

        public override bool Equals(object? obj)
        {
            return obj is SerializableFieldNameDefinition other && Equals(other);
        }
    }
}