#nullable enable

using System.Collections.Generic;
using Platonic.Core;
using UnityEngine;

namespace Platonic
{
    [CreateAssetMenu(fileName = "FieldNames", menuName = "Data/Field Names")]
    public class FieldNames : ScriptableObject
    {
        public string OutputPath = "Generated/";

        public string Namespace = "Namespace";

        public List<string> AdditionalUsings = null!;
        
        public List<SerializableFieldNameDefinition> Names = null!;

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