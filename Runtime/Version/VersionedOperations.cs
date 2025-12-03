#nullable enable
using System;
using System.Collections.Generic;
using Platonic.Collections;
using Platonic.Core;

namespace Platonic.Version
{
    public static class Versioned
    {
        public static IVersionedValue<T> AsVersioned<T>(this T value)
        {
            return new VersionedValue<T>(value);
        }
        
        public static IVersionedValue<TTarget> From<TSource, TTarget>(
            IVersionedValue<TSource> source, 
            Func<TSource, TTarget> transform)
        {
            return new TransformVersioned<TSource, TTarget>(source, transform);
        }

        public static IVersionedValue<TTarget> From<TSource1, TSource2, TTarget>(
            IVersionedValue<TSource1> source1, 
            IVersionedValue<TSource2> source2,
            Func<TSource1, TSource2, TTarget> transform)
        {
            return new Transform2Versioned<TSource1, TSource2, TTarget>(source1, source2, transform);
        }
        
        public static IVersionedValue<TTarget> From<TSource1, TSource2, TSource3, TTarget>(
            IVersionedValue<TSource1> source1, 
            IVersionedValue<TSource2> source2,
            IVersionedValue<TSource3> source3,
            Func<TSource1, TSource2, TSource3, TTarget> transform)
        {
            return new Transform3Versioned<TSource1, TSource2, TSource3, TTarget>(source1, source2, source3, transform);
        }
        
        public static IVersionedValue<TTarget> From<TSource1, TSource2, TSource3, TSource4, TTarget>(
            IVersionedValue<TSource1> source1,
            IVersionedValue<TSource2> source2,
            IVersionedValue<TSource3> source3,
            IVersionedValue<TSource4> source4,
            Func<TSource1, TSource2, TSource3, TSource4, TTarget> transform)
        {
            return new Transform4Versioned<TSource1, TSource2, TSource3, TSource4, TTarget>(source1, source2, source3, source4, transform);
        }
        
        public static IVersionedValue<TTarget> From<TSource1, TSource2, TSource3, TSource4, TSource5, TTarget>(
            IVersionedValue<TSource1> source1,
            IVersionedValue<TSource2> source2,
            IVersionedValue<TSource3> source3,
            IVersionedValue<TSource4> source4,
            IVersionedValue<TSource5> source5,
            Func<TSource1, TSource2, TSource3, TSource4, TSource5, TTarget> transform)
        {
            return new Transform5Versioned<TSource1, TSource2, TSource3, TSource4, TSource5, TTarget>(source1, source2, source3, source4, source5, transform);
        }
        
        public static IVersionedValue<TTarget> From<TSource1, TSource2, TSource3, TSource4, TSource5, TSource6, TTarget>(
            IVersionedValue<TSource1> source1,
            IVersionedValue<TSource2> source2,
            IVersionedValue<TSource3> source3,
            IVersionedValue<TSource4> source4,
            IVersionedValue<TSource5> source5,
            IVersionedValue<TSource6> source6,
            Func<TSource1, TSource2, TSource3, TSource4, TSource5, TSource6, TTarget> transform)
        {
            return new Transform6Versioned<TSource1, TSource2, TSource3, TSource4, TSource5, TSource6, TTarget>(source1, source2, source3, source4, source5, source6, transform);
        }
        
        
        public static IVersionedValue<TTarget> FromN<TSource, TTarget>(IEnumerable<IVersionedValue<TSource>> sourceFields,
            Func<IEnumerable<TSource>, TTarget> transform)
        {
            return new TransformNVersioned<TSource, TTarget>(new VersionedList<IVersionedValue<TSource>>(sourceFields), transform);
        }
        
        public static IVersionedValue<TTarget> FromN<TSource, TTarget>(
            IVersionedEnumerable<IVersionedValue<TSource>> sources, 
            Func<IEnumerable<TSource>, TTarget> transform)
        {
            return new TransformNVersioned<TSource, TTarget>(sources, transform);
        }
        
        public static IVersionedValue<TTarget> Lookup<TSource, TTarget>(IVersionedValue<TSource> sourceField,
            Func<TSource, IVersionedValue<TTarget?>?> lookup,
            TTarget defaultValue)
        {
            return new VersionedLookup<TSource, TTarget, TTarget>(sourceField, lookup, t=>t ?? defaultValue);
        }
        
        public static IVersionedValue<TFinalTarget> Lookup<TSource, TVersionedTarget, TFinalTarget>(IVersionedValue<TSource> sourceField,
            Func<TSource, IVersionedValue<TVersionedTarget>?> lookup,
            Func<TVersionedTarget?, TFinalTarget> transform)
        {
            return new VersionedLookup<TSource, TVersionedTarget, TFinalTarget>(sourceField, lookup, transform);
        }
    }
}