#nullable enable
using System;
using Platonic.Version;

namespace Platonic.Core
{
    public static class LookupExtensions
    {
        public static IField<TTarget> Lookup<TSource, TTarget>(this IFieldName<TTarget> targetName,
            IVersionedValue<TSource> sourceField, Func<TSource, IVersionedValue<TTarget>?> lookup, TTarget defaultValue)
        {
            return new VersionedLookup<TSource, TTarget, TTarget>(sourceField, lookup, t => t ?? defaultValue)
                .RenameAs(targetName);
        }
        
        public static IField<TFinalTarget> Lookup<TSource, TVersionedTarget, TFinalTarget>(this IFieldName<TFinalTarget> targetName,
            IVersionedValue<TSource> sourceField, Func<TSource, IVersionedValue<TVersionedTarget>?> lookup, Func<TVersionedTarget?, TFinalTarget> transform)
        {
            return new VersionedLookup<TSource, TVersionedTarget, TFinalTarget>(sourceField, lookup, transform)
                .RenameAs(targetName);
        }

        public static IField<TTarget> Lookup<TSource1, TSource2, TTarget>(this IFieldName<TTarget> targetName,
            IVersionedValue<TSource1> sourceField1, IVersionedValue<TSource2> sourceField2,
            Func<TSource1, TSource2, IVersionedValue<TTarget>?> lookup, TTarget defaultValue)
        {
            return new VersionedLookup2<TSource1, TSource2, TTarget, TTarget>(sourceField1, sourceField2, lookup,
                t => t ?? defaultValue).RenameAs(targetName);
        }

        public static IField<TTarget> LookupField<TTarget>(this IFieldName<TTarget> targetName,
            IVersionedValue<IData?> sourceField, TTarget defaultValue)
        {
            return new VersionedLookup<IData?, TTarget, TTarget>(sourceField,
                    source => source?.GetField(targetName), t => t ?? defaultValue)
                .RenameAs(targetName);
        }


        public static IField<TTarget> LookupField<TSource, TTarget>(this IFieldName<TTarget> targetName,
            IVersionedValue<TSource> sourceField, Func<TSource, IData?> lookup, TTarget defaultValue)
        {
            return new VersionedLookup<TSource, TTarget, TTarget>(sourceField,
                (source) => lookup(source)?.GetField(targetName), t => t ?? defaultValue).RenameAs(targetName);
        }
    }
}