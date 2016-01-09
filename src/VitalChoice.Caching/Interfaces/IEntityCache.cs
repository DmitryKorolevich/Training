using System.Collections.Generic;
using Microsoft.Data.Entity;
using VitalChoice.Caching.Services.Cache;
using VitalChoice.Caching.Services.Cache.Base;

namespace VitalChoice.Caching.Interfaces
{
    public interface IEntityCache<T>
    {
        CacheGetResult TryGetCached(QueryCacheData<T> queryCache, DbContext dbContext, out List<T> entities);
        CacheGetResult TryGetCachedFirstOrDefault(QueryCacheData<T> queryCache, DbContext dbContext, out T entity);
        void Update(QueryCacheData<T> query, IEnumerable<T> entities);
        void Update(QueryCacheData<T> query, T entity);
    }
}