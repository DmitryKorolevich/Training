using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using VitalChoice.Caching.Services.Cache;
using VitalChoice.Ecommerce.Domain;

namespace VitalChoice.Caching.Interfaces
{
    public interface IEntityCache<TEntity>
        where TEntity : Entity
    {
        CacheGetResult TryGetCached(IQueryable<TEntity> query, out List<TEntity> entities);
        CacheGetResult TryGetCachedFirstOrDefault(IQueryable<TEntity> query, out TEntity entity);
        void Update(IQueryable<TEntity> query, ICollection<TEntity> entities);
        void Update(IQueryable<TEntity> query, TEntity entity);
    }
}