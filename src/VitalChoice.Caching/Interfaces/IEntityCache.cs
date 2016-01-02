using System;
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
        CacheGetResult TryGetCached(IQueryable<T> query, DbContext dbContext, out List<T> entities);
        CacheGetResult TryGetCachedFirstOrDefault(IQueryable<T> query, DbContext dbContext, out T entity);
        void Update(IQueryable<T> query, ICollection<T> entities);
        void Update(IQueryable<T> query, T entity);
    }
}