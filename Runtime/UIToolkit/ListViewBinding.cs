#nullable enable
using System;
using System.Collections.Generic;
using Platonic.Collections;
using Platonic.Version;
using UnityEngine.UIElements;

namespace Platonic.UIToolkit
{
    /// <summary>
    /// Binds a ListView to any versioned sequence. The ListView reads a snapshot that is rebuilt, followed by
    /// <c>RefreshItems()</c>, only when the source's membership version changes.
    /// Gotcha: the source version covers membership only, so a row's contents do not refresh from here. Rows bind
    /// their own sources; because ListView recycles row elements, create those bindings once in makeItem and
    /// point them at the item with <see cref="VisualElementFieldBinding{TElement,TValue}.Rebind"/> in bindItem.
    /// Creating a new binding per bindItem call would stack bindings on the recycled element.
    /// </summary>
    public class ListViewBinding<T> : ElementBinding<ListView>
    {
        private readonly List<T> _snapshot = new();
        private bool _hasApplied;

        public IVersionedEnumerable<T> ListSource { get; }

        protected override IVersioned? SourceCore => ListSource;

        public ListViewBinding(
            ListView listView,
            IVersionedEnumerable<T> list,
            Func<VisualElement>? makeItem = null,
            Action<VisualElement, T>? bindItem = null,
            Action<VisualElement, T>? unbindItem = null,
            Action<VisualElement>? destroyItem = null)
            : base(listView)
        {
            ListSource = list ?? throw new ArgumentNullException(nameof(list));
            Initialize(
                makeItem,
                bindItem == null ? null : (element, index) => InvokeIfInRange(bindItem, element, index),
                unbindItem == null ? null : (element, index) => InvokeIfInRange(unbindItem, element, index),
                destroyItem);
        }

        public ListViewBinding(
            ListView listView,
            IVersionedEnumerable<T> list,
            Func<VisualElement> makeItem,
            Action<VisualElement, int> bindIndexItem,
            Action<VisualElement, int>? unbindIndexItem = null,
            Action<VisualElement>? destroyItem = null)
            : base(listView)
        {
            ListSource = list ?? throw new ArgumentNullException(nameof(list));
            Initialize(
                makeItem ?? throw new ArgumentNullException(nameof(makeItem)),
                bindIndexItem ?? throw new ArgumentNullException(nameof(bindIndexItem)),
                unbindIndexItem,
                destroyItem);
        }

        private void Initialize(
            Func<VisualElement>? makeItem,
            Action<VisualElement, int>? bindItem,
            Action<VisualElement, int>? unbindItem,
            Action<VisualElement>? destroyItem)
        {
            Target.itemsSource = _snapshot;
            if (makeItem != null) Target.makeItem = makeItem;
            if (bindItem != null) Target.bindItem = bindItem;
            if (unbindItem != null) Target.unbindItem = unbindItem;
            if (destroyItem != null) Target.destroyItem = destroyItem;

            Update();
        }

        private void InvokeIfInRange(Action<VisualElement, T> callback, VisualElement element, int index)
        {
            if (index >= 0 && index < _snapshot.Count)
            {
                callback(element, _snapshot[index]);
            }
        }

        protected override bool Poll()
        {
            var version = ListSource.Version;
            if (_hasApplied && version == CachedVersion) return false;

            _hasApplied = true;
            CachedVersion = version;

            _snapshot.Clear();
            _snapshot.AddRange(ListSource);
            Target.RefreshItems();
            return true;
        }
    }
}
