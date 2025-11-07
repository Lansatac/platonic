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
            return new VersionedLookup<TSource, TTarget>(sourceField, lookup, defaultValue)
                .RenameAs(targetName);
        }

        public static IField<TTarget> Lookup<TSource1, TSource2, TTarget>(this IFieldName<TTarget> targetName,
            IField<TSource1> sourceField1, IField<TSource2> sourceField2,
            Func<TSource1, TSource2, IField<TTarget>?> lookup, TTarget defaultValue)
        {
            return new VersionedLookup2<TSource1, TSource2, TTarget>(sourceField1, sourceField2, lookup,
                defaultValue).RenameAs(targetName);
        }

        public static IField<TTarget> LookupField<TTarget>(this IFieldName<TTarget> targetName,
            IField<IData?> sourceField, TTarget defaultValue)
        {
            return new VersionedLookup<IData?, TTarget>(sourceField,
                    source => source?.GetField(targetName), defaultValue)
                .RenameAs(targetName);
        }


        public static IField<TTarget> LookupField<TSource, TTarget>(this IFieldName<TTarget> targetName,
            IField<TSource> sourceField, Func<TSource, IData?> lookup, TTarget defaultValue)
        {
            return new VersionedLookup<TSource, TTarget>(sourceField, (source) => lookup(source)?.GetField(targetName),
                defaultValue).RenameAs(targetName);
        }

        public static IField<bool> Lookup<TSource>(this IFieldName<bool> targetName,
            IField<TSource> sourceField, Func<TSource, IData?> lookup)
        {
            return targetName.From(sourceField,
                (source) => lookup(source) != null);
        }


        public static IField<TTarget> Lookup<TSource, TTarget>(this IFieldName<TTarget> targetName,
            IField<TSource> source, Func<TSource, TTarget?> lookup, TTarget defaultValue)
        {
            return new Lookup<TSource, TTarget>(source, lookup, defaultValue).RenameAs(targetName);
        }
    }
}