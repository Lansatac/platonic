#nullable enable
using System;

namespace Platonic.Core
{
    public static partial class FieldOperations
    {
        public static TransformField<TSource, TSource> Borrow<TSource>(this IData source,
            IFieldName<TSource> sourceName, Func<TSource, TSource> transform)
        {
            var sourceField = source.GetField(sourceName);
            return new TransformField<TSource, TSource>(sourceField, sourceName, transform);
        }

        public static TransformField<TSource, TTarget> BorrowFieldAs<TSource, TTarget>(this IData source,
            IFieldName<TSource> sourceName, IFieldName<TTarget> targetName, Func<TSource, TTarget> transform)
        {
            var sourceField = source.GetField(sourceName);
            return new TransformField<TSource, TTarget>(sourceField, targetName, transform);
        }

        public static TransformField<TSource, TSource> BorrowFieldAs<TSource>(this IData source,
            IFieldName<TSource> sourceName, IFieldName<TSource> targetName)
        {
            var sourceField = source.GetField(sourceName);
            return new TransformField<TSource, TSource>(sourceField, targetName, val => val);
        }

        public static TransformField<TSource, TSource> Rename<TSource>(this IField<TSource> sourceField,
            IFieldName<TSource> targetName)
        {
            return new TransformField<TSource, TSource>(sourceField, targetName, source => source);
        }

        public static TransformField<TSource, TSource> From<TSource>(this IFieldName<TSource> targetName,
            IField<TSource> sourceField, Func<TSource, TSource> transform)
        {
            return new TransformField<TSource, TSource>(sourceField, targetName, transform);
        }

        public static TransformField<TSource, TTarget> From<TSource, TTarget>(this IFieldName<TTarget> targetName,
            IField<TSource> sourceField, Func<TSource, TTarget> transform)
        {
            return new TransformField<TSource, TTarget>(sourceField, targetName, transform);
        }

        public static Transform2Fields<TSource1, TSource2, TTarget> From<TSource1, TSource2, TTarget>(
            this IFieldName<TTarget> targetName, IField<TSource1> sourceField1, IField<TSource2> sourceField2,
            Func<TSource1, TSource2, TTarget> transform)
        {
            return new Transform2Fields<TSource1, TSource2, TTarget>(sourceField1, sourceField2, targetName, transform);
        }

        public static Transform3Fields<TSource1, TSource2, TSource3, TTarget> From<TSource1, TSource2, TSource3,
            TTarget>(
            this IFieldName<TTarget> targetName,
            IField<TSource1> sourceField1,
            IField<TSource2> sourceField2,
            IField<TSource3> sourceField3,
            Func<TSource1, TSource2, TSource3, TTarget> transform)
        {
            return new Transform3Fields<TSource1, TSource2, TSource3, TTarget>(sourceField1, sourceField2, sourceField3,
                targetName, transform);
        }

        public static Transform4Fields<TSource1, TSource2, TSource3, TSource4, TTarget> From<TSource1, TSource2,
            TSource3,
            TSource4, TTarget>(
            this IFieldName<TTarget> targetName,
            IField<TSource1> sourceField1,
            IField<TSource2> sourceField2,
            IField<TSource3> sourceField3,
            IField<TSource4> sourceField4,
            Func<TSource1, TSource2, TSource3, TSource4, TTarget> transform)
        {
            return new Transform4Fields<TSource1, TSource2, TSource3, TSource4, TTarget>(sourceField1, sourceField2,
                sourceField3,
                sourceField4, targetName, transform);
        }
    }
}