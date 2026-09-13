#nullable enable
using System;
using Platonic.Version;
using UnityEngine.UIElements;

namespace Platonic.UIToolkit
{
    /// <summary>
    /// Represents an active data binding between a VisualElement and a Platonic versioned source.
    /// </summary>
    public interface IBinding : IDisposable
    {
        /// <summary>
        /// The visual element target of this binding.
        /// </summary>
        VisualElement Target { get; }

        /// <summary>
        /// The versioned data source.
        /// </summary>
        IVersioned Source { get; }

        /// <summary>
        /// The version of the source at the time of the last update.
        /// </summary>
        ulong CachedVersion { get; }

        /// <summary>
        /// Whether the binding is currently active.
        /// </summary>
        bool IsBound { get; }

        /// <summary>
        /// Checks if the source version changed since the last update and, if so, applies the new value.
        /// Returns true if an update was applied; false if the version was unchanged or the binding is inactive.
        /// </summary>
        bool Update();

        /// <summary>
        /// Deactivates the binding, cancelling schedules and unregistering from runners.
        /// </summary>
        void Unbind();
    }

    /// <summary>
    /// Strongly-typed binding interface.
    /// </summary>
    public interface IBinding<out TElement, out TValue> : IBinding where TElement : VisualElement
    {
        new TElement Target { get; }
        new IVersionedValue<TValue> Source { get; }
    }
}
