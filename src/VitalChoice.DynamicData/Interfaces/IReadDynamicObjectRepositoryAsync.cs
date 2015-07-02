using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using VitalChoice.Data.Helpers;
using VitalChoice.Data.Repositories;
using VitalChoice.Domain;

namespace VitalChoice.DynamicData.Interfaces
{
    public interface IReadDynamicObjectRepositoryAsync<T, TEntity> : IReadObjectRepositoryAsync<T, TEntity>
        where TEntity : Entity
    {
        Task<T> SelectAsync(int id, bool withDefaults);
        Task<List<T>> SelectAsync(bool withDefaults);
        Task<List<T>> SelectAsync(IQueryObject<TEntity> queryObject, bool withDefaults);
        Task<List<T>> SelectAsync(Expression<Func<TEntity, bool>> query, bool withDefaults);
        T Select(int id, bool withDefaults);
        List<T> Select(bool withDefaults);
        List<T> Select(IQueryObject<TEntity> queryObject, bool withDefaults);
        List<T> Select(Expression<Func<TEntity, bool>> query, bool withDefaults);
    }
}