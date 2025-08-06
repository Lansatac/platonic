#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;
using Platonic.Version;

namespace Platonic.Core
{
    public class VersionedField<TValue> : IField<TValue>
    {
        private readonly IVersionedValue<TValue> _versionedValue;

        public VersionedField(IFieldName<TValue> name, IVersionedValue<TValue> value)
        {
            Name = name;
            _versionedValue = value;
        }

        public ulong Version => _versionedValue.Version;
        IFieldName IField.Name => Name;

        public IFieldName<TValue> Name { get; }

        object? IField.Value => Value;

        public TValue Value => _versionedValue.Value;

        public override string ToString()
        {
            return $"{Name.Name}: {Value}";
        }
    }
}