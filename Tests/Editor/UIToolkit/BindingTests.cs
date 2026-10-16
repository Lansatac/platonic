using System;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEditor;
using Platonic.Collections;
using Platonic.Core;
using Platonic.UIToolkit;
using Platonic.Version;
using UnityEngine.UIElements;

namespace Platonic.Editor.Tests.UIToolkit
{
    public class BindingTests
    {
        private sealed class TestWindow : EditorWindow { }

        private sealed class StubVersioned<T> : IVersionedValue<T>
        {
            public ulong Version { get; set; }
            public T Value { get; set; } = default!;
        }

        private TestWindow? _window;

        /// <summary>A root that is really attached to a panel, so attach/detach events fire.</summary>
        private VisualElement CreateAttachedRoot()
        {
            _window = EditorWindow.CreateInstance<TestWindow>();
            _window.Show();
            return _window.rootVisualElement;
        }

        [TearDown]
        public void TearDown()
        {
            if (_window != null) _window.Close();
            _window = null;
            BindingRunner.Clear();
        }

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
        public void UnattachedBinding_IsNotPolledByRunner()
        {
            var field = new MutableField<string>(_nameField, "One");
            var label = new Label();
            var binding = label.BindText(field);

            Assert.That(binding.IsActive, Is.False);
            Assert.That(BindingRunner.ActiveBindingCount, Is.EqualTo(0));

            field.Value = "Two";
            BindingRunner.UpdateAll();
            Assert.That(label.text, Is.EqualTo("One"));
        }

        [Test]
        public void AttachedBinding_IsPolledByRunner_AndReleasedOnDetach()
        {
            var field = new MutableField<string>(_nameField, "One");
            var root = CreateAttachedRoot();
            var label = new Label();
            root.Add(label);

            var binding = label.BindText(field);
            Assert.That(binding.IsActive, Is.True);
            Assert.That(BindingRunner.ActiveBindingCount, Is.EqualTo(1));

            field.Value = "Two";
            Assert.That(BindingRunner.UpdateAll(), Is.EqualTo(1));
            Assert.That(label.text, Is.EqualTo("Two"));

            label.RemoveFromHierarchy();
            Assert.That(binding.IsActive, Is.False);
            Assert.That(BindingRunner.ActiveBindingCount, Is.EqualTo(0));

            field.Value = "Three";
            BindingRunner.UpdateAll();
            Assert.That(label.text, Is.EqualTo("Two"));
        }

        [Test]
        public void ReattachedBinding_CatchesUpOnChangesMadeWhileDetached()
        {
            var field = new MutableField<string>(_nameField, "One");
            var root = CreateAttachedRoot();
            var label = new Label();
            root.Add(label);
            label.BindText(field);

            label.RemoveFromHierarchy();
            field.Value = "Two";
            root.Add(label);

            Assert.That(label.text, Is.EqualTo("Two"));
        }

        [Test]
        public void Binding_AppliesInitialValue_WhenSourceVersionIsNone()
        {
            var source = new StubVersioned<string> { Version = Versions.None, Value = "Initial" };
            var label = new Label();

            label.BindText(source);

            Assert.That(label.text, Is.EqualTo("Initial"));
        }

        [Test]
        public void Rebind_SwitchesSource_AndIgnoresTheOldOne()
        {
            var first = new MutableField<string>(_nameField, "First");
            var second = new MutableField<string>(_nameField, "Second");
            var label = new Label();
            var binding = label.BindText(first);

            binding.Rebind(second);
            Assert.That(label.text, Is.EqualTo("Second"));

            first.Value = "Stale";
            binding.Update();
            Assert.That(label.text, Is.EqualTo("Second"));

            second.Value = "SecondUpdated";
            binding.Update();
            Assert.That(label.text, Is.EqualTo("SecondUpdated"));
        }

        [Test]
        public void Slot_AppliesNothing_UntilRebound()
        {
            var label = new Label { text = "Untouched" };
            var slot = label.BindTextSlot();

            Assert.That(slot.Update(), Is.False);
            Assert.That(label.text, Is.EqualTo("Untouched"));

            slot.Rebind(new MutableField<string>(_nameField, "Bound"));
            Assert.That(label.text, Is.EqualTo("Bound"));
        }

        [Test]
        public void BindingScope_Dispose_UnbindsEverything()
        {
            var field = new MutableField<string>(_nameField, "One");
            var label = new Label();
            var scope = new BindingScope();
            var binding = scope.Add(label.BindText(field));

            scope.Dispose();

            Assert.That(binding.IsBound, Is.False);
            field.Value = "Two";
            binding.Update();
            Assert.That(label.text, Is.EqualTo("One"));
        }

        [Test]
        public void BindClass_TogglesClassFromSource()
        {
            var flag = new MutableField<bool>(_activeField, false);
            var element = new VisualElement();
            var binding = element.BindClass("is-active", flag);
            Assert.That(element.ClassListContains("is-active"), Is.False);

            flag.Value = true;
            binding.Update();
            Assert.That(element.ClassListContains("is-active"), Is.True);
        }

        [Test]
        public void ListView_RecycledRow_UsesOneSlotRebound_NotNewBindingsPerBind()
        {
            var rowA = new MutableField<string>(_nameField, "A");
            var rowB = new MutableField<string>(_nameField, "B");
            var rows = new VersionedList<IVersionedValue<string>> { rowA, rowB };
            var listView = new ListView();

            var slots = new List<VisualElementFieldBinding<Label, string>>();
            listView.BindListView(
                rows,
                makeItem: () =>
                {
                    var label = new Label();
                    slots.Add(label.BindTextSlot());
                    return label;
                },
                bindItem: (element, source) =>
                    slots[0].Rebind(source));

            var recycled = listView.makeItem();
            listView.bindItem(recycled, 0);
            Assert.That(((Label)recycled).text, Is.EqualTo("A"));
            listView.bindItem(recycled, 1);
            Assert.That(((Label)recycled).text, Is.EqualTo("B"));

            Assert.That(slots.Count, Is.EqualTo(1));
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
            Assert.That(listView.itemsSource.Count, Is.EqualTo(2));
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
            Assert.That(listView.itemsSource.Count, Is.EqualTo(3));

            // Verify bindItem callback works
            var label = new Label();
            listView.bindItem(label, 1);
            Assert.That(boundIndices, Contains.Item(1));
        }
    }
}
