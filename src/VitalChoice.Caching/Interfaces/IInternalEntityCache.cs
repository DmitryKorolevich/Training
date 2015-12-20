using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using VitalChoice.Caching.Data;
using VitalChoice.Ecommerce.Domain;

namespace VitalChoice.Caching.Interfaces
{
    internal interface IInternalEntityCache<TEntity>
        where TEntity : Entity
    {
        bool TryGetCachedFirst(EntityCacheKey key, out TEntity entity);
        bool TryGetCached(EntityCacheKey key, out List<TEntity> entities);
        bool TryGetCachedFirst<T>(EntityCacheKey key, Expression<Func<TEntity, T>> selector, out T selectItem);
        bool TryGetCached<T>(EntityCacheKey key, Expression<Func<TEntity, T>> selector, out List<T> selectList);

        void InvalidateCache(EntityCacheKey key, TEntity entity);
        void InvalidateCache(EntityCacheKey key, List<TEntity> entity);
        void InvalidateCache<T>(EntityCacheKey key, Expression<Func<TEntity, T>> selector, T entity);
        void InvalidateCache<T>(EntityCacheKey key, Expression<Func<TEntity, T>> selector, List<T> entity);

        void PurgeCache(EntityCacheKey key);
    }
}