using System.Collections;
using NUnit.Framework;
using Platonic.Collections;

namespace Platonic.Editor.Tests.Collections
{
    public class VersionedListTests
    {
        [Test]
        public void Add_Item_IncrementsVersion()
        {
            var list = new VersionedList<int>();
            ulong initialVersion = list.Version;

            list.Add(1);

            Assert.That(list.Count, Is.EqualTo(1));
            Assert.That(list.Version, Is.Not.EqualTo(initialVersion));
            Assert.That(list[0], Is.EqualTo(1));
        }

        [Test]
        public void Add_Object_IncrementsVersion()
        {
            var list = new VersionedList<string>();
            ulong initialVersion = list.Version;

            ((IList)list).Add("test");

            Assert.That(list.Count, Is.EqualTo(1));
            Assert.That(list.Version, Is.Not.EqualTo(initialVersion));
        }

        [Test]
        public void Remove_ExistingItem_IncrementsVersion()
        {
            var list = new VersionedList<int> { 1, 2, 3 };
            ulong initialVersion = list.Version;

            bool removed = list.Remove(2);

            Assert.That(removed, Is.True);
            Assert.That(list.Count, Is.EqualTo(2));
            Assert.That(list.Version, Is.Not.EqualTo(initialVersion));
        }

        [Test]
        public void Remove_NonExistingItem_DoesNotIncrementVersion()
        {
            var list = new VersionedList<int> { 1, 2, 3 };
            ulong initialVersion = list.Version;

            bool removed = list.Remove(99);

            Assert.That(removed, Is.False);
            Assert.That(list.Count, Is.EqualTo(3));
            Assert.That(list.Version, Is.EqualTo(initialVersion));
        }

        [Test]
        public void Remove_Object_IncrementsVersion()
        {
            var list = new VersionedList<string> { "a", "b" };
            ulong initialVersion = list.Version;

            ((IList)list).Remove("a");

            Assert.That(list.Count, Is.EqualTo(1));
            Assert.That(list.Version, Is.Not.EqualTo(initialVersion));
        }
        
        [Test]
        public void Remove_NonExistingObject_DoesNotIncrementVersion()
        {
            var list = new VersionedList<string> { "a", "b" };
            ulong initialVersion = list.Version;

            ((IList)list).Remove("z");

            Assert.That(list.Count, Is.EqualTo(2));
            Assert.That(list.Version, Is.EqualTo(initialVersion));
        }

        [Test]
        public void RemoveAt_IncrementsVersion()
        {
            var list = new VersionedList<int> { 10, 20, 30 };
            ulong initialVersion = list.Version;

            list.RemoveAt(1); // Remove 20

            Assert.That(list.Count, Is.EqualTo(2));
            Assert.That(list[1], Is.EqualTo(30));
            Assert.That(list.Version, Is.Not.EqualTo(initialVersion));
        }

        [Test]
        public void Clear_IncrementsVersion()
        {
            var list = new VersionedList<int> { 1, 2, 3 };
            ulong initialVersion = list.Version;

            list.Clear();

            Assert.That(list.Count, Is.EqualTo(0));
            Assert.That(list.Version, Is.Not.EqualTo(initialVersion));
        }

        [Test]
        public void Insert_IncrementsVersion()
        {
            var list = new VersionedList<int> { 1, 3 };
            ulong initialVersion = list.Version;

            list.Insert(1, 2);

            Assert.That(list.Count, Is.EqualTo(3));
            Assert.That(list[1], Is.EqualTo(2));
            Assert.That(list.Version, Is.Not.EqualTo(initialVersion));
        }

        [Test]
        public void Indexer_Set_IncrementsVersion()
        {
            var list = new VersionedList<int> { 10, 20 };
            ulong initialVersion = list.Version;

            list[0] = 99;

            Assert.That(list[0], Is.EqualTo(99));
            Assert.That(list.Version, Is.Not.EqualTo(initialVersion));
        }

        [Test]
        public void Indexer_Get_DoesNotIncrementVersion()
        {
            var list = new VersionedList<int> { 10, 20 };
            ulong initialVersion = list.Version;

            var item = list[0];

            Assert.That(item, Is.EqualTo(10));
            Assert.That(list.Version, Is.EqualTo(initialVersion));
        }

        [Test]
        public void Contains_ReturnsCorrectValue()
        {
            var list = new VersionedList<string> { "hello", "world" };
            
            Assert.That(list.Contains("hello"), Is.True);
            Assert.That(list.Contains("universe"), Is.False);
        }

        [Test]
        public void IndexOf_ReturnsCorrectIndex()
        {
            var list = new VersionedList<int> { 10, 20, 30 };

            Assert.That(list.IndexOf(20), Is.EqualTo(1));
            Assert.That(list.IndexOf(99), Is.EqualTo(-1));
        }
        
        [Test]
        public void CopyTo_CopiesElements()
        {
            var list = new VersionedList<int> { 1, 2, 3 };
            var array = new int[3];

            list.CopyTo(array, 0);

            Assert.That(array, Is.EqualTo(new[] { 1, 2, 3 }));
        }
    }
}