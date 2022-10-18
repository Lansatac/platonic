using System;

namespace Platonic.Version
{
    /// <summary>
    /// Interface for objects that can change value.
    /// If the version is different, the value will be too.
    /// Conversely, if the version is unchanged, the value has not.
    /// </summary>
    public interface IVersioned
    {
        ulong Version { get; }
    }

    public static class Versions
    {
        public const ulong None = ulong.MinValue;
        public const ulong Initial = 1ul;

        public static void Increment(ref ulong version)
        {
            version += 1ul;
        }
    }
}