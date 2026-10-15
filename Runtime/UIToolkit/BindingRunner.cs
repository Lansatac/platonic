#nullable enable
using System;
using System.Collections.Generic;

namespace Platonic.UIToolkit
{
    /// <summary>
    /// Centralized runner that manages and updates registered UI Toolkit bindings.
    /// Can be driven manually, via test harnesses, or automatically through engine update loops.
    /// </summary>
    public static class BindingRunner
    {
        private static readonly List<IBinding> _bindings = new();
        private static readonly List<IBinding> _bindingsToUpdate = new();

        /// <summary>
        /// Gets the current number of active bindings registered with the runner.
        /// </summary>
        public static int ActiveBindingCount
        {
            get
            {
                lock (_bindings)
                {
                    return _bindings.Count;
                }
            }
        }

        /// <summary>
        /// Registers a binding to receive updates from the centralized runner.
        /// </summary>
        public static void Register(IBinding binding)
        {
            if (binding == null) throw new ArgumentNullException(nameof(binding));

            lock (_bindings)
            {
                if (!_bindings.Contains(binding))
                {
                    _bindings.Add(binding);
                }
            }
        }

        /// <summary>
        /// Unregisters a binding from the centralized runner.
        /// </summary>
        public static bool Unregister(IBinding binding)
        {
            if (binding == null) return false;

            lock (_bindings)
            {
                return _bindings.Remove(binding);
            }
        }

        /// <summary>
        /// Updates all registered bindings.
        /// Returns the number of bindings that performed a mutation.
        /// </summary>
        public static int UpdateAll()
        {
            _bindingsToUpdate.Clear();

            lock (_bindings)
            {
                // Prune any disposed / unbound bindings
                for (var i = _bindings.Count - 1; i >= 0; i--)
                {
                    if (!_bindings[i].IsBound)
                    {
                        _bindings.RemoveAt(i);
                    }
                }

                _bindingsToUpdate.AddRange(_bindings);
            }

            var updatedCount = 0;
            foreach (var binding in _bindingsToUpdate)
            {
                if (binding.IsBound && binding.Update())
                {
                    updatedCount++;
                }
            }

            _bindingsToUpdate.Clear();
            return updatedCount;
        }

        /// <summary>
        /// Removes all registered bindings. Intended for cleanup / test teardown.
        /// </summary>
        public static void Clear()
        {
            lock (_bindings)
            {
                _bindings.Clear();
            }
            _bindingsToUpdate.Clear();
        }
    }
}
