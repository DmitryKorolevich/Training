using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Microsoft.Data.Entity;
using Microsoft.Data.Entity.ChangeTracking;
using VitalChoice.Caching.Interfaces;
using VitalChoice.Caching.Relational;
using VitalChoice.Caching.Services.Cache;
using VitalChoice.Caching.Services.Cache.Base;

namespace VitalChoice.Caching.Iterators
{
    internal class CacheIterator<TSource> : SimpleIterator<TSource>
        where TSource : class
    {
        private readonly IEnumerable<CacheResult<TSource>> _source;
        private readonly Func<TSource, bool> _predicate;
        private readonly bool _track;
        private readonly DbContext _context;
        private readonly ICacheKeysStorage<TSource> _keysStorage;
        private readonly Dictionary<EntityKey, EntityEntry<TSource>> _tracked;
        private IEnumerator<CacheResult<TSource>> _enumerator;

        public CacheIterator(IEnumerable<CacheResult<TSource>> source, Func<TSource, bool> predicate, bool track = false,
            DbContext context = null, ICacheKeysStorage<TSource> keysStorage = null)
        {
            _source = source;
            _predicate = predicate;
            _track = track;
            _context = context;
            _keysStorage = keysStorage;
            if (track)
            {
                if (context == null || keysStorage == null)
                {
                    throw new ArgumentException($"DbContext is null or ICacheKeysStorage is null <{typeof (TSource)}>");
                }
                _tracked = context.ChangeTracker.Entries<TSource>().ToDictionary(e => keysStorage.GetPrimaryKeyValue(e.Entity), e => e);
            }
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
                            CurrentValue = _track ? Attach(item) : item;
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
                        CurrentValue = _track ? Attach(item) : item;
                        return true;
                    }
                    Dispose();
                    break;
            }
            return false;
        }

        private TSource Attach(TSource item)
        {
            var pk = _keysStorage.GetPrimaryKeyValue(item);
            EntityEntry<TSource> entry;
            if (_tracked.TryGetValue(pk, out entry))
            {
                if (entry.State == EntityState.Detached)
                {
                    _context.Attach(item);
                }
                else
                {
                    item = entry.Entity;
                }
            }
            else
            {
                _context.Attach(item);
            }
            return item;
        }
    }
}