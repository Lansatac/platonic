#nullable enable

using NUnit.Framework;
using Platonic.Core;
using Platonic.Version;

namespace Platonic.Editor.Tests
{
    public class LookupTests
    {
        
        private readonly FieldName<int> _testInt = new(1, nameof(_testInt)); 
        private readonly FieldName<IData?> _testData = new(2, nameof(_testData));
        
        [Test]
        public void SimpleLookupShouldLookupData()
        {
            IData data1 = new MutableData(), data2 = new MutableData();
            var dataArray = new[] { data1, data2 };
            var index = new Field<int>(_testInt, 0);
            var lookup = _testData.Lookup(index, i => dataArray[i], null);
            Assert.That(lookup.Value, Is.EqualTo(data1));
        }

    }
}