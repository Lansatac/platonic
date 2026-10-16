#nullable enable
using System;
using Platonic.Collections;
using Platonic.Core;
using Platonic.Version;
using UnityEngine.UIElements;

namespace Platonic.UIToolkit
{
    /// <summary>
    /// Fluent extension methods for binding UI Toolkit controls to Platonic versioned sources and fields.
    /// </summary>
    public static class VisualElementBindingExtensions
    {
        /// <summary>
        /// Creates a generic binding between a VisualElement and an IVersionedValue source.
        /// </summary>
        public static VisualElementFieldBinding<TElement, TValue> Bind<TElement, TValue>(
            this TElement element,
            IVersionedValue<TValue> source,
            Action<TElement, TValue> apply)
            where TElement : VisualElement
        {
            return new VisualElementFieldBinding<TElement, TValue>(element, source, apply);
        }

        /// <summary>
        /// Creates a binding with no source yet. Point it at one with <c>Rebind</c>. Use this for recycled elements
        /// (ListView rows): create the slot once in makeItem and rebind it in bindItem.
        /// </summary>
        public static VisualElementFieldBinding<TElement, TValue> BindSlot<TElement, TValue>(
            this TElement element,
            Action<TElement, TValue> apply)
            where TElement : VisualElement
        {
            return new VisualElementFieldBinding<TElement, TValue>(element, null, apply);
        }

        /// <summary>
        /// Slot for the text of a TextElement. Point it at a source with <c>Rebind</c>.
        /// </summary>
        public static VisualElementFieldBinding<TElement, string> BindTextSlot<TElement>(this TElement element)
            where TElement : TextElement
        {
            return element.BindSlot<TElement, string>((el, val) => el.text = val);
        }

        /// <summary>
        /// Toggles a USS class on the element from a boolean source.
        /// </summary>
        public static VisualElementFieldBinding<VisualElement, bool> BindClass(
            this VisualElement element,
            string className,
            IVersionedValue<bool> source)
        {
            if (string.IsNullOrEmpty(className)) throw new ArgumentException("Class name is required.", nameof(className));

            return element.Bind(source, (el, enabled) => el.EnableInClassList(className, enabled));
        }

        /// <summary>
        /// Creates a generic binding between a VisualElement and a field on an IData object.
        /// </summary>
        public static VisualElementFieldBinding<TElement, TValue> Bind<TElement, TValue>(
            this TElement element,
            IData data,
            IFieldName<TValue> fieldName,
            Action<TElement, TValue> apply)
            where TElement : VisualElement
        {
            if (data == null) throw new ArgumentNullException(nameof(data));
            if (fieldName == null) throw new ArgumentNullException(nameof(fieldName));

            return element.Bind(data.GetField(fieldName), apply);
        }

        /// <summary>
        /// Binds the text property of a TextElement (Label, Button, etc.) to a string source.
        /// </summary>
        public static VisualElementFieldBinding<TElement, string> BindText<TElement>(
            this TElement element,
            IVersionedValue<string> source)
            where TElement : TextElement
        {
            return element.Bind(source, (el, val) => el.text = val);
        }

        /// <summary>
        /// Binds the text property of a TextElement to a source formatted with the provided function.
        /// </summary>
        public static VisualElementFieldBinding<TElement, TValue> BindText<TElement, TValue>(
            this TElement element,
            IVersionedValue<TValue> source,
            Func<TValue, string> formatter)
            where TElement : TextElement
        {
            if (formatter == null) throw new ArgumentNullException(nameof(formatter));
            return element.Bind(source, (el, val) => el.text = formatter(val));
        }

        /// <summary>
        /// Binds the text property of a TextElement to a string field on an IData object.
        /// </summary>
        public static VisualElementFieldBinding<TElement, string> BindText<TElement>(
            this TElement element,
            IData data,
            IFieldName<string> fieldName)
            where TElement : TextElement
        {
            if (data == null) throw new ArgumentNullException(nameof(data));
            if (fieldName == null) throw new ArgumentNullException(nameof(fieldName));

            return element.BindText(data.GetField(fieldName));
        }

        /// <summary>
        /// Binds the text property of a TextElement to a formatted field on an IData object.
        /// </summary>
        public static VisualElementFieldBinding<TElement, TValue> BindText<TElement, TValue>(
            this TElement element,
            IData data,
            IFieldName<TValue> fieldName,
            Func<TValue, string> formatter)
            where TElement : TextElement
        {
            if (data == null) throw new ArgumentNullException(nameof(data));
            if (fieldName == null) throw new ArgumentNullException(nameof(fieldName));

            return element.BindText(data.GetField(fieldName), formatter);
        }

        /// <summary>
        /// Binds the display style of a VisualElement to a boolean source (Flex when true, None when false).
        /// </summary>
        public static VisualElementFieldBinding<VisualElement, bool> BindDisplay(
            this VisualElement element,
            IVersionedValue<bool> isVisible,
            DisplayStyle visibleDisplay = DisplayStyle.Flex)
        {
            return element.Bind(
                isVisible,
                (el, visible) => el.style.display = visible ? visibleDisplay : DisplayStyle.None);
        }

