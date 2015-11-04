using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using VitalChoice.Data.Helpers;
using VitalChoice.Data.Repositories;
using VitalChoice.Domain;
using VitalChoice.Domain.Transfer.Base;

namespace VitalChoice.DynamicData.Interfaces
{
    public interface IDynamicReadServiceAsync<T, TEntity>// : IReadObjectServiceAsync<T, TEntity>
        where TEntity : Entity
    {
        Task<T> CreatePrototypeAsync(int idObjectType);

        Task<TModel> CreatePrototypeForAsync<TModel>(int idObjectType)
            where TModel : class, new();

        Task<T> SelectAsync(int id, bool withDefaults = false, Func<IQueryLite<TEntity>, IQueryLite<TEntity>> includesOverride = null);
        Task<List<T>> SelectAsync(ICollection<int> ids, bool withDefaults = false, Func<IQueryLite<TEntity>, IQueryLite<TEntity>> includesOverride = null);

        Task<List<T>> SelectAsync(IDictionary<string, object> values = null, IQueryObject<TEntity> queryObject = null,
            Func<IQueryLite<TEntity>, IQueryLite<TEntity>> includesOverride = null, bool withDefaults = false);

        Task<List<T>> SelectAsync(Expression<Func<TEntity, bool>> query = null,
            IDictionary<string, object> values = null,
            Func<IQueryLite<TEntity>, IQueryLite<TEntity>> includesOverride = null, bool withDefaults = false);

        Task<T> SelectFirstAsync(IDictionary<string, object> values = null, IQueryObject<TEntity> queryObject = null,
            Func<IQueryLite<TEntity>, IQueryLite<TEntity>> includesOverride = null, Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null, bool withDefaults = false);

        Task<T> SelectFirstAsync(Expression<Func<TEntity, bool>> query = null,
            IDictionary<string, object> values = null,
            Func<IQueryLite<TEntity>, IQueryLite<TEntity>> includesOverride = null, Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null, bool withDefaults = false);

        Task<PagedList<T>> SelectPageAsync(int page, int pageSize, IDictionary<string, object> values = null, IQueryObject<TEntity> queryObject = null,
            Func<IQueryLite<TEntity>, IQueryLite<TEntity>> includesOverride = null, Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null, bool withDefaults = false);

        Task<PagedList<T>> SelectPageAsync(int page, int pageSize, Expression<Func<TEntity, bool>> query = null,
            IDictionary<string, object> values = null,
            Func<IQueryLite<TEntity>, IQueryLite<TEntity>> includesOverride = null, Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null, bool withDefaults = false);

        T Select(int id, bool withDefaults = false, Func<IQueryLite<TEntity>, IQueryLite<TEntity>> includesOverride = null);
        List<T> Select(ICollection<int> ids, bool withDefaults = false, Func<IQueryLite<TEntity>, IQueryLite<TEntity>> includesOverride = null);

        List<T> Select(IDictionary<string, object> values = null, IQueryObject<TEntity> queryObject = null,
            Func<IQueryLite<TEntity>, IQueryLite<TEntity>> includesOverride = null, bool withDefaults = false);

        List<T> Select(Expression<Func<TEntity, bool>> query = null, IDictionary<string, object> values = null,
            Func<IQueryLite<TEntity>, IQueryLite<TEntity>> includesOverride = null, bool withDefaults = false);

        T SelectFirst(IDictionary<string, object> values = null, IQueryObject<TEntity> queryObject = null,
            Func<IQueryLite<TEntity>, IQueryLite<TEntity>> includesOverride = null, Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null, bool withDefaults = false);

        T SelectFirst(Expression<Func<TEntity, bool>> query = null,
            IDictionary<string, object> values = null,
            Func<IQueryLite<TEntity>, IQueryLite<TEntity>> includesOverride = null, Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null, bool withDefaults = false);

        PagedList<T> SelectPage(int page, int pageSize, IDictionary<string, object> values = null, IQueryObject<TEntity> queryObject = null,
            Func<IQueryLite<TEntity>, IQueryLite<TEntity>> includesOverride = null, Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null, bool withDefaults = false);

        PagedList<T> SelectPage(int page, int pageSize, Expression<Func<TEntity, bool>> query = null,
            IDictionary<string, object> values = null,
            Func<IQueryLite<TEntity>, IQueryLite<TEntity>> includesOverride = null, Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null, bool withDefaults = false);

        T CreatePrototype(int idObjectType);

        TModel CreatePrototypeFor<TModel>(int idObjectType)
            where TModel : class, new();
    }
}