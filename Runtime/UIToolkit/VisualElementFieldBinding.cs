#nullable enable
using System;
using Platonic.Version;
using UnityEngine.UIElements;

namespace Platonic.UIToolkit
{
    /// <summary>
    /// Applies an IVersionedValue to a VisualElement whenever the source's version changes.
    /// The source can be replaced with <see cref="Rebind"/>, so one binding can serve a recycled element
    /// (for example a ListView row) for its whole life instead of being recreated on every rebind.
    /// </summary>
    public class VisualElementFieldBinding<TElement, TValue> : ElementBinding<TElement>, IBinding<TElement, TValue>
        where TElement : VisualElement
    {
        private readonly Action<TElement, TValue> _apply;
        private bool _hasApplied;

        public IVersionedValue<TValue>? Source { get; private set; }

        protected override IVersioned? SourceCore => Source;

        public IField<TValue>? Field => Source as IField<TValue>;

        /// <param name="source">The initial source. Null creates an unbound slot to be pointed at a source with <see cref="Rebind"/>.</param>
        public VisualElementFieldBinding(
            TElement target,
            IVersionedValue<TValue>? source,
            Action<TElement, TValue> apply)
            : base(target)
        {
            _apply = apply ?? throw new ArgumentNullException(nameof(apply));
            Source = source;

            Update();
        }

        /// <summary>
        /// Points the binding at a different source (or null to stop updating) and applies it immediately.
        /// The previously applied value stays on the element until the new source applies.
        /// </summary>
        public void Rebind(IVersionedValue<TValue>? source)
        {
            Source = source;
            _hasApplied = false;
            CachedVersion = Versions.None;

            Update();
        }

        protected override bool Poll()
        {
            var source = Source;
            if (source == null) return false;

            var version = source.Version;
            if (_hasApplied && version == CachedVersion) return false;

            _hasApplied = true;
            CachedVersion = version;
            _apply(Target, source.Value);
            return true;
        }
    }
}
