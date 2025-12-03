#nullable enable

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Platonic.Collections;
using Platonic.Version;

namespace Platonic.Linq
{
    public static class VersionedEnumerableExtensions
    {
        public static IVersionedEnumerable<TTarget> VersionedSelect<TSource, TTarget>(
            this IVersionedEnumerable<TSource> source, Func<TSource, TTarget> selector)
        {
            return new VersionedSelector<TSource, TTarget>(source, selector);
        }

        public static IVersionedEnumerable<TSource> VersionedWhere<TSource>(this IVersionedEnumerable<TSource> source,
            Func<TSource, bool> predicate)
        {
            return new VersionedFilter<TSource>(source, predicate);
        }

        public static IVersionedEnumerable<TSource> VersionedConcat<TSource>(this IVersionedEnumerable<TSource> first,
            IVersionedEnumerable<TSource> second)
        {
            return new VersionedConcatenator<TSource>(first, second);
        }

        public static IVersionedEnumerable<TSource> ToVersionedEnumerable<TSource>(
            this IVersionedValue<IEnumerable<TSource>> source)
        {
            return new VersionedValueAdapter<TSource>(source);
        }

        private interface ICachingVersionedEnumerable<out T> : IVersionedEnumerable<T>
        {
            IEnumerable<T> Uncached { get; }
        }

        private abstract class CachedVersionedEnumerable<T> : ICachingVersionedEnumerable<T>
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
                    _cache.AddRange(Uncached);
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

            public IEnumerable<T> Uncached => GetUncached();
            protected abstract IEnumerable<T> GetUncached();
        }

        private class VersionedSelector<TSource, TTarget> : CachedVersionedEnumerable<TTarget>
        {
            private readonly IVersionedEnumerable<TSource> _source;
            private readonly IEnumerable<TSource> _uncachedSource;
            private readonly Func<TSource, TTarget> _selector;

            public VersionedSelector(IVersionedEnumerable<TSource> source, Func<TSource, TTarget> selector)
            {
                _source = source;
                _uncachedSource = source is ICachingVersionedEnumerable<TSource> cachingSource ? cachingSource.Uncached : source;
                _selector = selector;
            }

            public override ulong Version => _source.Version;

            protected override IEnumerable<TTarget> GetUncached()
            {
                return _uncachedSource.Select(_selector);
            }
        }

        private class VersionedFilter<TSource> : CachedVersionedEnumerable<TSource>
        {
            private readonly IVersionedEnumerable<TSource> _source;
            private readonly IEnumerable<TSource> _uncachedSource;
            private readonly Func<TSource, bool> _predicate;

            public VersionedFilter(IVersionedEnumerable<TSource> source, Func<TSource, bool> predicate)
            {
                _source = source;
                _uncachedSource = source is ICachingVersionedEnumerable<TSource> cachingSource ? cachingSource.Uncached : source;
                _predicate = predicate;
            }

            public override ulong Version => _source.Version;

            protected override IEnumerable<TSource> GetUncached()
            {
                return _uncachedSource.Where(_predicate);
            }
        }

        private class VersionedConcatenator<TSource> : CachedVersionedEnumerable<TSource>
        {
            private readonly IVersionedEnumerable<TSource> _first;
            private readonly IEnumerable<TSource> _firstUncachedSource;
            private readonly IVersionedEnumerable<TSource> _second;
            private readonly IEnumerable<TSource> _secondUncachedSource;

            public VersionedConcatenator(IVersionedEnumerable<TSource> first, IVersionedEnumerable<TSource> second)
            {
                _first = first;
                _firstUncachedSource = _first is ICachingVersionedEnumerable<TSource> firstCachingSource ? firstCachingSource.Uncached : _first;
                _second = second;
                _secondUncachedSource = _second is ICachingVersionedEnumerable<TSource> secondCachingSource ? secondCachingSource.Uncached : _second;
            }

            public override ulong Version => unchecked(_first.Version + _second.Version);

            protected override IEnumerable<TSource> GetUncached()
            {
                return _firstUncachedSource.Concat(_secondUncachedSource);
            }
        }

        private class VersionedValueAdapter<TSource> : IVersionedEnumerable<TSource>
        {
            private readonly IVersionedValue<IEnumerable<TSource>> _source;

            public VersionedValueAdapter(IVersionedValue<IEnumerable<TSource>> source)
            {
                _source = source;
            }

            public IEnumerator<TSource> GetEnumerator()
            {
                return _source.Value.GetEnumerator();
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return GetEnumerator();
            }

            public ulong Version => _source.Version;
            public IVersionedEnumerable<TSource> Value => this;
        }
    }
}