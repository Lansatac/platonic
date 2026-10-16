#nullable enable
using System;
using Platonic.Version;
using UnityEngine.UIElements;

namespace Platonic.UIToolkit
{
    /// <summary>
    /// Shared lifecycle for bindings. A binding is polled by <see cref="BindingRunner"/> only while its
    /// target is attached to a panel, so a binding on an element that is removed or destroyed stops costing
    /// anything and is released with the element. Call <see cref="Unbind"/> (or use a <see cref="BindingScope"/>)
    /// to end a binding while its element stays alive.
    /// </summary>
    public abstract class ElementBinding<TElement> : IBinding where TElement : VisualElement
    {
        private readonly EventCallback<AttachToPanelEvent> _attachCallback;
        private readonly EventCallback<DetachFromPanelEvent> _detachCallback;

        public TElement Target { get; }
        VisualElement IBinding.Target => Target;

        IVersioned? IBinding.Source => SourceCore;

        public ulong CachedVersion { get; protected set; } = Versions.None;

        public bool IsBound { get; private set; } = true;

        public bool IsActive { get; private set; }

        protected abstract IVersioned? SourceCore { get; }

        protected ElementBinding(TElement target)
        {
            Target = target ?? throw new ArgumentNullException(nameof(target));

            _attachCallback = _ => Activate();
            _detachCallback = _ => Deactivate();
            Target.RegisterCallback(_attachCallback);
            Target.RegisterCallback(_detachCallback);

            // Register only; the derived constructor performs the initial Update() once it is fully initialized.
            if (Target.panel != null)
            {
                IsActive = true;
                BindingRunner.Register(this);
            }
        }

        public bool Update()
        {
            return IsBound && Poll();
        }

        /// <summary>
        /// Applies the source to the target if it changed. Called only while <see cref="IsBound"/>.
        /// </summary>
        protected abstract bool Poll();

        private void Activate()
        {
            if (!IsBound || IsActive) return;

            IsActive = true;
            BindingRunner.Register(this);

            // The source may have changed while the element was detached.
            Update();
        }

        private void Deactivate()
        {
            if (!IsActive) return;

            IsActive = false;
            BindingRunner.Unregister(this);
        }

        public void Unbind()
        {
            if (!IsBound) return;

            Deactivate();
            IsBound = false;

            Target.UnregisterCallback(_attachCallback);
            Target.UnregisterCallback(_detachCallback);
        }

        public void Dispose()
        {
            Unbind();
        }
    }
}
