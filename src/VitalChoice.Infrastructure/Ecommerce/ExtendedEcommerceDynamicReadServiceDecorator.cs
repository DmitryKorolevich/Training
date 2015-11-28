using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using VitalChoice.Data.Helpers;
using VitalChoice.DynamicData.Interfaces;
using VitalChoice.Ecommerce.Domain.Dynamic;
using VitalChoice.Ecommerce.Domain.Entities.Base;
using VitalChoice.Ecommerce.Domain.Transfer;

namespace VitalChoice.Infrastructure.Ecommerce
{
    public class ExtendedEcommerceDynamicReadServiceDecorator<TDynamic, TEntity> : EcommerceDynamicReadServiceDecorator<TDynamic, TEntity>,
        IExtendedDynamicReadServiceAsync<TDynamic, TEntity> 
        where TEntity : DynamicDataEntity 
        where TDynamic : MappedObject
    {
        private readonly IExtendedDynamicReadServiceAsync<TDynamic, TEntity> _extendedReadService;

        public ExtendedEcommerceDynamicReadServiceDecorator(IExtendedDynamicReadServiceAsync<TDynamic, TEntity> extendedService) : base(extendedService)
        {
            _extendedReadService = extendedService;
        }

        public Task<TDynamic> SelectAsync(int id, bool withDefaults = false, Func<IQueryLite<TEntity>, IQueryLite<TEntity>> includesOverride = null)
        {
            return _extendedReadService.SelectAsync(id, withDefaults, includesOverride);
        }

        public Task<List<TDynamic>> SelectAsync(ICollection<int> ids, bool withDefaults = false, Func<IQueryLite<TEntity>, IQueryLite<TEntity>> includesOverride = null)
        {
            return _extendedReadService.SelectAsync(ids, withDefaults, includesOverride);
        }

        public Task<List<TDynamic>> SelectAsync(IQueryObject<TEntity> queryObject = null, Func<IQueryLite<TEntity>, IQueryLite<TEntity>> includesOverride = null, bool withDefaults = false)
        {
            return _extendedReadService.SelectAsync(queryObject, includesOverride, withDefaults);
        }

        public Task<List<TDynamic>> SelectAsync(Expression<Func<TEntity, bool>> query = null, Func<IQueryLite<TEntity>, IQueryLite<TEntity>> includesOverride = null, bool withDefaults = false)
        {
            return _extendedReadService.SelectAsync(query, includesOverride, withDefaults);
        }

        public Task<TDynamic> SelectFirstAsync(IQueryObject<TEntity> queryObject = null, Func<IQueryLite<TEntity>, IQueryLite<TEntity>> includesOverride = null, Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null, bool withDefaults = false)
        {
            return _extendedReadService.SelectFirstAsync(queryObject, includesOverride, orderBy, withDefaults);
        }

        public Task<TDynamic> SelectFirstAsync(Expression<Func<TEntity, bool>> query = null, Func<IQueryLite<TEntity>, IQueryLite<TEntity>> includesOverride = null, Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null, bool withDefaults = false)
        {
            return _extendedReadService.SelectFirstAsync(query, includesOverride, orderBy, withDefaults);
        }

        public Task<PagedList<TDynamic>> SelectPageAsync(int page, int pageSize, IQueryObject<TEntity> queryObject = null, Func<IQueryLite<TEntity>, IQueryLite<TEntity>> includesOverride = null, Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
            bool withDefaults = false)
        {
            return _extendedReadService.SelectPageAsync(page, pageSize, queryObject, includesOverride, orderBy, withDefaults);
        }

        public Task<PagedList<TDynamic>> SelectPageAsync(int page, int pageSize, Expression<Func<TEntity, bool>> query = null, Func<IQueryLite<TEntity>, IQueryLite<TEntity>> includesOverride = null, Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
            bool withDefaults = false)
        {
            return _extendedReadService.SelectPageAsync(page, pageSize, query, includesOverride, orderBy, withDefaults);
        }

        public TDynamic Select(int id, bool withDefaults = false, Func<IQueryLite<TEntity>, IQueryLite<TEntity>> includesOverride = null)
        {
            return _extendedReadService.Select(id, withDefaults, includesOverride);
        }

        public List<TDynamic> Select(ICollection<int> ids, bool withDefaults = false, Func<IQueryLite<TEntity>, IQueryLite<TEntity>> includesOverride = null)
        {
            return _extendedReadService.Select(ids, withDefaults, includesOverride);
        }

        public List<TDynamic> Select(IQueryObject<TEntity> queryObject = null, Func<IQueryLite<TEntity>, IQueryLite<TEntity>> includesOverride = null, bool withDefaults = false)
        {
            return _extendedReadService.Select(queryObject, includesOverride, withDefaults);
        }

        public List<TDynamic> Select(Expression<Func<TEntity, bool>> query = null, Func<IQueryLite<TEntity>, IQueryLite<TEntity>> includesOverride = null, bool withDefaults = false)
        {
            return _extendedReadService.Select(query, includesOverride, withDefaults);
        }

        public TDynamic SelectFirst(IQueryObject<TEntity> queryObject = null, Func<IQueryLite<TEntity>, IQueryLite<TEntity>> includesOverride = null, Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null, bool withDefaults = false)
        {
            return _extendedReadService.SelectFirst(queryObject, includesOverride, orderBy, withDefaults);
        }

        public TDynamic SelectFirst(Expression<Func<TEntity, bool>> query = null, Func<IQueryLite<TEntity>, IQueryLite<TEntity>> includesOverride = null, Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null, bool withDefaults = false)
        {
            return _extendedReadService.SelectFirst(query, includesOverride, orderBy, withDefaults);
        }

        public PagedList<TDynamic> SelectPage(int page, int pageSize, IQueryObject<TEntity> queryObject = null, Func<IQueryLite<TEntity>, IQueryLite<TEntity>> includesOverride = null, Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
            bool withDefaults = false)
        {
            return _extendedReadService.SelectPage(page, pageSize, queryObject, includesOverride, orderBy, withDefaults);
        }

        public PagedList<TDynamic> SelectPage(int page, int pageSize, Expression<Func<TEntity, bool>> query = null, Func<IQueryLite<TEntity>, IQueryLite<TEntity>> includesOverride = null, Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
            bool withDefaults = false)
        {
            return _extendedReadService.SelectPage(page, pageSize, query, includesOverride, orderBy, withDefaults);
        }
    }
}