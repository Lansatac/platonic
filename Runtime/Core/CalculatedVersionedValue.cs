#nullable enable

using System;
using System.Collections.Generic;
using Platonic.Version;

namespace Platonic.Core
{
    //Field that derives it's value from non-versioned sources
    //and updates it's version if the value has changed.
    //Exists primarily to bridge unversioned data to versioned Data
    public class CalculatedVersionedValue<TValue> : IVersionedValue<TValue>
    {
        private readonly Func<TValue> _calculationFunc;
        private TValue _value;
        private ulong _version = Versions.Initial;
        
        public CalculatedVersionedValue(Func<TValue> calculationFunc)
        {
            _calculationFunc = calculationFunc;
            _value = calculationFunc();
        }

        public ulong Version
        {
            get
            {
                UpdateValueIfNeeded();
                return _version;
            }
        }

        public TValue Value
        {
            get
            {
                UpdateValueIfNeeded();
                return _value;
            }
        }
        
        private void UpdateValueIfNeeded()
        {
            TValue newValue = _calculationFunc();
            if (!EqualityComparer<TValue>.Default.Equals(newValue, _value))
            {
                _value = newValue;
                _version++;
            }
        }

        public override string ToString()
        {
            return $"{Value}";
        }
    }
}