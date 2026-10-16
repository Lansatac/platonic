#nullable enable
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Platonic.UIToolkit
{
    /// <summary>
    /// Polls every active binding once per tick. Bindings register while their element is attached to a panel
    /// and unregister when it detaches, so the runner never keeps a detached element alive.
    /// In play mode the tick comes from <see cref="BindingDriver"/> in LateUpdate; tests and tools may call
    /// <see cref="UpdateAll"/> directly. UI Toolkit is single-threaded, so this is not thread-safe.
    /// </summary>
    public static class BindingRunner
    {
        private static readonly HashSet<IBinding> _active = new();
        private static readonly List<IBinding> _snapshot = new();
        private static bool _updating;

        public static int ActiveBindingCount => _active.Count;

        public static void Register(IBinding binding)
        {
            if (binding == null) throw new ArgumentNullException(nameof(binding));
            _active.Add(binding);
        }

        public static bool Unregister(IBinding binding)
        {
            return binding != null && _active.Remove(binding);
        }

        /// <summary>
        /// Updates all active bindings and returns how many applied a change.
        /// A binding unregistered by an earlier binding's update in the same tick is skipped.
        /// </summary>
        public static int UpdateAll()
        {
            if (_updating) return 0;

            _updating = true;
            var updatedCount = 0;
            try
            {
                _snapshot.AddRange(_active);
                foreach (var binding in _snapshot)
                {
                    if (binding.IsActive && binding.Update())
                    {
                        updatedCount++;
                    }
                }
            }
            finally
            {
                _snapshot.Clear();
                _updating = false;
            }

            return updatedCount;
        }

        /// <summary>
        /// Removes all registered bindings. Intended for cleanup / test teardown.
        /// </summary>
        public static void Clear()
        {
            _active.Clear();
        }

        // Statics survive when domain reload is disabled for Enter Play Mode.
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void ResetStatics()
        {
            _active.Clear();
            _snapshot.Clear();
            _updating = false;
        }
    }
}
