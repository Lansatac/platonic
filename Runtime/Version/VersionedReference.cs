#nullable enable
using System;

namespace Platonic.Version
{
    public class VersionedReference<T> : IVersionedValue<T>
        where T : class?
    {
        private readonly Func<T> _defaultValue;
        private T? _ref;
        private ulong _version = Versions.Initial;
        
        public T Ref
        {
            get
            {
                return _ref ??= _defaultValue();
            }
            set
            {
                if (_ref == value) return;
                
                _ref = value;
                _version = Version;
                Versions.Increment(ref _version);
            }
        }
        
        public VersionedReference(Func<T> defaultValue)
        {
            _defaultValue = defaultValue;
        }

        public ulong Version => _version;
        public T Value => Ref;
    }
    }