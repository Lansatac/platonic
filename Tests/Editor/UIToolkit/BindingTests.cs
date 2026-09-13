using System;
using System.Collections.Generic;
using NUnit.Framework;
using Platonic.Collections;
using Platonic.Core;
using Platonic.UIToolkit;
using Platonic.Version;
using UnityEngine.UIElements;

namespace Platonic.Editor.Tests.UIToolkit
{
    public class BindingTests
    {
        private readonly FieldName<string> _nameField = new(101, "Name");
        private readonly FieldName<int> _countField = new(102, "Count");
        private readonly FieldName<float> _progressField = new(103, "Progress");
        private readonly FieldName<bool> _activeField = new(104, "Active");

        [Test]
        public void BindText_UpdatesLabelText_WhenVersionChanges()
        {
            var field = new MutableField<string>(_nameField, "Initial");
            var label = new Label();

            var binding = label.BindText(field);

            Assert.That(label.text, Is.EqualTo("Initial"));
            Assert.That(binding.CachedVersion, Is.EqualTo(field.Version));

            field.Value = "Updated";
            Assert.That(label.text, Is.EqualTo("Initial"), "Should not update before binding update tick");

            var updated = binding.Update();
            Assert.That(updated, Is.True);
            Assert.That(label.text, Is.EqualTo("Updated"));
            Assert.That(binding.CachedVersion, Is.EqualTo(field.Version));
        }

        [Test]
        public void BindText_WithCustomFormatter_UpdatesCorrectly()
        {
            var field = new MutableField<int>(_countField, 42);
            var label = new Label();

            var binding = label.BindText(field, count => $"Items: {count}");

            Assert.That(label.text, Is.EqualTo("Items: 42"));

            field.Value = 100;
            binding.Update();
            Assert.That(label.text, Is.EqualTo("Items: 100"));
        }

        [Test]
        public void Binding_SuppressesDomMutation_WhenVersionMatchesCachedVersion()
        {
            var field = new MutableField<string>(_nameField, "Constant");
            var element = new VisualElement();
            var mutationCount = 0;

            var binding = element.Bind(field, (el, val) =>
            {
                mutationCount++;
            });

            Assert.That(mutationCount, Is.EqualTo(1), "Initial bind should apply value once");

            var updated = binding.Update();
            Assert.That(updated, Is.False, "Update should return false when version is unchanged");
            Assert.That(mutationCount, Is.EqualTo(1), "No DOM mutation should occur when version is unchanged");

            field.Value = "Changed";
            updated = binding.Update();
            Assert.That(updated, Is.True);
            Assert.That(mutationCount, Is.EqualTo(2));

            updated = binding.Update();
            Assert.That(updated, Is.False);
            Assert.That(mutationCount, Is.EqualTo(2));
        }

        [Test]
        public void BindDisplay_UpdatesStyleDisplay_BasedOnBoolean()
        {
            var field = new MutableField<bool>(_activeField, true);
            var element = new VisualElement();

            var binding = element.BindDisplay(field);

            Assert.That(element.style.display.value, Is.EqualTo(DisplayStyle.Flex));

            field.Value = false;
            binding.Update();
            Assert.That(element.style.display.value, Is.EqualTo(DisplayStyle.None));

            field.Value = true;
            binding.Update();
            Assert.That(element.style.display.value, Is.EqualTo(DisplayStyle.Flex));
        }

        [Test]
        public void BindDisplay_SupportsCustomVisibleDisplayStyle()
        {
            var field = new MutableField<bool>(_activeField, false);
            var element = new VisualElement();

            var binding = element.BindDisplay(field, DisplayStyle.Flex);
            Assert.That(element.style.display.value, Is.EqualTo(DisplayStyle.None));

            field.Value = true;
            binding.Update();
            Assert.That(element.style.display.value, Is.EqualTo(DisplayStyle.Flex));
        }

        [Test]
        public void BindProgressBar_UpdatesValue()
        {
            var field = new MutableField<float>(_progressField, 0.25f);
            var progressBar = new ProgressBar();

            var binding = progressBar.BindProgressBar(field);

            Assert.That(progressBar.value, Is.EqualTo(0.25f).Within(0.001f));

            field.Value = 0.75f;
            binding.Update();
            Assert.That(progressBar.value, Is.EqualTo(0.75f).Within(0.001f));
        }

        [Test]
        public void BindEnabled_UpdatesElementEnabledState()
        {
            var field = new MutableField<bool>(_activeField, false);
            var button = new Button();

            var binding = button.BindEnabled(field);

            Assert.That(button.enabledSelf, Is.False);

            field.Value = true;
            binding.Update();
            Assert.That(button.enabledSelf, Is.True);
        }

        [Test]
        public void BindValue_UpdatesNotifyValueChangedControl()
        {
            var field = new MutableField<string>(_nameField, "Alpha");
            var textField = new TextField();

            var binding = textField.BindValue(field);

            Assert.That(textField.value, Is.EqualTo("Alpha"));

            field.Value = "Beta";
            binding.Update();
            Assert.That(textField.value, Is.EqualTo("Beta"));
        }

