using NUnit.Framework;
using Platonic.Core;
using Platonic.Version;

namespace Platonic.Editor.Tests
{
    public class MutableFieldTests
    {
        private readonly FieldName<int> TestInt = new(1, nameof(TestInt)); 
    
    
        [Test]
        public void NewFieldShouldHaveInitialVersion()
        {
            var field = new MutableField<int>(TestInt, 5);
            Assert.That(field.Version, Is.EqualTo(Versions.Initial));
        }
    
        [Test]
        public void ChangingFieldValueShouldChangeFieldVersion()
        {
            var field = new MutableField<int>(TestInt, 5);
            field.Value = 6;
            Assert.That(field.Version, Is.Not.EqualTo(Versions.Initial));
        }
    
        [Test]
        public void NewFieldShouldHaveInitialValue([Values(1,5)]int initialValue)
        {
            var field = new MutableField<int>(TestInt, initialValue);
            Assert.That(field.Value, Is.EqualTo(initialValue));
        }
    
        [Test]
        public void ChangedFieldShouldHaveChangedValue([Values(1,5)]int changedValue)
        {
            var field = new MutableField<int>(TestInt, 0);
            field.Value = changedValue;
            Assert.That(field.Value, Is.EqualTo(changedValue));
        }
    }
}
