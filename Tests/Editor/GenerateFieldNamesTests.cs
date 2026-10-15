#nullable enable

using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using Platonic.Core;
using Platonic.Editor.Generator;
using UnityEditor;
using UnityEngine;

namespace Platonic.Editor.Tests
{
    public class GenerateFieldNamesTests
    {
        [Test]
        public void FilterAffectedFieldNames_FiltersOnlyFieldNamesAssets()
        {
            var mockFieldNames = ScriptableObject.CreateInstance<FieldNames>();
            mockFieldNames.name = "TestFields";
            mockFieldNames.Namespace = "TestNamespace";
            mockFieldNames.ClassName = "TestNames";

            var lookup = new Dictionary<string, FieldNames?>
            {
                { "Assets/Data/Field Names/TestFields.asset", mockFieldNames },
                { "Assets/Data/Other.asset", null },
                { "Assets/Scripts/SomeScript.cs", null },
                { "Assets/Textures/Image.png", null },
            };

            var imported = new[]
            {
                "Assets/Data/Field Names/TestFields.asset",
                "Assets/Scripts/SomeScript.cs",
                "Assets/Textures/Image.png"
            };
            var moved = new[]
            {
                "Assets/Data/Other.asset"
            };

            var result = GenerateFieldNames.FilterAffectedFieldNames(
                imported,
                moved,
                path => lookup.TryGetValue(path, out var fn) ? fn : null
            );

            Assert.That(result, Has.Count.EqualTo(1));
            Assert.That(result.First(), Is.SameAs(mockFieldNames));

            Object.DestroyImmediate(mockFieldNames);
        }

        [Test]
        public void FilterAffectedFieldNames_IgnoresEmptyOrNullPaths()
        {
            var result = GenerateFieldNames.FilterAffectedFieldNames(
                new[] { "", "Assets/NotAnAsset.txt" },
                System.Array.Empty<string>(),
                _ => null
            );

            Assert.That(result, Is.Empty);
        }

        [Test]
        public void GenerateSource_ProducesExpectedCode()
        {
            var fieldNames = ScriptableObject.CreateInstance<FieldNames>();
            fieldNames.name = "PlayerFields";
            fieldNames.Namespace = "Farhome.Data";
            fieldNames.ClassName = "PlayerNames";
            fieldNames.AdditionalUsings = new List<string> { "System" };
            fieldNames.Names = new List<SerializableFieldNameDefinition>
            {
                new()
                {
                    Name = "Health",
                    Type = SerializableFieldNameDefinition.FieldType.@int,
                    Nullable = false,
                    Enumerable = false
                },
                new()
                {
                    Name = "Tags",
                    Type = SerializableFieldNameDefinition.FieldType.@string,
                    Nullable = false,
                    Enumerable = true
                }
            };

            var source = GenerateFieldNames.GenerateSource(fieldNames);

            Assert.That(source, Does.Contain("namespace Farhome.Data"));
            Assert.That(source, Does.Contain("public static class PlayerNames"));
            Assert.That(source, Does.Contain("public static readonly FieldName<int> Health = Platonic.Core.Names.Register<int>(\"Farhome.Data.\" + nameof(Health));"));
            Assert.That(source, Does.Contain("public static readonly FieldName<IEnumerable<string>> Tags = Platonic.Core.Names.Register<IEnumerable<string>>(\"Farhome.Data.\" + nameof(Tags));"));
            Assert.That(source, Does.Contain("public static int Get_Health(this IData data) {return data.GetField(PlayerNames.Health).Value;}"));
            Assert.That(source, Does.Contain("public static IEnumerable<string> Get_Tags(this IData data) {return data.GetField(PlayerNames.Tags).Value;}"));

            Object.DestroyImmediate(fieldNames);
        }
    }
}
