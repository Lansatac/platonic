#nullable enable
using System;
using Platonic.Version;

namespace Platonic.Core
{
    public static partial class FieldOperations
    {
        public static IField<TSource> BorrowedFrom<TSource>(this IFieldName<TSource> sourceName, IData source)
        {
            return source.GetField(sourceName);
        }

        public static IField<TSource> BorrowedFrom<TSource>(this IFieldName<TSource> sourceName, IData source,
            Func<TSource, TSource> transform)
        {
            var sourceField = source.GetField(sourceName);
            return new TransformField<TSource, TSource>(sourceField, sourceName, transform);
        }

        public static IField<TTarget> BorrowedAs<TSource, TTarget>(this IFieldName<TTarget> targetName,
            IData source, IFieldName<TSource> sourceName, Func<TSource, TTarget> transform)
        {
            var sourceField = source.GetField(sourceName);
            return new TransformField<TSource, TTarget>(sourceField, targetName, transform);
        }

        public static IField<TSource> BorrowedAs<TSource>(this IFieldName<TSource> targetName,
            IData source, IFieldName<TSource> sourceName)
        {
            var sourceField = source.GetField(sourceName);
            return new TransformField<TSource, TSource>(sourceField, targetName, val => val);
        }

        public static IField<TSource> RenameAs<TSource>(this IField<TSource> sourceField,
            IFieldName<TSource> targetName)
        {
            return new TransformField<TSource, TSource>(sourceField, targetName, source => source);
        }

        public static IField<TTarget> From<TSource, TTarget>(this IFieldName<TTarget> targetName,
            IField<TSource> sourceField, Func<TSource, TTarget> transform)
        {
            return new TransformField<TSource, TTarget>(sourceField, targetName, transform);
        }

        public static IField<TTarget> From<TSource1, TSource2, TTarget>(
            this IFieldName<TTarget> targetName, IField<TSource1> sourceField1, IField<TSource2> sourceField2,
            Func<TSource1, TSource2, TTarget> transform)
        {
            return new Transform2Fields<TSource1, TSource2, TTarget>(sourceField1, sourceField2, targetName, transform);
        }

        public static IField<TTarget> From<TSource1, TSource2, TSource3,
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

        public static IField<TTarget> From<TSource1, TSource2,
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

        public static IField<TTarget> From<TSource1, TSource2,
            TSource3, TSource4, TSource5, TTarget>(
            this IFieldName<TTarget> targetName,
            IField<TSource1> sourceField1,
            IField<TSource2> sourceField2,
            IField<TSource3> sourceField3,
            IField<TSource4> sourceField4,
            IField<TSource5> sourceField5,
            Func<TSource1, TSource2, TSource3, TSource4, TSource5, TTarget> transform)
        {
            return new Transform5Fields<TSource1, TSource2, TSource3, TSource4, TSource5, TTarget>(
                sourceField1, sourceField2, sourceField3, sourceField4, sourceField5, targetName, transform);
        }

        public static IField<TTarget> From<TSource1, TSource2,
            TSource3, TSource4, TSource5, TSource6, TTarget>(
            this IFieldName<TTarget> targetName,
            IField<TSource1> sourceField1,
            IField<TSource2> sourceField2,
            IField<TSource3> sourceField3,
            IField<TSource4> sourceField4,
            IField<TSource5> sourceField5,
            IField<TSource6> sourceField6,
            Func<TSource1, TSource2, TSource3, TSource4, TSource5, TSource6, TTarget> transform)
        {
            return new Transform6Fields<TSource1, TSource2, TSource3, TSource4, TSource5, TSource6, TTarget>(
                sourceField1, sourceField2, sourceField3, sourceField4, sourceField5, sourceField6, targetName,
                transform);
        }

        public static IField<TTarget> FromVersioned<TSource, TTarget>(
            this IFieldName<TTarget> targetName,
            TSource source, Func<TSource,
                TTarget> transform)
            where TSource : IVersioned
        {
            return new TransformVersioned<TSource, TTarget>(source, targetName, transform);
        }
    }
}