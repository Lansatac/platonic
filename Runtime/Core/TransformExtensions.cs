using System;

namespace Platonic.Core
{
    public static class TransformExtensions
    {
        public static TransformField<TSource, TSource> Transform<TSource>(this IData source, IFieldName<TSource> sourceName, Func<TSource, TSource> transform)
        {
            var sourceField = source.GetField(sourceName);
            return new TransformField<TSource, TSource>(sourceField, sourceName, transform);
        }
        
        public static TransformField<TSource, TTarget> Transform<TSource, TTarget>(this IData source, IFieldName<TSource> sourceName, IFieldName<TTarget> targetName, Func<TSource, TTarget> transform)
        {
            var sourceField = source.GetField(sourceName);
            return new TransformField<TSource, TTarget>(sourceField, targetName, transform);
        }
        
        // public static TransformField<TSource, TSource> Transform<TSource>(this IField<TSource> sourceField, Func<TSource, TSource> transform)
        // {
        //     return new TransformField<TSource, TSource>(sourceField, sourceField.Name, transform);
        // }
        
        public static TransformField<TSource, TSource> Transform<TSource>(this IData source, IFieldName<TSource> sourceName, IFieldName<TSource> targetName)
        {
            var sourceField = source.GetField(sourceName);
            return new TransformField<TSource, TSource>(sourceField, targetName, _=>_);
        }
        
        public static TransformField<TSource, TSource> Transform<TSource>(this IField<TSource> sourceField, IFieldName<TSource> targetName)
        {
            return new TransformField<TSource, TSource>(sourceField, targetName, _=>_);
        }
        
        public static TransformField<TSource, TSource> Transform<TSource>(this IField<TSource> sourceField, IFieldName<TSource> targetName, Func<TSource, TSource> transform)
        {
            return new TransformField<TSource, TSource>(sourceField, targetName, transform);
        }
        
        public static TransformField<TSource, TTarget> Transform<TSource, TTarget>(this IField<TSource> sourceField, IFieldName<TTarget> targetName, Func<TSource, TTarget> transform)
        {
            return new TransformField<TSource, TTarget>(sourceField, targetName, transform);
        }
        
        public static TransformField<TSource1, TSource2, TTarget> Transform<TSource1, TSource2, TTarget>(this IField<TSource1> sourceField1, IField<TSource2> sourceField2, IFieldName<TTarget> targetName, Func<TSource1, TSource2, TTarget> transform)
        {
            return new TransformField<TSource1, TSource2, TTarget>(sourceField1, sourceField2, targetName, transform);
        }
    }
}