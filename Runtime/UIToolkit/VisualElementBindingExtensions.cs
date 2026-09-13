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
            Action<TElement, TValue> apply,
            bool autoSchedule = true,
            bool registerWithRunner = true)
            where TElement : VisualElement
        {
            return new VisualElementFieldBinding<TElement, TValue>(element, source, apply, autoSchedule, registerWithRunner);
        }

        /// <summary>
        /// Creates a generic binding between a VisualElement and a field on an IData object.
        /// </summary>
        public static VisualElementFieldBinding<TElement, TValue> Bind<TElement, TValue>(
            this TElement element,
            IData data,
            IFieldName<TValue> fieldName,
            Action<TElement, TValue> apply,
            bool autoSchedule = true,
            bool registerWithRunner = true)
            where TElement : VisualElement
        {
            if (data == null) throw new ArgumentNullException(nameof(data));
            if (fieldName == null) throw new ArgumentNullException(nameof(fieldName));

            return element.Bind(data.GetField(fieldName), apply, autoSchedule, registerWithRunner);
        }

        /// <summary>
        /// Binds the text property of a TextElement (Label, Button, etc.) to a string source.
        /// </summary>
        public static VisualElementFieldBinding<TElement, string> BindText<TElement>(
            this TElement element,
            IVersionedValue<string> source,
            bool autoSchedule = true,
            bool registerWithRunner = true)
            where TElement : TextElement
        {
            return element.Bind(source, (el, val) => el.text = val, autoSchedule, registerWithRunner);
        }

        /// <summary>
        /// Binds the text property of a TextElement to a source formatted with the provided function.
        /// </summary>
        public static VisualElementFieldBinding<TElement, TValue> BindText<TElement, TValue>(
            this TElement element,
            IVersionedValue<TValue> source,
            Func<TValue, string> formatter,
            bool autoSchedule = true,
            bool registerWithRunner = true)
            where TElement : TextElement
        {
            if (formatter == null) throw new ArgumentNullException(nameof(formatter));
            return element.Bind(source, (el, val) => el.text = formatter(val), autoSchedule, registerWithRunner);
        }

        /// <summary>
        /// Binds the text property of a TextElement to a string field on an IData object.
        /// </summary>
        public static VisualElementFieldBinding<TElement, string> BindText<TElement>(
            this TElement element,
            IData data,
            IFieldName<string> fieldName,
            bool autoSchedule = true,
            bool registerWithRunner = true)
            where TElement : TextElement
        {
            if (data == null) throw new ArgumentNullException(nameof(data));
            if (fieldName == null) throw new ArgumentNullException(nameof(fieldName));

            return element.BindText(data.GetField(fieldName), autoSchedule, registerWithRunner);
        }

        /// <summary>
        /// Binds the text property of a TextElement to a formatted field on an IData object.
        /// </summary>
        public static VisualElementFieldBinding<TElement, TValue> BindText<TElement, TValue>(
            this TElement element,
            IData data,
            IFieldName<TValue> fieldName,
            Func<TValue, string> formatter,
            bool autoSchedule = true,
            bool registerWithRunner = true)
            where TElement : TextElement
        {
            if (data == null) throw new ArgumentNullException(nameof(data));
            if (fieldName == null) throw new ArgumentNullException(nameof(fieldName));

            return element.BindText(data.GetField(fieldName), formatter, autoSchedule, registerWithRunner);
        }

        /// <summary>
        /// Binds the display style of a VisualElement to a boolean source (Flex when true, None when false).
        /// </summary>
        public static VisualElementFieldBinding<VisualElement, bool> BindDisplay(
            this VisualElement element,
            IVersionedValue<bool> isVisible,
            DisplayStyle visibleDisplay = DisplayStyle.Flex,
            bool autoSchedule = true,
            bool registerWithRunner = true)
        {
            return element.Bind(
                isVisible,
                (el, visible) => el.style.display = visible ? visibleDisplay : DisplayStyle.None,
                autoSchedule,
                registerWithRunner);
        }

        /// <summary>
        /// Binds the display style of a VisualElement to a DisplayStyle source.
        /// </summary>
        public static VisualElementFieldBinding<VisualElement, DisplayStyle> BindDisplay(
            this VisualElement element,
            IVersionedValue<DisplayStyle> displayStyle,
            bool autoSchedule = true,
            bool registerWithRunner = true)
        {
            return element.Bind(
                displayStyle,
                (el, display) => el.style.display = display,
                autoSchedule,
                registerWithRunner);
        }

        /// <summary>
        /// Binds the display style of a VisualElement to a boolean field on an IData object.
        /// </summary>
        public static VisualElementFieldBinding<VisualElement, bool> BindDisplay(
            this VisualElement element,
            IData data,
            IFieldName<bool> fieldName,
            DisplayStyle visibleDisplay = DisplayStyle.Flex,
            bool autoSchedule = true,
            bool registerWithRunner = true)
        {
            if (data == null) throw new ArgumentNullException(nameof(data));
            if (fieldName == null) throw new ArgumentNullException(nameof(fieldName));

            return element.BindDisplay(data.GetField(fieldName), visibleDisplay, autoSchedule, registerWithRunner);
        }

        /// <summary>
        /// Binds the value property of a ProgressBar to a float source.
        /// </summary>
        public static VisualElementFieldBinding<ProgressBar, float> BindProgressBar(
            this ProgressBar progressBar,
            IVersionedValue<float> source,
            bool autoSchedule = true,
            bool registerWithRunner = true)
        {
            return progressBar.Bind(source, (pb, val) => pb.value = val, autoSchedule, registerWithRunner);
        }

        /// <summary>
        /// Binds the value property of a ProgressBar to a float field on an IData object.
        /// </summary>
        public static VisualElementFieldBinding<ProgressBar, float> BindProgressBar(
            this ProgressBar progressBar,
            IData data,
            IFieldName<float> fieldName,
            bool autoSchedule = true,
            bool registerWithRunner = true)
        {
            if (data == null) throw new ArgumentNullException(nameof(data));
            if (fieldName == null) throw new ArgumentNullException(nameof(fieldName));

            return progressBar.BindProgressBar(data.GetField(fieldName), autoSchedule, registerWithRunner);
        }

        /// <summary>
        /// Binds the enabled state of a VisualElement to a boolean source.
        /// </summary>
        public static VisualElementFieldBinding<VisualElement, bool> BindEnabled(
            this VisualElement element,
            IVersionedValue<bool> isEnabled,
            bool autoSchedule = true,
            bool registerWithRunner = true)
        {
            return element.Bind(isEnabled, (el, enabled) => el.SetEnabled(enabled), autoSchedule, registerWithRunner);
        }

        /// <summary>
        /// Binds the enabled state of a VisualElement to a boolean field on an IData object.
        /// </summary>
        public static VisualElementFieldBinding<VisualElement, bool> BindEnabled(
            this VisualElement element,
            IData data,
            IFieldName<bool> fieldName,
            bool autoSchedule = true,
            bool registerWithRunner = true)
        {
            if (data == null) throw new ArgumentNullException(nameof(data));
            if (fieldName == null) throw new ArgumentNullException(nameof(fieldName));

            return element.BindEnabled(data.GetField(fieldName), autoSchedule, registerWithRunner);
        }

        /// <summary>
        /// Binds any control implementing INotifyValueChanged to a versioned source.
        /// </summary>
        public static VisualElementFieldBinding<TField, TValue> BindValue<TField, TValue>(
            this TField field,
            IVersionedValue<TValue> source,
            bool autoSchedule = true,
            bool registerWithRunner = true)
            where TField : VisualElement, INotifyValueChanged<TValue>
        {
            return field.Bind(source, (f, val) => f.value = val, autoSchedule, registerWithRunner);
        }

        /// <summary>
        /// Binds any control implementing INotifyValueChanged to a field on an IData object.
        /// </summary>
        public static VisualElementFieldBinding<TField, TValue> BindValue<TField, TValue>(
            this TField field,
            IData data,
            IFieldName<TValue> fieldName,
            bool autoSchedule = true,
            bool registerWithRunner = true)
            where TField : VisualElement, INotifyValueChanged<TValue>
        {
            if (data == null) throw new ArgumentNullException(nameof(data));
            if (fieldName == null) throw new ArgumentNullException(nameof(fieldName));

            return field.BindValue(data.GetField(fieldName), autoSchedule, registerWithRunner);
        }

        /// <summary>
        /// Binds a UI Toolkit ListView to an IVersionedList source with item factory and item binding callbacks.
        /// </summary>
        public static ListViewBinding<T> BindListView<T>(
            this ListView listView,
            IVersionedList<T> list,
            Func<VisualElement>? makeItem = null,
            Action<VisualElement, T>? bindItem = null,
            Action<VisualElement, T>? unbindItem = null,
            Action<VisualElement>? destroyItem = null,
            bool autoSchedule = true,
            bool registerWithRunner = true)
        {
            return new ListViewBinding<T>(listView, list, makeItem, bindItem, unbindItem, destroyItem, autoSchedule, registerWithRunner);
        }

        /// <summary>
        /// Binds a UI Toolkit ListView to an IVersionedList source with item factory and index-based item binding callbacks.
        /// </summary>
        public static ListViewBinding<T> BindListView<T>(
            this ListView listView,
            IVersionedList<T> list,
            Func<VisualElement> makeItem,
            Action<VisualElement, int> bindIndexItem,
            Action<VisualElement, int>? unbindIndexItem = null,
            Action<VisualElement>? destroyItem = null,
            bool autoSchedule = true,
            bool registerWithRunner = true)
        {
            return new ListViewBinding<T>(listView, list, makeItem, bindIndexItem, unbindIndexItem, destroyItem, autoSchedule, registerWithRunner);
        }
    }
}
