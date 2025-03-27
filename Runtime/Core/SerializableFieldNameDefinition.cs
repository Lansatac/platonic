#nullable enable

using System;

namespace Platonic.Core
{
    [Serializable]
    public struct SerializableFieldNameDefinition : IEquatable<SerializableFieldNameDefinition>
    {
        public enum FieldType {@int, @float, @bool, @string, Vector2, Vector3, custom}

        public string Name;
        public FieldType Type;
        public string CustomTypeName;

        public override int GetHashCode()
        {
            unchecked
            {
                return Name.GetHashCode() * 37 + Type.GetHashCode() * 37 + (Type == FieldType.custom ? 0 : CustomTypeName.GetHashCode() * 37);
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