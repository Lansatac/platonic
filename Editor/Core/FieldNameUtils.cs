#nullable enable
using System;
using System.Linq;
using Platonic.Core;
using UnityEditor;
using UnityEngine;

namespace Platonic.Editor.Core
{
    public static class FieldNameUtils
    {
        /// <summary>
        /// Adds a new field definition to the FieldNames asset with the given namespace.
        /// type should be a FieldType enum name ("int", "float", "bool", "string", "Vector2", "Vector3", "IData")
        /// or a fully-qualified custom type name. Triggers regeneration.
        /// </summary>
        public static void AddNameToNamespace(string @namespace, string name, string type, bool enumerable = false, bool nullable = false)
        {
            var asset = FindAssetByNamespace(@namespace);
            if (asset == null)
                throw new Exception($"No FieldNames asset found with namespace '{@namespace}'. Use CreateNamespace to create one first.");

            if (asset.Names.Any(n => n.Name == name))
                throw new Exception($"Field '{name}' already exists in namespace '{@namespace}'.");

            var definition = new SerializableFieldNameDefinition
            {
                Name = name,
                Type = ParseFieldType(type),
                CustomTypeName = IsBuiltinType(type) ? "" : type,
                Enumerable = enumerable,
                Nullable = nullable,
            };

            asset.Names.Add(definition);
            EditorUtility.SetDirty(asset);
            AssetDatabase.SaveAssets();
        }

        /// <summary>
        /// Creates a new FieldNames asset at Assets/Data/Field Names/{namespace}.asset
        /// with the given namespace and class name "Names".
        /// </summary>
        public static FieldNames CreateNamespace(string @namespace, string outputPath = "Generated/", string className = "Names")
        {
            if (FindAssetByNamespace(@namespace) != null)
                throw new Exception($"A FieldNames asset with namespace '{@namespace}' already exists.");

            var folderPath = "Assets/Data/Field Names";
            if (!AssetDatabase.IsValidFolder(folderPath))
            {
                var parts = folderPath.Split('/');
                var current = parts[0];
                for (int i = 1; i < parts.Length; i++)
                {
                    var next = current + "/" + parts[i];
                    if (!AssetDatabase.IsValidFolder(next))
                        AssetDatabase.CreateFolder(current, parts[i]);
                    current = next;
                }
            }

            var asset = ScriptableObject.CreateInstance<FieldNames>();
            asset.Namespace = @namespace;
            asset.ClassName = className;
            asset.OutputPath = outputPath;
            asset.Names = new System.Collections.Generic.List<SerializableFieldNameDefinition>();
            asset.AdditionalUsings = new System.Collections.Generic.List<string>();

            var assetPath = $"{folderPath}/{@namespace}.asset";
            AssetDatabase.CreateAsset(asset, assetPath);
            AssetDatabase.SaveAssets();
            return asset;
        }

        /// <summary>
        /// Adds a using directive to the FieldNames asset with the given namespace.
        /// </summary>
        public static void AddUsingToNamespace(string @namespace, string usingDirective)
        {
            var asset = FindAssetByNamespace(@namespace);
            if (asset == null)
                throw new Exception($"No FieldNames asset found with namespace '{@namespace}'.");

            if (asset.AdditionalUsings.Contains(usingDirective))
                return;

            asset.AdditionalUsings.Add(usingDirective);
            EditorUtility.SetDirty(asset);
            AssetDatabase.SaveAssets();
        }

        public static FieldNames? FindAssetByNamespace(string @namespace)
        {
            var guids = AssetDatabase.FindAssets("t:FieldNames");
            foreach (var guid in guids)
            {
                var path = AssetDatabase.GUIDToAssetPath(guid);
                var asset = AssetDatabase.LoadAssetAtPath<FieldNames>(path);
                if (asset != null && asset.Namespace == @namespace)
                    return asset;
            }
            return null;
        }

        private static SerializableFieldNameDefinition.FieldType ParseFieldType(string type)
        {
            return type switch
            {
                "int" => SerializableFieldNameDefinition.FieldType.@int,
                "float" => SerializableFieldNameDefinition.FieldType.@float,
                "bool" => SerializableFieldNameDefinition.FieldType.@bool,
                "string" => SerializableFieldNameDefinition.FieldType.@string,
                "Vector2" => SerializableFieldNameDefinition.FieldType.Vector2,
                "Vector3" => SerializableFieldNameDefinition.FieldType.Vector3,
                "IData" => SerializableFieldNameDefinition.FieldType.IData,
                _ => SerializableFieldNameDefinition.FieldType.custom,
            };
        }

        private static bool IsBuiltinType(string type) =>
            type is "int" or "float" or "bool" or "string" or "Vector2" or "Vector3" or "IData";
    }
}
