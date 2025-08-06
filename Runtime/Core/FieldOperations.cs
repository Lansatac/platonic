#nullable enable
using System;
using System.Collections.Generic;
using Platonic.Version;
using UnityEngine;

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
            return new VersionedField<TSource>(sourceName, Versioned.From(sourceField, transform));
        }

        public static IField<TTarget> BorrowedAs<TSource, TTarget>(this IFieldName<TTarget> targetName,
            IData source, IFieldName<TSource> sourceName, Func<TSource, TTarget> transform)
        {
            var sourceField = source.GetField(sourceName);
            return new VersionedField<TTarget>(targetName, Versioned.From(sourceField, transform));
        }

        public static IField<TSource> BorrowedAs<TSource>(this IFieldName<TSource> targetName,
            IData source, IFieldName<TSource> sourceName)
        {
            var sourceField = source.GetField(sourceName);
            return new VersionedField<TSource>(targetName, sourceField);
        }

        public static IField<TSource> RenameAs<TSource>(this IVersionedValue<TSource> sourceField,
            IFieldName<TSource> targetName)
        {
            return new VersionedField<TSource>(targetName, sourceField);
        }

        public static IField<TTarget> From<TSource, TTarget>(this IFieldName<TTarget> targetName,
            IVersionedValue<TSource> sourceField, Func<TSource, TTarget> transform)
        {
            return new VersionedField<TTarget>(targetName, Versioned.From(sourceField, transform));
        }

        public static IField<TTarget> From<TSource1, TSource2, TTarget>(
            this IFieldName<TTarget> targetName, IVersionedValue<TSource1> sourceField1,
            IVersionedValue<TSource2> sourceField2,
            Func<TSource1, TSource2, TTarget> transform)
        {
            return new VersionedField<TTarget>(targetName, Versioned.From(sourceField1, sourceField2, transform));
        }

        public static IField<TTarget> From<TSource1, TSource2, TSource3,
            TTarget>(
            this IFieldName<TTarget> targetName,
            IVersionedValue<TSource1> sourceField1,
            IVersionedValue<TSource2> sourceField2,
            IVersionedValue<TSource3> sourceField3,
            Func<TSource1, TSource2, TSource3, TTarget> transform)
        {
            return new VersionedField<TTarget>(targetName, Versioned.From(sourceField1, sourceField2, sourceField3, transform));
        }

        public static IField<TTarget> From<TSource1, TSource2,
            TSource3,
            TSource4, TTarget>(
            this IFieldName<TTarget> targetName,
            IVersionedValue<TSource1> sourceField1,
            IVersionedValue<TSource2> sourceField2,
            IVersionedValue<TSource3> sourceField3,
            IVersionedValue<TSource4> sourceField4,
            Func<TSource1, TSource2, TSource3, TSource4, TTarget> transform)
        {
            return new VersionedField<TTarget>(targetName, Versioned.From(sourceField1, sourceField2, sourceField3, sourceField4, transform));
        }

        public static IField<TTarget> From<TSource1, TSource2,
            TSource3, TSource4, TSource5, TTarget>(
            this IFieldName<TTarget> targetName,
            IVersionedValue<TSource1> sourceField1,
            IVersionedValue<TSource2> sourceField2,
            IVersionedValue<TSource3> sourceField3,
            IVersionedValue<TSource4> sourceField4,
            IVersionedValue<TSource5> sourceField5,
            Func<TSource1, TSource2, TSource3, TSource4, TSource5, TTarget> transform)
        {
            return new VersionedField<TTarget>(targetName, Versioned.From(sourceField1, sourceField2, sourceField3, sourceField4, sourceField5, transform));
        }

        public static IField<TTarget> From<TSource1, TSource2,
            TSource3, TSource4, TSource5, TSource6, TTarget>(
            this IFieldName<TTarget> targetName,
            IVersionedValue<TSource1> sourceField1,
            IVersionedValue<TSource2> sourceField2,
            IVersionedValue<TSource3> sourceField3,
            IVersionedValue<TSource4> sourceField4,
            IVersionedValue<TSource5> sourceField5,
            IVersionedValue<TSource6> sourceField6,
            Func<TSource1, TSource2, TSource3, TSource4, TSource5, TSource6, TTarget> transform)
        {
            return new VersionedField<TTarget>(targetName, Versioned.From(sourceField1, sourceField2, sourceField3, sourceField4, sourceField5, sourceField6, transform));
        }

        public static IField<TTarget> FromN<TSource, TTarget>(this IFieldName<TTarget> targetName,
            IEnumerable<IVersionedValue<TSource>> sourceFields, Func<IEnumerable<TSource>, TTarget> transform)
        {
            return new VersionedField<TTarget>(targetName,
                Versioned.FromN(sourceFields.AsVersioned(), transform));
        }

        public static IField<TTarget> FromN<TSource, TTarget>(this IFieldName<TTarget> targetName,
            IVersionedValue<IEnumerable<IVersionedValue<TSource>>> sourceFields,
            Func<IEnumerable<TSource>, TTarget> transform)
        {
            return new VersionedField<TTarget>(targetName, Versioned.FromN(sourceFields, transform));
        }
    }
}