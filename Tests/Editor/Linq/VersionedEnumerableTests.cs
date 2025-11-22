using System.Collections;
 using System.Collections.Generic;
 using System.Linq;
 using NUnit.Framework;
 using Platonic.Collections;
 using Platonic.Linq;
 using Platonic.Version;

 namespace Platonic.Editor.Tests.Linq
 {
     public class VersionedEnumerableTests
     {
         private class TestVersionedEnumerable<T> : IVersionedEnumerable<T>
         {
             private readonly IEnumerable<T> _source;
 
             public TestVersionedEnumerable(IEnumerable<T> source, ulong version = Versions.Initial)
             {
                 _source = source;
                 Version = version;
             }
 
             public IEnumerator<T> GetEnumerator() => _source.GetEnumerator();
 
             IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
 
             public ulong Version { get; set; }
             public IVersionedEnumerable<T> Value => this;
         }
 
         [Test]
         public void VersionedSelect_ProjectsValuesCorrectly()
         {
             var sourceData = new[] { 1, 2, 3 };
             var source = new TestVersionedEnumerable<int>(sourceData);
 
             var result = source.VersionedSelect(x => x * 2);
 
             Assert.That(result.ToArray(), Is.EqualTo(new[] { 2, 4, 6 }));
         }
 
         [Test]
         public void VersionedSelect_PropagatesSourceVersion()
         {
             var source = new TestVersionedEnumerable<int>(Enumerable.Empty<int>(), version: 42);
 
             var result = source.VersionedSelect(x => x);
 
             Assert.That(result.Version, Is.EqualTo(42));
         }
 
         [Test]
         public void VersionedSelect_ReflectsDynamicVersionChanges()
         {
             var source = new TestVersionedEnumerable<int>(Enumerable.Empty<int>(), version: 10);
             var result = source.VersionedSelect(x => x);
 
             Assert.That(result.Version, Is.EqualTo(10));
 
             source.Version = 20;
             Assert.That(result.Version, Is.EqualTo(20));
         }
 
         [Test]
         public void VersionedSelect_ReturnsVersionedEnumerable_ValueIsSelf()
         {
             var source = new TestVersionedEnumerable<int>(new[] { 1 });
             var result = source.VersionedSelect(x => x);
 
             Assert.That(result.Value, Is.SameAs(result));
         }

             [Test]
             public void VersionedWhere_FiltersValuesCorrectly()
             {
                 var sourceData = new[] { 1, 2, 3, 4, 5 };
                 var source = new TestVersionedEnumerable<int>(sourceData);

                 var result = source.VersionedWhere(x => x % 2 == 0);

                 Assert.That(result.ToArray(), Is.EqualTo(new[] { 2, 4 }));
             }

             [Test]
             public void VersionedWhere_PropagatesSourceVersion()
             {
                 var source = new TestVersionedEnumerable<int>(Enumerable.Empty<int>(), version: 42);

                 var result = source.VersionedWhere(x => true);

                 Assert.That(result.Version, Is.EqualTo(42));
             }

             [Test]
             public void VersionedWhere_ReflectsDynamicVersionChanges()
             {
                 var source = new TestVersionedEnumerable<int>(Enumerable.Empty<int>(), version: 10);
                 var result = source.VersionedWhere(x => true);

                 Assert.That(result.Version, Is.EqualTo(10));

                 source.Version = 20;
                 Assert.That(result.Version, Is.EqualTo(20));
             }

             [Test]
             public void VersionedWhere_ReturnsVersionedEnumerable_ValueIsSelf()
             {
                 var source = new TestVersionedEnumerable<int>(new[] { 1 });
                 var result = source.VersionedWhere(x => true);

                 Assert.That(result.Value, Is.SameAs(result));
             }
         }
     }