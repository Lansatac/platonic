using System.Collections.Generic;
using NUnit.Framework;
using Platonic.Collections;

namespace Platonic.Editor.Tests.Collections
{
    public class VersionedDictionaryTests
    {
        [Test]
        public void Add_KeyVal_IncrementsVersion()
        {
            var dict = new VersionedDictionary<int, string>();
            ulong initialVersion = dict.Version;

            dict.Add(1, "one");

            Assert.That(dict.Count, Is.EqualTo(1));
            Assert.That(dict.Version, Is.Not.EqualTo(initialVersion));
            Assert.That(dict[1], Is.EqualTo("one"));
        }

        [Test]
        public void Add_KeyValuePair_IncrementsVersion()
        {
            var dict = new VersionedDictionary<int, string>();
            ulong initialVersion = dict.Version;

            dict.Add(new KeyValuePair<int, string>(1, "one"));

            Assert.That(dict.Count, Is.EqualTo(1));
            Assert.That(dict.Version, Is.Not.EqualTo(initialVersion));
            Assert.That(dict[1], Is.EqualTo("one"));
        }

        [Test]
        public void Remove_ExistingKey_IncrementsVersion()
        {
            var dict = new VersionedDictionary<int, string> { { 1, "one" }, { 2, "two" } };
            ulong initialVersion = dict.Version;

            bool removed = dict.Remove(1);

            Assert.That(removed, Is.True);
            Assert.That(dict.Count, Is.EqualTo(1));
            Assert.That(dict.Version, Is.Not.EqualTo(initialVersion));
        }

        [Test]
        public void Remove_NonExistingKey_DoesNotIncrementVersion()
        {
            var dict = new VersionedDictionary<int, string> { { 1, "one" } };
            ulong initialVersion = dict.Version;

            bool removed = dict.Remove(99);

            Assert.That(removed, Is.False);
            Assert.That(dict.Count, Is.EqualTo(1));
            Assert.That(dict.Version, Is.EqualTo(initialVersion));
        }

        [Test]
        public void Remove_ExistingKeyValuePair_IncrementsVersion()
        {
            var dict = new VersionedDictionary<int, string> { { 1, "one" } };
            ulong initialVersion = dict.Version;

            bool removed = dict.Remove(new KeyValuePair<int, string>(1, "one"));

            Assert.That(removed, Is.True);
            Assert.That(dict.Count, Is.EqualTo(0));
            Assert.That(dict.Version, Is.Not.EqualTo(initialVersion));
        }
        
        [Test]
        public void Remove_NonExistingKeyValuePair_DoesNotIncrementVersion()
        {
            var dict = new VersionedDictionary<int, string> { { 1, "one" } };
            ulong initialVersion = dict.Version;

            bool removed = dict.Remove(new KeyValuePair<int, string>(1, "wrong"));

            Assert.That(removed, Is.False);
            Assert.That(dict.Count, Is.EqualTo(1));
            Assert.That(dict.Version, Is.EqualTo(initialVersion));
        }

        [Test]
        public void Clear_IncrementsVersion()
        {
            var dict = new VersionedDictionary<int, string> { { 1, "one" } };
            ulong initialVersion = dict.Version;

            dict.Clear();

            Assert.That(dict.Count, Is.EqualTo(0));
            Assert.That(dict.Version, Is.Not.EqualTo(initialVersion));
        }

        [Test]
        public void Indexer_Set_Existing_IncrementsVersion()
        {
            var dict = new VersionedDictionary<int, string> { { 1, "one" } };
            ulong initialVersion = dict.Version;

            dict[1] = "updated";

            Assert.That(dict[1], Is.EqualTo("updated"));
            Assert.That(dict.Version, Is.Not.EqualTo(initialVersion));
        }
        
        [Test]
        public void Indexer_Set_New_IncrementsVersion()
        {
            var dict = new VersionedDictionary<int, string>();
            ulong initialVersion = dict.Version;

            dict[1] = "new";

            Assert.That(dict[1], Is.EqualTo("new"));
            Assert.That(dict.Version, Is.Not.EqualTo(initialVersion));
        }

        [Test]
        public void Indexer_Get_DoesNotIncrementVersion()
        {
             var dict = new VersionedDictionary<int, string> { { 1, "one" } };
             ulong initialVersion = dict.Version;
             
             var val = dict[1];
             
             Assert.That(val, Is.EqualTo("one"));
             Assert.That(dict.Version, Is.EqualTo(initialVersion));
        }
        
        [Test]
        public void ContainsKey_ReturnsCorrectValue()
        {
            var dict = new VersionedDictionary<int, string> { { 1, "one" } };
            
            Assert.That(dict.ContainsKey(1), Is.True);
            Assert.That(dict.ContainsKey(2), Is.False);
        }
        
        [Test]
        public void TryGetValue_ReturnsCorrectValue()
        {
             var dict = new VersionedDictionary<int, string> { { 1, "one" } };
             
             bool found = dict.TryGetValue(1, out var val);
             
             Assert.That(found, Is.True);
             Assert.That(val, Is.EqualTo("one"));
             
             found = dict.TryGetValue(2, out val);
             Assert.That(found, Is.False);
        }

        [Test]
        public void Keys_And_Values_Access_Works()
        {
            var dict = new VersionedDictionary<int, string> { { 1, "one" }, { 2, "two" } };
            
            Assert.That(dict.Keys, Contains.Item(1));
            Assert.That(dict.Keys, Contains.Item(2));
            Assert.That(dict.Values, Contains.Item("one"));
            Assert.That(dict.Values, Contains.Item("two"));
        }
    }
}
