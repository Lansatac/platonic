using System;
using System.Collections.Generic;
using UnityEngine;

namespace Platonic
{
    [CreateAssetMenu(fileName = "FieldNames", menuName = "Data/Field Names")]
    public class FieldNames : ScriptableObject
    {
        [Serializable]
        public struct FieldName : IEquatable<FieldName>
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

            public bool Equals(FieldName other)
            {
                return Name == other.Name && Type == other.Type;
            }

            public override bool Equals(object obj)
            {
                return obj is FieldName other && Equals(other);
            }
        }

        public string OutputPath = "Generated/";

        public string Namespace = "Namespace";
 
        public List<FieldName> Names;

        public int GetSerializationHash()
        {
            unchecked
            {
                var hash = 0;

                hash = OutputPath.GetHashCode() * 37 + Namespace.GetHashCode();
                foreach (var fieldName in Names)
                {
                    hash = hash * 37 + fieldName.GetHashCode();
                }

                return hash;
            }
        }
    }
}