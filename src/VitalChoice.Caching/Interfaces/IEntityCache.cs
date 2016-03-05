using System.Collections.Generic;
using Microsoft.Data.Entity;
using VitalChoice.Caching.Services.Cache;
using VitalChoice.Caching.Services.Cache.Base;

namespace VitalChoice.Caching.Interfaces
{
    public interface IEntityCache<T>
    {
        CacheGetResult TryGetCached(QueryData<T> query, out List<T> entities);
        CacheGetResult TryGetCachedFirstOrDefault(QueryData<T> query, out T entity);
        bool Update(QueryData<T> query, IEnumerable<T> entities);
        bool Update(QueryData<T> query, T entity);
    }
}