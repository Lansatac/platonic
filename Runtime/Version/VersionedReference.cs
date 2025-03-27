#nullable enable
using System;

namespace Platonic.Version
{
    public class VersionedReference<T> : IVersioned
        where T : class?
    {
        private readonly Func<T>? _defaultValue;
        private T? _ref;
        private ulong _version = Versions.Initial;
        
        public T? Ref
        {
            get
            {
                if (_ref == null && _defaultValue != null)
                {
                    _ref = _defaultValue();
                }
                    
                return _ref;
            }
            set
            {
                if (_ref == value) return;
                
                _ref = value;
                _version = Version;
                Versions.Increment(ref _version);
            }
        }
        
        

        public VersionedReference()
        {
        }
        
        public VersionedReference(Func<T> defaultValue)
        {
            this._defaultValue = defaultValue;
        }

        public ulong Version => _version;
        }
    }