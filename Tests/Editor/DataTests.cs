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
            Assert.That(data1.Fields.Select(f=>f.Name.ID), Is.EquivalentTo(new []{TestInt.ID}));
            
            var data2 = new Data(new Field<int>(TestInt, 5), new Field<float>(TestFloat, 5f));
            Assert.That(data2.Fields.Select(f=>f.Name.ID), Is.EquivalentTo(new []{TestInt.ID, TestFloat.ID}));
        }

    }
}
