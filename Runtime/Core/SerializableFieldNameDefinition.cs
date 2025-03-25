using System;

namespace Platonic.Core
{
    [Serializable]
    public struct SerializableFieldNameDefinition : IEquatable<SerializableFieldNameDefinition>
    {
        public string Name;
        public string Type;

        public override int GetHashCode()
        {
            unchecked
            {
                return Name.GetHashCode() * 37 + Type.GetHashCode() * 37;
            }
        }

        public bool Equals(SerializableFieldNameDefinition other)
        {
            return Name == other.Name && Type == other.Type;
        }

        public override bool Equals(object obj)
        {
            return obj is SerializableFieldNameDefinition other && Equals(other);
        }
    }
}