using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using VitalChoice.Data.Helpers;
using VitalChoice.Data.Repositories;
using VitalChoice.Domain;

namespace VitalChoice.DynamicData.Interfaces
{
    public interface IReadDynamicObjectServiceAsync<T, TEntity> : IReadObjectServiceAsync<T, TEntity>
        where TEntity : Entity
    {
        Task<T> CreatePrototypeAsync(int idObjectType);
        Task<TModel> CreatePrototypeForAsync<TModel>(int idObjectType)
            where TModel : class, new();
        Task<T> SelectAsync(int id, bool withDefaults);
        Task<List<T>> SelectAsync(ICollection<int> ids, bool withDefaults);
        Task<List<T>> SelectAsync(bool withDefaults);
        Task<List<T>> SelectAsync(IQueryObject<TEntity> queryObject, bool withDefaults);
        Task<List<T>> SelectAsync(Expression<Func<TEntity, bool>> query, bool withDefaults);
        Task<List<T>> SelectAsync(IDictionary<string, object> values, bool withDefaults);
        Task<List<T>> SelectAsync(IDictionary<string, object> values, Expression<Func<TEntity, bool>> query, bool withDefaults);
        T CreatePrototype(int idObjectType);
        TModel CreatePrototypeFor<TModel>(int idObjectType)
            where TModel : class, new();
        T Select(int id, bool withDefaults);
        List<T> Select(ICollection<int> ids, bool withDefaults);
        List<T> Select(bool withDefaults);
        List<T> Select(IQueryObject<TEntity> queryObject, bool withDefaults);
        List<T> Select(Expression<Func<TEntity, bool>> query, bool withDefaults);
        List<T> Select(IDictionary<string, object> values, bool withDefaults);
        List<T> Select(IDictionary<string, object> values, Expression<Func<TEntity, bool>> query, bool withDefaults);

    }
}