        /// <summary>
        /// Binds the display style of a VisualElement to a DisplayStyle source.
        /// </summary>
        public static VisualElementFieldBinding<VisualElement, DisplayStyle> BindDisplay(
            this VisualElement element,
            IVersionedValue<DisplayStyle> displayStyle)
        {
            return element.Bind(
                displayStyle,
                (el, display) => el.style.display = display);
        }

        /// <summary>
        /// Binds the display style of a VisualElement to a boolean field on an IData object.
        /// </summary>
        public static VisualElementFieldBinding<VisualElement, bool> BindDisplay(
            this VisualElement element,
            IData data,
            IFieldName<bool> fieldName,
            DisplayStyle visibleDisplay = DisplayStyle.Flex)
        {
            if (data == null) throw new ArgumentNullException(nameof(data));
            if (fieldName == null) throw new ArgumentNullException(nameof(fieldName));

            return element.BindDisplay(data.GetField(fieldName), visibleDisplay);
        }

        /// <summary>
        /// Binds the value property of a ProgressBar to a float source.
        /// </summary>
        public static VisualElementFieldBinding<ProgressBar, float> BindProgressBar(
            this ProgressBar progressBar,
            IVersionedValue<float> source)
        {
            return progressBar.Bind(source, (pb, val) => pb.value = val);
        }

        /// <summary>
        /// Binds the value property of a ProgressBar to a float field on an IData object.
        /// </summary>
        public static VisualElementFieldBinding<ProgressBar, float> BindProgressBar(
            this ProgressBar progressBar,
            IData data,
            IFieldName<float> fieldName)
        {
            if (data == null) throw new ArgumentNullException(nameof(data));
            if (fieldName == null) throw new ArgumentNullException(nameof(fieldName));

            return progressBar.BindProgressBar(data.GetField(fieldName));
        }

        /// <summary>
        /// Binds the enabled state of a VisualElement to a boolean source.
        /// </summary>
        public static VisualElementFieldBinding<VisualElement, bool> BindEnabled(
            this VisualElement element,
            IVersionedValue<bool> isEnabled)
        {
            return element.Bind(isEnabled, (el, enabled) => el.SetEnabled(enabled));
        }

        /// <summary>
        /// Binds the enabled state of a VisualElement to a boolean field on an IData object.
        /// </summary>
        public static VisualElementFieldBinding<VisualElement, bool> BindEnabled(
            this VisualElement element,
            IData data,
            IFieldName<bool> fieldName)
        {
            if (data == null) throw new ArgumentNullException(nameof(data));
            if (fieldName == null) throw new ArgumentNullException(nameof(fieldName));

            return element.BindEnabled(data.GetField(fieldName));
        }

        /// <summary>
        /// Binds any control implementing INotifyValueChanged to a versioned source.
        /// </summary>
        public static VisualElementFieldBinding<TField, TValue> BindValue<TField, TValue>(
            this TField field,
            IVersionedValue<TValue> source)
            where TField : VisualElement, INotifyValueChanged<TValue>
        {
            return field.Bind(source, (f, val) => f.value = val);
        }

        /// <summary>
        /// Binds any control implementing INotifyValueChanged to a field on an IData object.
        /// </summary>
        public static VisualElementFieldBinding<TField, TValue> BindValue<TField, TValue>(
            this TField field,
            IData data,
            IFieldName<TValue> fieldName)
            where TField : VisualElement, INotifyValueChanged<TValue>
        {
            if (data == null) throw new ArgumentNullException(nameof(data));
            if (fieldName == null) throw new ArgumentNullException(nameof(fieldName));

            return field.BindValue(data.GetField(fieldName));
        }

        /// <summary>
        /// Binds a UI Toolkit ListView to an IVersionedList source with item factory and item binding callbacks.
        /// </summary>
        public static ListViewBinding<T> BindListView<T>(
            this ListView listView,
            IVersionedEnumerable<T> list,
            Func<VisualElement>? makeItem = null,
            Action<VisualElement, T>? bindItem = null,
            Action<VisualElement, T>? unbindItem = null,
            Action<VisualElement>? destroyItem = null)
        {
            return new ListViewBinding<T>(listView, list, makeItem, bindItem, unbindItem, destroyItem);
        }

        /// <summary>
        /// Binds a UI Toolkit ListView to an IVersionedList source with item factory and index-based item binding callbacks.
        /// </summary>
        public static ListViewBinding<T> BindListView<T>(
            this ListView listView,
            IVersionedEnumerable<T> list,
            Func<VisualElement> makeItem,
            Action<VisualElement, int> bindIndexItem,
            Action<VisualElement, int>? unbindIndexItem = null,
            Action<VisualElement>? destroyItem = null)
        {
            return new ListViewBinding<T>(listView, list, makeItem, bindIndexItem, unbindIndexItem, destroyItem);
        }
    }
}
