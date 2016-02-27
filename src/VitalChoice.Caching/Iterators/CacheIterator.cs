using System;
using System.Collections.Generic;
using Microsoft.Data.Entity;
using Microsoft.Data.Entity.ChangeTracking;
using VitalChoice.Caching.Interfaces;
using VitalChoice.Caching.Relational;
using VitalChoice.Caching.Services.Cache.Base;
using VitalChoice.Ecommerce.Domain;
using VitalChoice.ObjectMapping.Base;
using VitalChoice.ObjectMapping.Extensions;

namespace VitalChoice.Caching.Iterators
{
    internal struct TrackedIteratorParams<T>
        where T : class, new()
    {
        public IEnumerable<CacheResult<T>> Source;
        public Func<T, bool> Predicate;
        public Dictionary<EntityKey, EntityEntry<T>> Tracked;
        public ICacheKeysStorage<T> KeysStorage;
    }

    internal class CacheIterator<TSource> : SimpleIterator<TSource>
        where TSource : class, new()
    {
        private readonly IEnumerable<CacheResult<TSource>> _source;
        private readonly Func<TSource, bool> _predicate;
        private readonly bool _track;
        private readonly ICacheKeysStorage<TSource> _keysStorage;
        public Dictionary<EntityKey, EntityEntry<TSource>> Tracked { get; }
        private IEnumerator<CacheResult<TSource>> _enumerator;

        public CacheIterator(IEnumerable<CacheResult<TSource>> source, Func<TSource, bool> predicate)
        {
            _source = source;
            _predicate = predicate;
        }

        public CacheIterator(TrackedIteratorParams<TSource> trackedParams)
        {
            _track = true;
            _source = trackedParams.Source;
            _predicate = trackedParams.Predicate;
            _keysStorage = trackedParams.KeysStorage;
            Tracked = trackedParams.Tracked;
        }

        public override void Dispose()
        {
            _enumerator?.Dispose();
            _enumerator = null;
            base.Dispose();
        }

        public bool Found { get; private set; }

        public CacheGetResult AggregatedResult { get; private set; } = CacheGetResult.Found;

        public override bool MoveNext()
        {
            switch (State)
            {
                case 1:
                    _enumerator = _source.GetEnumerator();
                    if (_predicate == null)
                    {
                        State = 3;
                        goto case 3;
                    }
                    State = 2;
                    goto case 2;
                case 2:
                    while (_enumerator.MoveNext())
                    {
                        CacheResult<TSource> item = _enumerator.Current;
                        if (item.Result != CacheGetResult.Found)
                        {
                            AggregatedResult = item.Result;
                            Dispose();
                            break;
                        }
                        Found = true;
                        if (_predicate(item))
                        {
                            CurrentValue = _track ? GetAttached(item) : item;
                            return true;
                        }
                    }
                    Dispose();
                    break;
                case 3:
                    while (_enumerator.MoveNext())
                    {
                        CacheResult<TSource> item = _enumerator.Current;
                        if (item.Result != CacheGetResult.Found)
                        {
                            AggregatedResult = item.Result;
                            Dispose();
                            break;
                        }
                        Found = true;
                        CurrentValue = _track ? GetAttached(item) : item;
                        return true;
                    }
                    Dispose();
                    break;
            }
            return false;
        }

        private TSource GetAttached(TSource item)
        {
            var pk = _keysStorage.GetPrimaryKeyValue(item);
            EntityEntry<TSource> entry;
            if (Tracked.TryGetValue(pk, out entry))
            {
                item = entry.State == EntityState.Detached ? item.Clone<TSource, Entity>() : entry.Entity;
            }
            else
            {
                item = item.Clone<TSource, Entity>();
            }
            return item;
        }
    }
}