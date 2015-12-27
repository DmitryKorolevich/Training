using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using VitalChoice.Ecommerce.Domain;

namespace VitalChoice.Caching.Interfaces
{
    public interface IEntityCache<TEntity>
        where TEntity : Entity
    {
        bool TryGetCached(IQueryable<TEntity> query, out List<TEntity> entities);
        bool TryGetCachedFirstOrDefault(IQueryable<TEntity> query, out TEntity entity);
        bool TryGetCached<T>(IQueryable<TEntity> query, Expression<Func<TEntity, T>> selector, out List<T> results);
        bool TryGetCachedFirstOrDefault<T>(IQueryable<TEntity> query, Expression<Func<TEntity, T>> selector, out T result);
        void Update(IQueryable<TEntity> query, ICollection<TEntity> entities);
        void Update(IQueryable<TEntity> query, TEntity entity);
        void Update<T>(IQueryable<TEntity> query, Expression<Func<TEntity, T>> selector, ICollection<T> results);
        void Update<T>(IQueryable<TEntity> query, Expression<Func<TEntity, T>> selector, T result);
    }
}