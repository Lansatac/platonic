#nullable enable
using System;
using Platonic.Version;

namespace Platonic.Core
{
    public static class LookupExtensions
    {
        public static FieldLookup<TSource, TTarget> Lookup<TSource, TTarget>(this IFieldName<TTarget> targetName,
            IField<TSource> sourceField, Func<TSource, IField<TTarget>?> lookup, TTarget defaultValue)
        {
            return new FieldLookup<TSource, TTarget>(sourceField, targetName, lookup, defaultValue);
        }
        
        public static IField<TTarget> Lookup<TSource1, TSource2, TTarget>(this IFieldName<TTarget> targetName,
            IField<TSource1> sourceField1, IField<TSource2> sourceField2, Func<TSource1, TSource2, IField<TTarget>?> lookup, TTarget defaultValue)
        {
            return new FieldLookup2Fields<TSource1, TSource2, TTarget>(sourceField1, sourceField2, targetName, lookup, defaultValue);
        }

        public static IField<TTarget> LookupField<TTarget>(this IFieldName<TTarget> targetName,
            IField<IData?> sourceField, TTarget defaultValue)
        {
            return new FieldLookup<IData?, TTarget>(sourceField, targetName, source=> source?.GetField(targetName), defaultValue);
        }
        
        
        public static FieldLookup<TSource, TTarget> LookupField<TSource, TTarget>(this IFieldName<TTarget> targetName,
            IField<TSource> sourceField, Func<TSource, IData?> lookup, TTarget defaultValue)
        {
            return new FieldLookup<TSource, TTarget>(sourceField, targetName,
                (source) => lookup(source)?.GetField(targetName), defaultValue);
        }

        public static IField<bool> Lookup<TSource>(this IFieldName<bool> targetName,
            IField<TSource> sourceField, Func<TSource, IData?> lookup)
        {
            return targetName.From(sourceField,
                (source) => lookup(source) != null);
        }


        public static Lookup<TSource, TTarget> Lookup<TSource, TTarget>(this IFieldName<TTarget> targetName,
            IField<TSource> source, Func<TSource, TTarget?> lookup, TTarget defaultValue)
        {
            return new Lookup<TSource, TTarget>(source, targetName, lookup, defaultValue);
        }
    }
}