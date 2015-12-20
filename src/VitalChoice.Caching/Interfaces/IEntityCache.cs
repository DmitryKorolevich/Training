using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using VitalChoice.Ecommerce.Domain;

namespace VitalChoice.Caching.Interfaces
{
    public interface IEntityCache<TEntity>
        where TEntity:Entity
    {
        Task<List<TEntity>> GetCachedOrReadAsync(IQueryable<TEntity> query);
        Task<TEntity> GetCachedOrReadFirstOrDefaultAsync(IQueryable<TEntity> query);
        Task<List<T>> GetCachedOrReadAsync<T>(IQueryable<TEntity> query, Expression<Func<TEntity, T>> selector);
        Task<T> GetCachedOrReadFirstOrDefaultAsync<T>(IQueryable<TEntity> query, Expression<Func<TEntity, T>> selector);
    }
}