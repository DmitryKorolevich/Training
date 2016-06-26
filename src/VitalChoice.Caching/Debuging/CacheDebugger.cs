using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using VitalChoice.Caching.Extensions;
using VitalChoice.Caching.Interfaces;
using VitalChoice.Caching.Services.Cache.Base;

namespace VitalChoice.Caching.Debuging
{
    public struct CacheUpdateData
    {
        public Type EntityType { get; set; }

        public object UpdateEntity { get; set; }

        public ICollection<object> CachedEntities { get; set; }

        public object ActualDbEntity { get; set; }
    }

    public static class CacheDebugger
    {
        internal static ICacheServiceScopeFactoryContainer ScopeFactoryContainer;

        public static IInternalEntityCacheFactory CacheFactory { get; internal set; }
        public static IEntityInfoStorage EntityInfo { get; internal set; }

        public static IEnumerable<CacheUpdateData> ProcessDbUpdateException(DbUpdateException exception)
        {
            foreach (var entry in exception.Entries)
            {
                if (entry.Entity != null)
                {
                    var type = entry.Entity.GetType();
                    EntityInfo entityInfo;
                    if (EntityInfo.GetEntityInfo(type, out entityInfo))
                    {
                        IInternalEntityCache internalCache = CacheFactory.GetCache(type);
                        if (internalCache != null)
                        {
                            var pk = entityInfo.PrimaryKey.GetPrimaryKeyValue(entry.Entity);
                            List<object> cachedList = new List<object>();
                            foreach (var data in internalCache.GetAllCaches())
                            {
                                var cached = data.GetUntyped(pk);
                                if (cached != null)
                                {
                                    using (cached.Lock())
                                    {
                                        cachedList.Add(cached.EntityUntyped.DeepCloneItem(data.Relations));
                                    }
                                }
                            }
                            yield return new CacheUpdateData
                            {
                                EntityType = type,
                                ActualDbEntity = EntityInfo.GetEntity(type, pk, ScopeFactoryContainer.ScopeFactory),
                                CachedEntities = cachedList,
                                UpdateEntity = entry.Entity
                            };
                        }
                    }
                }
            }
        }
    }
}