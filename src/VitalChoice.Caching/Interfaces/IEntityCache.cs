﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.Data.Entity;
using VitalChoice.Caching.Services.Cache;
using VitalChoice.Ecommerce.Domain;

namespace VitalChoice.Caching.Interfaces
{
    public interface IEntityCache<T>
        where T : Entity
    {
        CacheGetResult TryGetCached(QueryCacheData<T> queryCache, DbContext dbContext, out List<T> entities);
        CacheGetResult TryGetCachedFirstOrDefault(QueryCacheData<T> queryCache, DbContext dbContext, out T entity);
        void Update(QueryCacheData<T> query, IEnumerable<T> entities);
        void Update(QueryCacheData<T> query, T entity);
    }
}