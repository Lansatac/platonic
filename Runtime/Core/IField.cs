﻿#nullable enable
using Platonic.Core;
using Platonic.Version;

namespace Platonic
{
    public interface IField: IVersioned
    {
        IFieldName Name { get; }
        object? Value { get; }
    }
}

namespace Platonic.Core
{
    public interface IField<out T> : IField
    {
        new IFieldName<T> Name { get; }
        new T Value { get; }
    }
}