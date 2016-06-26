using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking.Internal;
using VitalChoice.Caching.Extensions;
using VitalChoice.Caching.Interfaces;
using VitalChoice.Caching.Relational;
using VitalChoice.Caching.Relational.ChangeTracking;
using VitalChoice.Caching.Services.Cache.Base;
using VitalChoice.Ecommerce.Domain;
using VitalChoice.ObjectMapping.Base;
using VitalChoice.ObjectMapping.Extensions;

namespace VitalChoice.Caching.Iterators
{
    internal class CacheIterator<TSource> : SimpleIterator<TSource>
    {
        private readonly IEnumerable<CacheResult<TSource>> _source;
        private readonly Func<TSource, bool> _predicate;
        private readonly IDictionary<TrackedEntityKey, InternalEntityEntry> _trackData;
        private readonly EntityInfo _entityInfo;
        private IEnumerator<CacheResult<TSource>> _enumerator;

        public CacheIterator(IEnumerable<CacheResult<TSource>> source, Func<TSource, bool> predicate, EntityInfo entityInfo = null, IDictionary<TrackedEntityKey, InternalEntityEntry> trackData = null)
        {
            _source = source;
            _predicate = predicate;
            _trackData = trackData;
            _entityInfo = entityInfo;
            HasResults = true;
        }

        public override void Dispose()
        {
            _enumerator?.Dispose();
            _enumerator = null;
            base.Dispose();
        }

        public bool HasResults { get; private set; }

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
                            HasResults = false;
                            AggregatedResult = item.Result;
                            Dispose();
                            break;
                        }
                        if (item.Entity != null && _predicate(item))
                        {
                            if (_trackData != null && _entityInfo != null)
                            {
                                var trackKey = new TrackedEntityKey(typeof(TSource),
                                    _entityInfo.PrimaryKey.GetPrimaryKeyValue(item.Entity));

                                InternalEntityEntry entry;
                                if (_trackData.TryGetValue(trackKey, out entry))
                                {
                                    if (entry.EntityState != EntityState.Detached)
                                    {
                                        entry.SetEntityState(EntityState.Detached);
                                    }
                                }
                            }
                            CurrentValue = item;
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
                            HasResults = false;
                            AggregatedResult = item.Result;
                            Dispose();
                            break;
                        }
                        if (_trackData != null && _entityInfo != null && item.Entity != null)
                        {
                            var trackKey = new TrackedEntityKey(typeof(TSource),
                                _entityInfo.PrimaryKey.GetPrimaryKeyValue(item.Entity));

                            InternalEntityEntry entry;
                            if (_trackData.TryGetValue(trackKey, out entry))
                            {
                                if (entry.EntityState != EntityState.Detached)
                                {
                                    entry.SetEntityState(EntityState.Detached);
                                }
                            }
                        }
                        CurrentValue = item;
                        return true;
                    }
                    Dispose();
                    break;
            }
            return false;
        }

        //private TSource GetAttached(TSource item)
        //{
        //    if (item == null)
        //        return null;

        //    var pk = _internalCache.EntityInfo.PrimaryKey.GetPrimaryKeyValue(item);
        //    EntityEntry<TSource> entry;
        //    if (Tracked.TryGetValue(pk, out entry))
        //    {
        //        item = entry.State == EntityState.Detached ? item.Clone() : entry.Entity;
        //    }
        //    else
        //    {
        //        item = item.Clone();
        //    }
        //    return item;
        //}
    }
}