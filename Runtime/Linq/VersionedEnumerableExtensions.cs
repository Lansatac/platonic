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

        public static IVersionedEnumerable<TResult> VersionedSelectMany<TSource, TResult>(
            this IVersionedEnumerable<TSource> source, Func<TSource, IEnumerable<TResult>> selector)
        {
            return new VersionedSelectManyImpl<TSource, TResult>(source, selector);
        }

        public static IVersionedEnumerable<TSource> VersionedWhere<TSource>(
            this IVersionedEnumerable<TSource> source,
            Func<TSource, bool> predicate)
        {
            return new VersionedFilter<TSource>(source, predicate);
        }

        public static IVersionedEnumerable<TSource> VersionedConcat<TSource>(
            this IVersionedEnumerable<TSource> first,
            IVersionedEnumerable<TSource> second)
        {
            return new VersionedConcatenator<TSource>(first, second);
        }

        public static IVersionedOrderedEnumerable<TSource> VersionedOrderBy<TSource, TKey>(
            this IVersionedEnumerable<TSource> source,
            Func<TSource, TKey> keySelector)
            where TKey : IComparable<TKey>
        {
            return new VersionedOrderedEnumerable<TSource>(source)
                .AddLevel(x => keySelector(x), false);
        }

        public static IVersionedOrderedEnumerable<TSource> VersionedOrderBy<TSource, TKey>(
            this IVersionedEnumerable<TSource> source,
            Func<TSource, IVersionedValue<TKey>> keySelector)
            where TKey : IComparable<TKey>
        {
            return new VersionedOrderedEnumerable<TSource>(source)
                .AddVersionedLevel(keySelector, false);
        }

        public static IVersionedOrderedEnumerable<TSource> VersionedOrderByDescending<TSource, TKey>(
            this IVersionedEnumerable<TSource> source,
            Func<TSource, TKey> keySelector)
            where TKey : IComparable<TKey>
        {
            return new VersionedOrderedEnumerable<TSource>(source)
                .AddLevel(x => keySelector(x), true);
        }

        public static IVersionedOrderedEnumerable<TSource> VersionedOrderByDescending<TSource, TKey>(
            this IVersionedEnumerable<TSource> source,
            Func<TSource, IVersionedValue<TKey>> keySelector)
            where TKey : IComparable<TKey>
        {
            return new VersionedOrderedEnumerable<TSource>(source)
                .AddVersionedLevel(keySelector, true);
        }

        public static IVersionedOrderedEnumerable<TSource> VersionedThenBy<TSource, TKey>(
            this IVersionedOrderedEnumerable<TSource> source,
            Func<TSource, TKey> keySelector)
            where TKey : IComparable<TKey>
        {
            return source.AddLevel(x => keySelector(x), false);
        }

        public static IVersionedOrderedEnumerable<TSource> VersionedThenBy<TSource, TKey>(
            this IVersionedOrderedEnumerable<TSource> source,
            Func<TSource, IVersionedValue<TKey>> keySelector)
            where TKey : IComparable<TKey>
        {
            return source.AddVersionedLevel(keySelector, false);
        }

        public static IVersionedOrderedEnumerable<TSource> VersionedThenByDescending<TSource, TKey>(
            this IVersionedOrderedEnumerable<TSource> source,
            Func<TSource, TKey> keySelector)
            where TKey : IComparable<TKey>
        {
            return source.AddLevel(x => keySelector(x), true);
        }

        public static IVersionedOrderedEnumerable<TSource> VersionedThenByDescending<TSource, TKey>(
            this IVersionedOrderedEnumerable<TSource> source,
            Func<TSource, IVersionedValue<TKey>> keySelector)
            where TKey : IComparable<TKey>
        {
            return source.AddVersionedLevel(keySelector, true);
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

        public interface IVersionedOrderedEnumerable<out T> : IVersionedEnumerable<T>
        {
            IVersionedOrderedEnumerable<T> AddLevel<TKey>(Func<T, TKey> keySelector, bool descending)
                where TKey : IComparable<TKey>;

            IVersionedOrderedEnumerable<T> AddVersionedLevel<TKey>(Func<T, IVersionedValue<TKey>> keySelector,
                bool descending)
                where TKey : IComparable<TKey>;
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

            IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

            public abstract ulong Version { get; }
            public IVersionedEnumerable<T> Value => this;

            public IEnumerable<T> Uncached => GetUncached();
            protected abstract IEnumerable<T> GetUncached();
        }

        private sealed class VersionedSelector<TSource, TTarget> : CachedVersionedEnumerable<TTarget>
        {
            private readonly IVersionedEnumerable<TSource> _source;
            private readonly IEnumerable<TSource> _uncachedSource;
            private readonly Func<TSource, TTarget> _selector;

            public VersionedSelector(IVersionedEnumerable<TSource> source, Func<TSource, TTarget> selector)
            {
                _source = source;
                _uncachedSource = source is ICachingVersionedEnumerable<TSource> cachingSource
                    ? cachingSource.Uncached
                    : source;
                _selector = selector;
            }

            public override ulong Version => _source.Version;

            protected override IEnumerable<TTarget> GetUncached() => _uncachedSource.Select(_selector);
        }

        private sealed class VersionedSelectManyImpl<TSource, TResult> : CachedVersionedEnumerable<TResult>
        {
            private readonly IVersionedEnumerable<TSource> _source;
            private readonly IEnumerable<TSource> _uncachedSource;
            private readonly Func<TSource, IEnumerable<TResult>> _selector;

            public VersionedSelectManyImpl(IVersionedEnumerable<TSource> source, Func<TSource, IEnumerable<TResult>> selector)
            {
                _source = source;
                _uncachedSource = source is ICachingVersionedEnumerable<TSource> cachingSource
                    ? cachingSource.Uncached
                    : source;
                _selector = selector;
            }

            public override ulong Version => _source.Version;

            protected override IEnumerable<TResult> GetUncached() => _uncachedSource.SelectMany(_selector);
        }

        private sealed class VersionedFilter<TSource> : CachedVersionedEnumerable<TSource>
        {
            private readonly IVersionedEnumerable<TSource> _source;
            private readonly IEnumerable<TSource> _uncachedSource;
            private readonly Func<TSource, bool> _predicate;

            public VersionedFilter(IVersionedEnumerable<TSource> source, Func<TSource, bool> predicate)
            {
                _source = source;
                _uncachedSource = source is ICachingVersionedEnumerable<TSource> cachingSource
                    ? cachingSource.Uncached
                    : source;
                _predicate = predicate;
            }

            public override ulong Version => _source.Version;

            protected override IEnumerable<TSource> GetUncached() => _uncachedSource.Where(_predicate);
        }

        private sealed class VersionedConcatenator<TSource> : CachedVersionedEnumerable<TSource>
        {
            private readonly IVersionedEnumerable<TSource> _first;
            private readonly IEnumerable<TSource> _firstUncachedSource;
            private readonly IVersionedEnumerable<TSource> _second;
            private readonly IEnumerable<TSource> _secondUncachedSource;

            public VersionedConcatenator(IVersionedEnumerable<TSource> first, IVersionedEnumerable<TSource> second)
            {
                _first = first;
                _firstUncachedSource = first is ICachingVersionedEnumerable<TSource> firstCachingSource
                    ? firstCachingSource.Uncached
                    : first;

                _second = second;
                _secondUncachedSource = second is ICachingVersionedEnumerable<TSource> secondCachingSource
                    ? secondCachingSource.Uncached
                    : second;
            }

            public override ulong Version => unchecked(_first.Version + _second.Version);

            protected override IEnumerable<TSource> GetUncached() => _firstUncachedSource.Concat(_secondUncachedSource);
        }

        private sealed class VersionedOrderedEnumerable<TSource> : CachedVersionedEnumerable<TSource>,
            IVersionedOrderedEnumerable<TSource>
        {
            private readonly IVersionedEnumerable<TSource> _source;
            private readonly IEnumerable<TSource> _uncachedSource;
            private readonly IReadOnlyList<ISortLevel<TSource>> _levels;
            
            private ulong _cachedSourceVersion = Versions.None;
            private ulong _cachedLevelsVersion = Versions.None;
            private ulong _version = Versions.Initial;

            public VersionedOrderedEnumerable(IVersionedEnumerable<TSource> source)
                : this(
                    source,
                    source is ICachingVersionedEnumerable<TSource> cachingSource ? cachingSource.Uncached : source,
                    Array.Empty<ISortLevel<TSource>>())
            {
            }

            private VersionedOrderedEnumerable(
                IVersionedEnumerable<TSource> source,
                IEnumerable<TSource> uncachedSource,
                IReadOnlyList<ISortLevel<TSource>> levels)
            {
                _source = source;
                _uncachedSource = uncachedSource;
                _levels = levels;
            }

            public IVersionedOrderedEnumerable<TSource> AddLevel<TKey>(Func<TSource, TKey> keySelector, bool descending)
                where TKey : IComparable<TKey>
            {
                return new VersionedOrderedEnumerable<TSource>(
                    _source,
                    _uncachedSource,
                    _levels.Concat(new ISortLevel<TSource>[]
                        { new PlainSortLevel<TSource, TKey>(keySelector, descending) }).ToArray());
            }

            public IVersionedOrderedEnumerable<TSource> AddVersionedLevel<TKey>(
                Func<TSource, IVersionedValue<TKey>> keySelector,
                bool descending)
                where TKey : IComparable<TKey>
            {
                return new VersionedOrderedEnumerable<TSource>(
                    _source,
                    _uncachedSource,
                    _levels.Concat(new ISortLevel<TSource>[]
                        { new VersionedSortLevel<TSource, TKey>(keySelector, descending) }).ToArray());
            }

            public override ulong Version
            {
                get
                {
                    if (_source.Version != _cachedSourceVersion)
                    {
                        _cachedSourceVersion = _source.Version;
                        _cachedLevelsVersion = Versions.None;
                    }
                    
                    var levelsVersion = Versions.None;
                    foreach (var level in _levels)
                    {
                        levelsVersion = unchecked(levelsVersion + level.Version(_uncachedSource));
                    }

                    if (_cachedLevelsVersion != levelsVersion)
                    {
                        _cachedLevelsVersion = levelsVersion;
                        Versions.Increment(ref _version);
                    }

                    return _version;
                }
            }

            protected override IEnumerable<TSource> GetUncached()
            {
                IOrderedEnumerable<TSource>? ordered = null;

                foreach (var level in _levels)
                {
                    ordered = level.Apply(_uncachedSource, ordered);
                }

                return ordered ?? _uncachedSource;
            }
        }

        private interface ISortLevel<TSource>
        {
            ulong Version(IEnumerable<TSource> source);
            IOrderedEnumerable<TSource> Apply(IEnumerable<TSource> source, IOrderedEnumerable<TSource>? ordered);
        }

        private sealed class PlainSortLevel<TSource, TKey> : ISortLevel<TSource>
            where TKey : IComparable<TKey>
        {
            private readonly Func<TSource, TKey> _keySelector;
            private readonly bool _descending;

            public PlainSortLevel(Func<TSource, TKey> keySelector, bool descending)
            {
                _keySelector = keySelector;
                _descending = descending;
            }

            public ulong Version(IEnumerable<TSource> source) => 1ul;

            public IOrderedEnumerable<TSource> Apply(IEnumerable<TSource> source, IOrderedEnumerable<TSource>? ordered)
            {
                if (ordered == null)
                {
                    return _descending ? source.OrderByDescending(_keySelector) : source.OrderBy(_keySelector);
                }

                return _descending ? ordered.ThenByDescending(_keySelector) : ordered.ThenBy(_keySelector);
            }
        }

        private sealed class VersionedSortLevel<TSource, TKey> : ISortLevel<TSource>
            where TKey : IComparable<TKey>
        {
            private readonly Func<TSource, IVersionedValue<TKey>> _keySelector;
            private readonly bool _descending;
            private ulong _cachedSourceVersion;
            private ulong _cachedListVersion;
            private ulong _version = Versions.Initial;

            public VersionedSortLevel(Func<TSource, IVersionedValue<TKey>> keySelector, bool descending)
            {
                _keySelector = keySelector;
                _descending = descending;
            }

            public ulong Version(IEnumerable<TSource> source)
            {
                var sourceVersion = GetSourceVersion(source);
                if (sourceVersion != _cachedSourceVersion)
                {
                    _cachedSourceVersion = sourceVersion;
                    _cachedListVersion = Versions.None;
                }

                var listVersion = Versions.None;
                foreach (var item in source)
                {
                    listVersion = unchecked(listVersion + _keySelector(item).Version);
                }

                if (_cachedListVersion != listVersion)
                {
                    _cachedListVersion = listVersion;
                    Versions.Increment(ref _version);
                }

                return _version;
            }

            private static ulong GetSourceVersion(IEnumerable<TSource> source)
            {
                return source is IVersioned versionedSource ? versionedSource.Version : Versions.Initial;
            }

            public IOrderedEnumerable<TSource> Apply(IEnumerable<TSource> source, IOrderedEnumerable<TSource>? ordered)
            {
                if (ordered == null)
                {
                    return _descending
                        ? source.OrderByDescending(item => _keySelector(item).Value)
                        : source.OrderBy(item => _keySelector(item).Value);
                }

                return _descending
                    ? ordered.ThenByDescending(item => _keySelector(item).Value)
                    : ordered.ThenBy(item => _keySelector(item).Value);
            }
        }

        private sealed class VersionedValueAdapter<TSource> : IVersionedEnumerable<TSource>
        {
            private readonly IVersionedValue<IEnumerable<TSource>> _source;

            public VersionedValueAdapter(IVersionedValue<IEnumerable<TSource>> source)
            {
                _source = source;
            }

            public IEnumerator<TSource> GetEnumerator() => _source.Value.GetEnumerator();
            IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

            public ulong Version => _source.Version;
            public IVersionedEnumerable<TSource> Value => this;
        }
    }
}