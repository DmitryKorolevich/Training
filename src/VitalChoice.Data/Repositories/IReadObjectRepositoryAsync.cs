using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using VitalChoice.Data.Helpers;
using VitalChoice.Domain;

namespace VitalChoice.Data.Repositories
{
    public interface IReadObjectRepositoryAsync<T, TEntity>
        where TEntity : Entity
    {
        Task<T> SelectAsync(int id);
        Task<List<T>> SelectAsync(ICollection<int> ids);
        Task<List<T>> SelectAsync();
        Task<List<T>> SelectAsync(IQueryObject<TEntity> queryObject);
        Task<List<T>> SelectAsync(Expression<Func<TEntity, bool>> query);
        T Select(int id);
        List<T> Select(ICollection<int> ids);
        List<T> Select();
        List<T> Select(IQueryObject<TEntity> queryObject);
        List<T> Select(Expression<Func<TEntity, bool>> query);
    }
}