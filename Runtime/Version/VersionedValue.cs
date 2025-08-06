namespace Platonic.Version
{
    public class VersionedValue<TValue> : IVersionedValue<TValue>
    {
        public VersionedValue(TValue value)
        {
            Value = value;
        }

        public ulong Version => Versions.Initial;
        public TValue Value { get; }
    }
}