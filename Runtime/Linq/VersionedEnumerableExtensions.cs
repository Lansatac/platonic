#nullable enable

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Platonic.Collections;

namespace Platonic.Linq
{
    public static class VersionedEnumerableExtensions
    {
        public static IVersionedEnumerable<TTarget> VersionedSelect<TSource, TTarget>(this IVersionedEnumerable<TSource> source, Func<TSource, TTarget> selector)
        {
            return new VersionedSelector<TSource, TTarget>(source, selector);
        }

        public static IVersionedEnumerable<TSource> VersionedWhere<TSource>(this IVersionedEnumerable<TSource> source, Func<TSource, bool> predicate)
        {
            return new VersionedFilter<TSource>(source, predicate);
        }
        
        public static IVersionedEnumerable<TSource> VersionedConcat<TSource>(this IVersionedEnumerable<TSource> first, IVersionedEnumerable<TSource> second)
        {
            return new VersionedConcatenator<TSource>(first, second);
        }


        private abstract class CachedVersionedEnumerable<T> : IVersionedEnumerable<T>
        {
            private List<T>? _cache;
            private ulong _cachedVersion;

            public IEnumerator<T> GetEnumerator()
            {
                _cache ??= new List<T>();
                
                var currentVersion = Version;
                if (currentVersion != _cachedVersion)
                {
                    _cache.Clear();
                    CacheResults(_cache);
                    _cachedVersion = currentVersion;
                }
                return _cache.GetEnumerator();
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return GetEnumerator();
            }

            public abstract ulong Version { get; }
            public IVersionedEnumerable<T> Value => this;

            protected abstract void CacheResults(List<T> cache);
        }

        private class VersionedSelector<TSource, TTarget> : CachedVersionedEnumerable<TTarget>
        {
            private readonly IVersionedEnumerable<TSource> _source;
            private readonly Func<TSource, TTarget> _selector;

            public VersionedSelector(IVersionedEnumerable<TSource> source, Func<TSource, TTarget> selector)
            {
                _source = source;
                _selector = selector;
            }

            public override ulong Version => _source.Version;

            protected override void CacheResults(List<TTarget> cache)
            {
                cache.AddRange(_source.Select(_selector));
            }
        }

        private class VersionedFilter<TSource> : CachedVersionedEnumerable<TSource>
        {
            private readonly IVersionedEnumerable<TSource> _source;
            private readonly Func<TSource, bool> _predicate;

            public VersionedFilter(IVersionedEnumerable<TSource> source, Func<TSource, bool> predicate)
            {
                _source = source;
                _predicate = predicate;
            }

            public override ulong Version => _source.Version;

            protected override void CacheResults(List<TSource> cache)
            {
                cache.AddRange(_source.Where(_predicate));
            }
        }

        private class VersionedConcatenator<TSource> : CachedVersionedEnumerable<TSource>
        {
            private readonly IVersionedEnumerable<TSource> _first;
            private readonly IVersionedEnumerable<TSource> _second;

            public VersionedConcatenator(IVersionedEnumerable<TSource> first, IVersionedEnumerable<TSource> second)
            {
                _first = first;
                _second = second;
            }

            public override ulong Version => unchecked(_first.Version + _second.Version);

            protected override void CacheResults(List<TSource> cache)
            {
                cache.AddRange(_first.Concat(_second));
            }
        }
    }
}