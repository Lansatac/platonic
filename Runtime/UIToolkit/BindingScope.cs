#nullable enable
using System;
using System.Collections.Generic;

namespace Platonic.UIToolkit
{
    /// <summary>
    /// Owns a group of bindings so a screen can end them all at once. Bindings are already released when their
    /// element is detached and collected; use a scope when the element outlives the data it was bound to.
    /// </summary>
    public sealed class BindingScope : IDisposable
    {
        private readonly List<IBinding> _bindings = new();
        private bool _disposed;

        public TBinding Add<TBinding>(TBinding binding) where TBinding : IBinding
        {
            if (binding == null) throw new ArgumentNullException(nameof(binding));

            if (_disposed)
            {
                binding.Unbind();
            }
            else
            {
                _bindings.Add(binding);
            }

            return binding;
        }

        public void Dispose()
        {
            _disposed = true;
            foreach (var binding in _bindings)
            {
                binding.Unbind();
            }

            _bindings.Clear();
        }
    }
}
