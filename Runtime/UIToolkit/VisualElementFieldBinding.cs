#nullable enable
using System;
using Platonic.Version;
using UnityEngine.UIElements;

namespace Platonic.UIToolkit
{
    /// <summary>
    /// Binds a VisualElement property or action to an IVersionedValue source.
    /// Tracks cached version against source version and applies updates only on version change.
    /// </summary>
    public class VisualElementFieldBinding<TElement, TValue> : IBinding<TElement, TValue>
        where TElement : VisualElement
    {
        private readonly Action<TElement, TValue> _apply;
        private readonly bool _autoSchedule;
        private readonly EventCallback<AttachToPanelEvent> _attachCallback;
        private readonly EventCallback<DetachFromPanelEvent> _detachCallback;
        private IVisualElementScheduledItem? _scheduledItem;

        public TElement Target { get; }
        VisualElement IBinding.Target => Target;

        public IVersionedValue<TValue> Source { get; }
        IVersioned IBinding.Source => Source;

        public IField<TValue>? Field => Source as IField<TValue>;

        public ulong CachedVersion { get; private set; } = Versions.None;

        public bool IsBound { get; private set; } = true;

        public VisualElementFieldBinding(
            TElement target,
            IVersionedValue<TValue> source,
            Action<TElement, TValue> apply,
            bool autoSchedule = true,
            bool registerWithRunner = true)
        {
            Target = target ?? throw new ArgumentNullException(nameof(target));
            Source = source ?? throw new ArgumentNullException(nameof(source));
            _apply = apply ?? throw new ArgumentNullException(nameof(apply));
            _autoSchedule = autoSchedule;

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

            // Perform initial binding update
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

            if (CachedVersion != Source.Version)
            {
                CachedVersion = Source.Version;
                _apply(Target, Source.Value);
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
