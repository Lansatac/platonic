using System.Linq;
using NUnit.Framework;
using Platonic.Collections;
using Platonic.Linq;
using Platonic.Version;
using UnityEngine;

namespace Platonic.Editor.Tests.Linq
{
    public class VersionedEnumerableTests
    {
        private sealed class SortItem
        {
            public SortItem(string id, int plainKey, VersionedValue<int> versionedKey)
            {
                Id = id;
                PlainKey = plainKey;
                VersionedKey = versionedKey;
            }

            public string Id { get; }
            public int PlainKey { get; set; }
            public VersionedValue<int> VersionedKey { get; }
        }

        [Test]
        public void VersionedSelect_ProjectsValuesCorrectly()
        {
            var sourceData = new[] { 1, 2, 3 };
            var source = new VersionedList<int>(sourceData);

            var result = source.VersionedSelect(x => x * 2);

            Assert.That(result.ToArray(), Is.EqualTo(new[] { 2, 4, 6 }));
        }

        [Test]
        public void VersionedSelect_ReturnsVersionedEnumerable_ValueIsSelf()
        {
            var source = new VersionedList<int>(new[] { 1 });
            var result = source.VersionedSelect(x => x);

            Assert.That(result.Value, Is.SameAs(result));
        }

        [Test]
        public void VersionedWhere_FiltersValuesCorrectly()
        {
            var sourceData = new[] { 1, 2, 3, 4, 5 };
            var source = new VersionedList<int>(sourceData);

            var result = source.VersionedWhere(x => x % 2 == 0);

            Assert.That(result.ToArray(), Is.EqualTo(new[] { 2, 4 }));
        }

        [Test]
        public void VersionedWhere_ReflectsDynamicVersionChanges()
        {
            var source = new VersionedList<int>(Enumerable.Empty<int>());
            var result = source.VersionedWhere(x => true);

            Assert.That(result.Version, Is.EqualTo(1));
            source.Add(10);
            Assert.That(result.Version, Is.EqualTo(2));
        }

        [Test]
        public void VersionedWhere_ReturnsVersionedEnumerable_ValueIsSelf()
        {
            var source = new VersionedList<int>(new[] { 1 });
            var result = source.VersionedWhere(x => true);

            Assert.That(result.Value, Is.SameAs(result));
        }

        [Test]
        public void VersionedOrderBy_SortsByPlainKey()
        {
            var source = new VersionedList<SortItem>(new[]
            {
                new SortItem("c", 3, new VersionedValue<int>(30)),
                new SortItem("a", 1, new VersionedValue<int>(10)),
                new SortItem("b", 2, new VersionedValue<int>(20)),
            });

            var result = source.VersionedOrderBy(x => x.PlainKey);

            Assert.That(result.Select(x => x.Id).ToArray(), Is.EqualTo(new[] { "a", "b", "c" }));
        }

        [Test]
        public void VersionedOrderBy_VersionedKey_SortsByCurrentInnerValue()
        {
            var first = new SortItem("a", 1, new VersionedValue<int>(30));
            var second = new SortItem("b", 2, new VersionedValue<int>(10));
            var third = new SortItem("c", 3, new VersionedValue<int>(20));

            var source = new VersionedList<SortItem>(new[] { first, second, third });

            var result = source.VersionedOrderBy(x => x.VersionedKey);

            Assert.That(result.Select(x => x.Id).ToArray(), Is.EqualTo(new[] { "b", "c", "a" }));
        }

        [Test]
        public void VersionedOrderBy_VersionedKey_VersionChangesWhenInnerVersionChanges()
        {
            var item = new SortItem("a", 1, new VersionedValue<int>(10));
            var source = new VersionedList<SortItem>(new[] { item });

            var result = source.VersionedOrderBy(x => x.VersionedKey);

            var initialVersion = result.Version;

            item.VersionedKey.Value = 9;

            Assert.That(result.Version, Is.Not.EqualTo(initialVersion));
        }

