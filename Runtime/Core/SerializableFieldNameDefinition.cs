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
            IData = 11,
            custom = 6
        }

        public string Name;
        public FieldType Type;
        public string CustomTypeName;
        public bool Enumerable;
        public bool Nullable;

        public override int GetHashCode()
        {
            unchecked
            {
                return (Name?.GetHashCode() ?? 0) * 37 + Type.GetHashCode() * 37 +
                       Enumerable.GetHashCode() * 37 + Nullable.GetHashCode() * 37 +
                       (Type == FieldType.custom ? (CustomTypeName?.GetHashCode() ?? 0) * 37 : 0);
            }
        }

        public bool Equals(SerializableFieldNameDefinition other)
        {
            return Name == other.Name && Type == other.Type && Enumerable == other.Enumerable &&
                   Nullable == other.Nullable &&
                   (Type != FieldType.custom || CustomTypeName == other.CustomTypeName);
        }

        public override bool Equals(object? obj)
        {
            return obj is SerializableFieldNameDefinition other && Equals(other);
        }
    }
}