        [Test]
        public void BindingRunner_UpdateAll_TicksAllRegisteredBindings()
        {
            var field1 = new MutableField<string>(_nameField, "One");
            var field2 = new MutableField<int>(_countField, 1);

            var label1 = new Label();
            var label2 = new Label();

            var b1 = label1.BindText(field1);
            var b2 = label2.BindText(field2, c => c.ToString());

            field1.Value = "Two";
            field2.Value = 2;

            var updatedCount = BindingRunner.UpdateAll();

            Assert.That(updatedCount, Is.GreaterThanOrEqualTo(2));
            Assert.That(label1.text, Is.EqualTo("Two"));
            Assert.That(label2.text, Is.EqualTo("2"));

            b1.Dispose();
            b2.Dispose();
        }

        [Test]
        public void Unbind_And_Dispose_PreventsFurtherUpdates()
        {
            var field = new MutableField<string>(_nameField, "Original");
            var label = new Label();

            var binding = label.BindText(field);
            Assert.That(binding.IsBound, Is.True);

            binding.Unbind();
            Assert.That(binding.IsBound, Is.False);

            field.Value = "Modified";
            var updated = binding.Update();
            Assert.That(updated, Is.False);
            Assert.That(label.text, Is.EqualTo("Original"));
        }

        [Test]
        public void ListViewBinding_RefreshesItems_WhenVersionIncrements()
        {
            var list = new VersionedList<string> { "Item 1", "Item 2" };
            var listView = new ListView();

            var makeCount = 0;
            var bindCount = 0;

            var binding = listView.BindListView(
                list,
                makeItem: () =>
                {
                    makeCount++;
                    return new Label();
                },
                bindItem: (element, item) =>
                {
                    bindCount++;
                    ((Label)element).text = item;
                });

            Assert.That(binding.IsBound, Is.True);
            Assert.That(listView.itemsSource, Is.SameAs(list));
            Assert.That(binding.CachedVersion, Is.EqualTo(list.Version));

            // No version change -> update returns false
            var updated = binding.Update();
            Assert.That(updated, Is.False);

            // Mutate list -> version changes
            list.Add("Item 3");
            Assert.That(list.Version, Is.GreaterThan(binding.CachedVersion));

            updated = binding.Update();
            Assert.That(updated, Is.True);
            Assert.That(binding.CachedVersion, Is.EqualTo(list.Version));
            Assert.That(listView.itemsSource.Count, Is.EqualTo(3));
        }

        [Test]
        public void BindFromData_UpdatesControlsViaFieldName()
        {
            var nameField = new MutableField<string>(_nameField, "InitialData");
            var activeField = new MutableField<bool>(_activeField, false);
            var progressField = new MutableField<float>(_progressField, 0.1f);

            var data = new MutableData(nameField, activeField, progressField);

            var label = new Label();
            var button = new Button();
            var progressBar = new ProgressBar();

            var b1 = label.BindText(data, _nameField);
            var b2 = button.BindEnabled(data, _activeField);
            var b3 = progressBar.BindProgressBar(data, _progressField);

            Assert.That(label.text, Is.EqualTo("InitialData"));
            Assert.That(button.enabledSelf, Is.False);
            Assert.That(progressBar.value, Is.EqualTo(0.1f).Within(0.001f));

            nameField.Value = "UpdatedData";
            activeField.Value = true;
            progressField.Value = 0.9f;

            b1.Update();
            b2.Update();
            b3.Update();

            Assert.That(label.text, Is.EqualTo("UpdatedData"));
            Assert.That(button.enabledSelf, Is.True);
            Assert.That(progressBar.value, Is.EqualTo(0.9f).Within(0.001f));
        }

        [Test]
        public void BindDisplay_WithDirectDisplayStyle_UpdatesCorrectly()
        {
            var field = new MutableField<DisplayStyle>(new FieldName<DisplayStyle>(105, "Display"), DisplayStyle.None);
            var element = new VisualElement();

            var binding = element.BindDisplay(field);
            Assert.That(element.style.display.value, Is.EqualTo(DisplayStyle.None));

            field.Value = DisplayStyle.Flex;
            binding.Update();
            Assert.That(element.style.display.value, Is.EqualTo(DisplayStyle.Flex));
        }

        [Test]
        public void ListViewBinding_IndexBasedCallbacks_ExecuteProperly()
        {
            var list = new VersionedList<int> { 10, 20, 30 };
            var listView = new ListView();

            var boundIndices = new List<int>();
            var binding = listView.BindListView(
                list,
                makeItem: () => new Label(),
                bindIndexItem: (el, index) => boundIndices.Add(index));

            Assert.That(binding.IsBound, Is.True);
            Assert.That(listView.itemsSource, Is.SameAs(list));

            // Verify bindItem callback works
            var label = new Label();
            listView.bindItem(label, 1);
            Assert.That(boundIndices, Contains.Item(1));
        }
    }
}
