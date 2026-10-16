#nullable enable
using System;
using Platonic.Version;
using UnityEngine.UIElements;

namespace Platonic.UIToolkit
{
    /// <summary>
    /// An active link between a VisualElement and a Platonic versioned source.
    /// Note: unrelated to <c>UnityEngine.UIElements.IBinding</c>, which belongs to Unity's SerializedObject binding.
    /// </summary>
    public interface IBinding : IDisposable
    {
        /// <summary>
        /// The visual element target of this binding.
        /// </summary>
        VisualElement Target { get; }

        /// <summary>
        /// The versioned data source, or null for a slot that has not been pointed at a source yet.
        /// </summary>
        IVersioned? Source { get; }

        /// <summary>
        /// The version of the source at the time of the last applied update.
        /// </summary>
        ulong CachedVersion { get; }

        /// <summary>
        /// False once <see cref="Unbind"/> has been called. A binding never becomes bound again.
        /// </summary>
        bool IsBound { get; }

        /// <summary>
        /// True while the target is attached to a panel. Only active bindings are polled by <see cref="BindingRunner"/>.
        /// </summary>
        bool IsActive { get; }

        /// <summary>
        /// Applies the source to the target if its version changed since the last update.
        /// Returns true if an update was applied; false if nothing changed or the binding is unbound.
        /// </summary>
        bool Update();

        /// <summary>
        /// Permanently detaches the binding from its element and from the runner.
        /// </summary>
        void Unbind();
    }

    /// <summary>
    /// Strongly-typed binding interface.
    /// </summary>
    public interface IBinding<out TElement, out TValue> : IBinding where TElement : VisualElement
    {
        new TElement Target { get; }
        new IVersionedValue<TValue>? Source { get; }
    }
}
