using System.Linq;
using NUnit.Framework;
using Platonic.Core;

namespace Platonic.Editor.Tests
{
    public class DataTests
    {
        private readonly FieldName<int> TestInt = new(1, nameof(TestInt));
        private readonly FieldName<float> TestFloat = new(2, nameof(TestFloat));

        [Test]
        public void NewDataShouldContainInitialFields()
        {
            var data1 = new Data(new Field<int>(TestInt, 5));
            Assert.That(data1.Fields.Select(f=>f.Name.Id), Is.EquivalentTo(new []{TestInt.Id}));
            
            var data2 = new Data(new Field<int>(TestInt, 5), new Field<float>(TestFloat, 5f));
            Assert.That(data2.Fields.Select(f=>f.Name.Id), Is.EquivalentTo(new []{TestInt.Id, TestFloat.Id}));
        }

        [Test]
        public void MutableDataGetField()
        {
            var field = new Field<int>(TestInt, 5);
            var mutableData = new MutableData(field);
            var results = mutableData.TryGetField<int>(field.Name.Id, out var retrievedField);
            Assert.That(results, Is.True);
            Assert.That(retrievedField, Is.EqualTo(field));
        }
    }
}
