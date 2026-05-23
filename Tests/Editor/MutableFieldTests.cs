using NUnit.Framework;
using Platonic.Core;
using Platonic.Version;

namespace Platonic.Editor.Tests
{
    public class MutableFieldTests
    {
        private readonly FieldName<int> TestInt = new(1, nameof(TestInt)); 
        private readonly FieldName<VersionedValue<int>> TestVersionedValue = new(2, nameof(TestVersionedValue)); 
    
    
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

        [Test]
        public void ReadingVersionedValueFieldVersionWithoutValueChangeShouldNotChangeFieldVersion()
        {
            var value = new VersionedValue<int>(5);
            var field = new MutableField<VersionedValue<int>>(TestVersionedValue, value);
            ulong initialVersion = field.Version;

            Assert.That(field.Version, Is.EqualTo(initialVersion));
        }

        [Test]
        public void ChangingVersionedValueShouldChangeFieldVersion()
        {
            var value = new VersionedValue<int>(5);
            var field = new MutableField<VersionedValue<int>>(TestVersionedValue, value);
            ulong initialVersion = field.Version;

            value.Value = 6;

            Assert.That(field.Version, Is.Not.EqualTo(initialVersion));
            Assert.That(field.Value.Value, Is.EqualTo(6));
        }

        [Test]
        public void AssigningSameVersionedValueValueShouldNotChangeFieldVersion()
        {
            var value = new VersionedValue<int>(5);
            var field = new MutableField<VersionedValue<int>>(TestVersionedValue, value);
            ulong initialVersion = field.Version;

            value.Value = 5;

            Assert.That(field.Version, Is.EqualTo(initialVersion));
        }

        [Test]
        public void ReplacedVersionedValueShouldForwardNewValueVersionChanges()
        {
            var value = new VersionedValue<int>(5);
            var replacement = new VersionedValue<int>(10);
            var field = new MutableField<VersionedValue<int>>(TestVersionedValue, value);
            _ = field.Version;

            field.Value = replacement;
            ulong replacementVersion = field.Version;
            value.Value = 6;

            Assert.That(field.Version, Is.EqualTo(replacementVersion));

            replacement.Value = 11;

            Assert.That(field.Version, Is.Not.EqualTo(replacementVersion));
            Assert.That(field.Value, Is.SameAs(replacement));
        }
    }
}
