namespace Platonic.Version
{
    public class VersionedValue<TValue> : IVersionedValue<TValue>
    {
        //For serialization
        private VersionedValue()
        {
            _value = default;
        }
        
        public VersionedValue(TValue value)
        {
            _value = value;
        }

        public ulong Version { get; private set; } = Versions.Initial;

        private TValue _value;
        public TValue Value
        {
            get => _value;
            set
            {
                if (Equals(_value, value)) return;
                
                _value = value;
                Version += 1;
            }
        }
        
        public void ForceVersionIncrement() => Version += 1;
    }
}