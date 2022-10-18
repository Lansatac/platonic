namespace Platonic.Version
{
    public class VersionedReference<T> : IVersioned
        where T : class
    {
        private T? _ref;
        private ulong _version = Versions.Initial;

        public T? Ref
        {
            get => _ref;
            set
            {
                if (_ref == value) return;
                
                _ref = value;
                _version = Version;
                Versions.Increment(ref _version);
            }
        }

        public ulong Version => _version;
    }
}