        [Test]
        public void VersionedThenBy_VersionedKey_ReordersWhenSecondaryKeyChanges()
        {
            var first = new SortItem("a", 1, new VersionedValue<int>(2));
            var second = new SortItem("b", 1, new VersionedValue<int>(1));
            var source = new VersionedList<SortItem>(new[] { first, second });

            var result = source
                .VersionedOrderBy(x => x.PlainKey)
                .VersionedThenBy(x => x.VersionedKey);

            Assert.That(result.Select(x => x.Id).ToArray(), Is.EqualTo(new[] { "b", "a" }));

            first.VersionedKey.Value = 0;

            Assert.That(result.Select(x => x.Id).ToArray(), Is.EqualTo(new[] { "a", "b" }));
        }
        
        [Test]
        public void VersionedThenBy_VersionedKey_DoesNotReorderWhenSecondaryKeyDoesNotChangeOrder()
        {
            var first = new SortItem("a", 1, new VersionedValue<int>(2));
            var second = new SortItem("b", 1, new VersionedValue<int>(1));
            var source = new VersionedList<SortItem>(new[] { first, second });

            var result = source
                .VersionedOrderBy(x => x.PlainKey)
                .VersionedThenBy(x => x.VersionedKey);

            Assert.That(result.Select(x => x.Id).ToArray(), Is.EqualTo(new[] { "b", "a" }));

            first.VersionedKey.Value = 3;

            Assert.That(result.Select(x => x.Id).ToArray(), Is.EqualTo(new[] { "b", "a" }));
        }
        
        [Test]
        public void VersionedOrderBy_VersionedKey_VersionChangesWhenSortChanges()
        {
            var first = new SortItem("a", 1, new VersionedValue<int>(10));
            var second = new SortItem("b", 1, new VersionedValue<int>(20));
            var source = new VersionedList<SortItem>(new[] { first, second });

            var result = source.VersionedOrderBy(x => x.VersionedKey);

            var initialVersion = result.Version;

            Assert.That(result.Select(x => x.Id).ToArray(), Is.EqualTo(new[] { "a", "b" }));

            first.VersionedKey.Value = 21;

            Assert.That(result.Version, Is.Not.EqualTo(initialVersion));
            Assert.That(result.Select(x => x.Id).ToArray(), Is.EqualTo(new[] { "b", "a" }));

        }


        [Test]
        public void VersionedOrderByDescending_VersionedKey_SortsDescending()
        {
            var source = new VersionedList<SortItem>(new[]
            {
                new SortItem("a", 1, new VersionedValue<int>(10)),
                new SortItem("b", 2, new VersionedValue<int>(30)),
                new SortItem("c", 3, new VersionedValue<int>(20)),
            });

            var result = source.VersionedOrderByDescending(x => x.VersionedKey);

            Assert.That(result.Select(x => x.Id).ToArray(), Is.EqualTo(new[] { "b", "c", "a" }));
        }

        [Test]
        public void VersionedSelectMany_ProjectsValuesCorrectly()
        {
            var source = new VersionedList<int[]>(new[] { new[] { 1, 2 }, new[] { 3, 4 } });

            var result = source.VersionedSelectMany(x => x);

            Assert.That(result.ToArray(), Is.EqualTo(new[] { 1, 2, 3, 4 }));
        }

        [Test]
        public void VersionedSelectMany_ReflectsDynamicVersionChanges()
        {
            var source = new VersionedList<int[]>(new[] { new[] { 1 } });
            var result = source.VersionedSelectMany(x => x);

            Assert.That(result.Version, Is.EqualTo(1));
            source.Add(new[] { 2 });
            Assert.That(result.Version, Is.EqualTo(2));
            Assert.That(result.ToArray(), Is.EqualTo(new[] { 1, 2 }));
        }

        [Test]
        public void VersionedSelectMany_ReturnsVersionedEnumerable_ValueIsSelf()
        {
            var source = new VersionedList<int[]>(new[] { new[] { 1 } });
            var result = source.VersionedSelectMany(x => x);

            Assert.That(result.Value, Is.SameAs(result));
        }
    }
}