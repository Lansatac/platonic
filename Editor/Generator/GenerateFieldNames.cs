#nullable enable

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEditor;
using UnityEngine;

namespace Platonic.Editor.Generator
{
    public static class GenerateFieldNames
    {
        [InitializeOnLoadMethod]
        private static void CheckForGenerationUpdates()
        {
            var oldHash = EditorPrefs.GetString($"{Application.dataPath}.Platonic.GeneratorHash");
            var allFieldNames = GetAllFieldNames();
            var newHash = GetSourceFileHash(allFieldNames);
            if (oldHash != newHash)
            {
                GenCode(allFieldNames, newHash);
            }
        }

        private static string GetSourceFileHash(IEnumerable<FieldNames> names)
        {
            int hash = 0;
            unchecked
            {
                foreach (var fieldName in names)
                {
                    hash = hash * 37 + fieldName.GetSerializationHash();
                }
            }

            return hash.ToString();
        }

        public static List<FieldNames> GetAllFieldNames()
        {
            var list = new List<FieldNames>();
            var assetIds = AssetDatabase.FindAssets("t:FieldNames");

            foreach (var id in assetIds)
            {
                var assetPath = AssetDatabase.GUIDToAssetPath(id);

                var names = AssetDatabase.LoadAssetAtPath<FieldNames>(assetPath);
                list.Add(names);
            }

            return list;
        }

        [MenuItem("Platonic/Generate Fields Names")]
        public static void GenCode()
        {
            var fieldNames = GetAllFieldNames();
            GenCode(fieldNames, GetSourceFileHash(fieldNames));
        }

        public static void GenCode(IEnumerable<FieldNames> allNames, string hash)
        {
            foreach (var names in allNames)
            {
                var generatedFile =
                    new FileInfo($"{Path.Combine("Assets/" + names.OutputPath, names.Namespace)}.generated.cs");
                generatedFile.Directory!.Create();
                Debug.Log($"Generating {generatedFile.FullName}");

                using var writeStream = generatedFile.OpenWrite();
                writeStream.SetLength(0); //overwrite existing

                using var writer = new StreamWriter(writeStream);
                StringBuilder source = new StringBuilder(
                    "// File generated by Platonic, do not modify manually\n" +
                    "using Platonic.Core;\n" +
                    "using UnityEngine;\n");

                foreach (var additionalUsing in names.AdditionalUsings)
                {
                    source.AppendLine($"using {additionalUsing};");
                }
                
                source.Append("\n" +
                              $"namespace {names.Namespace}\n" +
                              "{\n" +
                              "#if UNITY_EDITOR\n" +
                              "\tusing UnityEditor;\n" +
                              "\t[InitializeOnLoad]\n" +
                              "#endif\n" +
                              "\tpublic static class Names\n" +
                              "\t{\n" +
                              "\t\t[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]\n" +
                              "\t\tprivate static void Init(){}" +
                              "\n\n");
                
                HashSet<string> usedNames = new();
                foreach (var name in names.Names)
                {
                    if (!usedNames.Contains(name.Name))
                    {
                        source.AppendLine(
                            $"\t\tpublic static FieldName<{name.Type}> {name.Name} = Platonic.Core.Names.Register<{name.Type}>(\"{names.Namespace}.\" + nameof({name.Name}));");
                        usedNames.Add(name.Name);
                    }
                    else
                    {
                        Debug.LogWarning($"Duplicate name {name.Name}!");
                    }
                }

                source.Append("\t}\n" +
                              "}\n");
                writer.Write(source);
            }

            EditorPrefs.SetString($"{Application.dataPath}.Platonic.GeneratorHash", hash);

            AssetDatabase.Refresh();
            AssetDatabase.SaveAssets();
        }
    }
}