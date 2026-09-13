#nullable enable
using System;
using System.Collections;
using Platonic.Collections;
using Platonic.Version;
using UnityEngine.UIElements;

namespace Platonic.UIToolkit
{
    /// <summary>
    /// Binds a UI Toolkit ListView to an IVersionedList source.
    /// Handles makeItem, bindItem, and unbindItem delegates and invokes RefreshItems() only on list version change.
    /// </summary>
    public class ListViewBinding<T> : IBinding
    {
        private readonly bool _autoSchedule;
        private readonly EventCallback<AttachToPanelEvent> _attachCallback;
        private readonly EventCallback<DetachFromPanelEvent> _detachCallback;
        private IVisualElementScheduledItem? _scheduledItem;

        public ListView Target { get; }
        VisualElement IBinding.Target => Target;

        public IVersionedList<T> ListSource { get; }
        IVersioned IBinding.Source => ListSource;

        public ulong CachedVersion { get; private set; } = Versions.None;

        public bool IsBound { get; private set; } = true;

        public ListViewBinding(
            ListView listView,
            IVersionedList<T> list,
            Func<VisualElement>? makeItem = null,
            Action<VisualElement, T>? bindItem = null,
            Action<VisualElement, T>? unbindItem = null,
            Action<VisualElement>? destroyItem = null,
            bool autoSchedule = true,
            bool registerWithRunner = true)
        {
            Target = listView ?? throw new ArgumentNullException(nameof(listView));
            ListSource = list ?? throw new ArgumentNullException(nameof(list));
            _autoSchedule = autoSchedule;

            Target.itemsSource = (IList)list;

            if (makeItem != null)
            {
                Target.makeItem = makeItem;
            }

            if (bindItem != null)
            {
                Target.bindItem = (element, index) =>
                {
                    if (index >= 0 && index < list.Count)
                    {
                        bindItem(element, list[index]);
                    }
                };
            }

            if (unbindItem != null)
            {
                Target.unbindItem = (element, index) =>
                {
                    if (index >= 0 && index < list.Count)
                    {
                        unbindItem(element, list[index]);
                    }
                };
            }

            if (destroyItem != null)
            {
                Target.destroyItem = destroyItem;
            }

            _attachCallback = _ => OnAttachToPanel();
            _detachCallback = _ => OnDetachFromPanel();
            Target.RegisterCallback(_attachCallback);
            Target.RegisterCallback(_detachCallback);

            if (registerWithRunner)
            {
                BindingRunner.Register(this);
            }

            if (_autoSchedule && Target.panel != null)
            {
                StartScheduling();
            }

            Update();
        }

        public ListViewBinding(
            ListView listView,
            IVersionedList<T> list,
            Func<VisualElement> makeItem,
            Action<VisualElement, int> bindIndexItem,
            Action<VisualElement, int>? unbindIndexItem = null,
            Action<VisualElement>? destroyItem = null,
            bool autoSchedule = true,
            bool registerWithRunner = true)
        {
            Target = listView ?? throw new ArgumentNullException(nameof(listView));
            ListSource = list ?? throw new ArgumentNullException(nameof(list));
            _autoSchedule = autoSchedule;

            Target.itemsSource = (IList)list;
            Target.makeItem = makeItem ?? throw new ArgumentNullException(nameof(makeItem));
            Target.bindItem = bindIndexItem ?? throw new ArgumentNullException(nameof(bindIndexItem));

            if (unbindIndexItem != null)
            {
                Target.unbindItem = unbindIndexItem;
            }

            if (destroyItem != null)
            {
                Target.destroyItem = destroyItem;
            }

            _attachCallback = _ => OnAttachToPanel();
            _detachCallback = _ => OnDetachFromPanel();
            Target.RegisterCallback(_attachCallback);
            Target.RegisterCallback(_detachCallback);

            if (registerWithRunner)
            {
                BindingRunner.Register(this);
            }

            if (_autoSchedule && Target.panel != null)
            {
                StartScheduling();
            }

            Update();
        }

        private void StartScheduling()
        {
            if (_scheduledItem == null)
            {
                _scheduledItem = Target.schedule.Execute(() => Update());
            }
            else
            {
                _scheduledItem.Resume();
            }
        }

        private void OnAttachToPanel()
        {
            if (!IsBound) return;

            if (_autoSchedule)
            {
                StartScheduling();
            }

            Update();
        }

        private void OnDetachFromPanel()
        {
            _scheduledItem?.Pause();
        }

        public bool Update()
        {
            if (!IsBound) return false;

            if (CachedVersion != ListSource.Version)
            {
                CachedVersion = ListSource.Version;
                Target.RefreshItems();
                return true;
            }

            return false;
        }

        public void Unbind()
        {
            if (!IsBound) return;

            IsBound = false;
            _scheduledItem?.Pause();
            _scheduledItem = null;

            Target.UnregisterCallback(_attachCallback);
            Target.UnregisterCallback(_detachCallback);

            BindingRunner.Unregister(this);
        }

        public void Dispose()
        {
            Unbind();
        }
    }
}
