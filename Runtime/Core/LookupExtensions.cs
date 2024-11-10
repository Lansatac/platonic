using System;

namespace Platonic.Core
{
    public static class LookupExtensions
    {
        public static FieldLookup<TSource, TTarget> Lookup<TSource, TTarget>(this IField<TSource> sourceField, IFieldName<TTarget> targetName, Func<TSource, IField<TTarget>> lookup)
        {
            return new FieldLookup<TSource, TTarget>(sourceField, targetName, lookup);
        }
        
        public static FieldLookup<TSource, TTarget> Lookup<TSource, TTarget>(this IField<TSource> sourceField, IFieldName<TTarget> targetName, Func<TSource, IData> lookup)
        {
            return new FieldLookup<TSource, TTarget>(sourceField, targetName, (source) => lookup(source).GetField(targetName));
        }
    }
}