using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Internal;
using VitalChoice.Data.Extensions;
using VitalChoice.Data.Helpers;
using VitalChoice.Data.Repositories;
using VitalChoice.Data.Services;
using VitalChoice.DynamicData.Extensions;
using VitalChoice.DynamicData.Helpers;
using VitalChoice.DynamicData.Interfaces;
using VitalChoice.Ecommerce.Domain.Dynamic;
using VitalChoice.Ecommerce.Domain.Entities;
using VitalChoice.Ecommerce.Domain.Entities.Base;
using VitalChoice.Ecommerce.Domain.Helpers;
using VitalChoice.Ecommerce.Domain.Transfer;

namespace VitalChoice.DynamicData.Base
{
    public abstract /*partial*/ class DynamicReadServiceAsync<TDynamic, TEntity, TOptionType, TOptionValue> :
        IExtendedDynamicReadServiceAsync<TDynamic, TEntity>
        where TEntity : DynamicDataEntity<TOptionValue, TOptionType>, new()
        where TOptionType : OptionType, new()
        where TOptionValue : OptionValue<TOptionType>, new()
        where TDynamic : MappedObject, new()
    {
        protected readonly IDynamicMapper<TDynamic, TEntity, TOptionType, TOptionValue> DynamicMapper;
        protected readonly IRepositoryAsync<TEntity> ObjectRepository;
        protected readonly IReadRepositoryAsync<TOptionValue> OptionValuesRepository;
        protected readonly IReadRepositoryAsync<BigStringValue> BigStringRepository;
        protected readonly IObjectLogItemExternalService ObjectLogItemExternalService;
        protected readonly DynamicExtensionsRewriter QueryVisitor;
        protected readonly IDynamicEntityOrderingExtension<TEntity> OrderingExtension;
        protected readonly ILogger Logger;

        protected DynamicReadServiceAsync(
            IDynamicMapper<TDynamic, TEntity, TOptionType, TOptionValue> mapper,
            IRepositoryAsync<TEntity> objectRepository,
            IReadRepositoryAsync<BigStringValue> bigStringRepository,
            IReadRepositoryAsync<TOptionValue> optionValuesRepository,
            IObjectLogItemExternalService objectLogItemExternalService,
            ILogger logger, DynamicExtensionsRewriter queryVisitor, IDynamicEntityOrderingExtension<TEntity> orderingExtension)
        {
            DynamicMapper = mapper;
            ObjectRepository = objectRepository;
            BigStringRepository = bigStringRepository;
            OptionValuesRepository = optionValuesRepository;
            ObjectLogItemExternalService = objectLogItemExternalService;
            Logger = logger;
            QueryVisitor = queryVisitor;
            OrderingExtension = orderingExtension;
        }

        #region Extension Points

        protected virtual Task AfterSelect(ICollection<TEntity> entity)
        {
            return TaskCache.CompletedTask;
        }

        protected virtual IQueryLite<TEntity> BuildIncludes(IQueryLite<TEntity> query)
        {
            return query;
        }

        protected virtual Expression<Func<TEntity, bool>> AdditionalDefaultConditions => null;

        protected async Task<List<TEntity>> SelectEntitiesAsync(Expression<Func<TEntity, bool>> query = null,
            Func<IQueryLite<TEntity>, IQueryLite<TEntity>> includesOverride = null,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null)
        {
            var queryFluent = BuildQueryFluent(query, includesOverride, orderBy);
            var entities = await queryFluent.SelectAsync(false);
            await ProcessEntities(entities);
            return entities;
        }

        protected async Task<TEntity> SelectEntityFirstAsync(Expression<Func<TEntity, bool>> query = null,
            Func<IQueryLite<TEntity>, IQueryLite<TEntity>> includesOverride = null,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null)
        {
            var queryFluent = BuildQueryFluent(query, includesOverride, orderBy);
            var entity = await queryFluent.SelectFirstOrDefaultAsync(false);
            if (entity != null)
            {
                await ProcessEntities(new[] {entity});
            }
            return entity;
        }

        protected async Task<PagedList<TEntity>> SelectEntityPageAsync(int page, int pageSize,
            Expression<Func<TEntity, bool>> query = null,
            Func<IQueryLite<TEntity>, IQueryLite<TEntity>> includesOverride = null,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null)
        {
            var queryFluent = BuildQueryFluent(query, includesOverride, orderBy);
            var entities = await queryFluent.SelectPageAsync(page, pageSize, false);
            await ProcessEntities(entities.Items);
            return entities;
        }

        #endregion

        public IDynamicMapper<TDynamic, TEntity> Mapper => DynamicMapper;

        Task<TDynamic> IDynamicReadServiceAsync<TDynamic, TEntity>.SelectAsync(int id, bool withDefaults)
        {
            return SelectAsync(id, withDefaults, null);
        }

        Task<List<TDynamic>> IDynamicReadServiceAsync<TDynamic, TEntity>.SelectAsync(ICollection<int> ids, bool withDefaults)
        {
            return SelectAsync(ids, withDefaults, null);
        }

        TDynamic IDynamicReadServiceAsync<TDynamic, TEntity>.Select(int id, bool withDefaults)
        {
            return Select(id, withDefaults, null);
        }

        List<TDynamic> IDynamicReadServiceAsync<TDynamic, TEntity>.Select(ICollection<int> ids, bool withDefaults)
        {
            return Select(ids, withDefaults, null);
        }

        public async Task<TDynamic> SelectAsync(int id, bool withDefaults = false,
            // ReSharper disable once MethodOverloadWithOptionalParameter
            Func<IQueryLite<TEntity>, IQueryLite<TEntity>> includesOverride = null)
        {
            return
                await
                    DynamicMapper.FromEntityAsync(
                        await
                            SelectEntityFirstAsync(o => o.Id == id && o.StatusCode != (int) RecordStatusCode.Deleted,
                                includesOverride: includesOverride), withDefaults);
        }

        public async Task<List<TDynamic>> SelectAsync(ICollection<int> ids, bool withDefaults = false,
            // ReSharper disable once MethodOverloadWithOptionalParameter
            Func<IQueryLite<TEntity>, IQueryLite<TEntity>> includesOverride = null)
        {
            return
                await
                    DynamicMapper.FromEntityRangeAsync(
                        await
                            SelectEntitiesAsync(o => ids.Contains(o.Id) && o.StatusCode != (int) RecordStatusCode.Deleted,
                                includesOverride: includesOverride), withDefaults);
        }

        public Task<List<TDynamic>> SelectAsync(IQueryObject<TEntity> queryObject = null,
            Func<IQueryLite<TEntity>, IQueryLite<TEntity>> includesOverride = null,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
            bool withDefaults = false)
        {
            return SelectAsync(queryObject?.Query(), includesOverride, orderBy, withDefaults);
        }

        public async Task<List<TDynamic>> SelectAsync(Expression<Func<TEntity, bool>> query = null,
            Func<IQueryLite<TEntity>, IQueryLite<TEntity>> includesOverride = null,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
            bool withDefaults = false)
        {
            return await DynamicMapper.FromEntityRangeAsync(await SelectEntitiesAsync(query, includesOverride, orderBy), withDefaults);
        }

        public Task<TDynamic> SelectFirstAsync(IQueryObject<TEntity> queryObject = null,
            Func<IQueryLite<TEntity>, IQueryLite<TEntity>> includesOverride = null,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
            bool withDefaults = false)
        {
            return SelectFirstAsync(queryObject?.Query(), includesOverride, orderBy, withDefaults);
        }

        public async Task<TDynamic> SelectFirstAsync(Expression<Func<TEntity, bool>> query = null,
            Func<IQueryLite<TEntity>, IQueryLite<TEntity>> includesOverride = null,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
            bool withDefaults = false)
        {
            return await DynamicMapper.FromEntityAsync(await SelectEntityFirstAsync(query, includesOverride, orderBy), withDefaults);
        }

        public Task<PagedList<TDynamic>> SelectPageAsync(int page, int pageSize,
            IQueryObject<TEntity> queryObject = null, Func<IQueryLite<TEntity>, IQueryLite<TEntity>> includesOverride = null,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
            bool withDefaults = false)
        {
            return SelectPageAsync(page, pageSize, queryObject?.Query(), includesOverride, orderBy, withDefaults);
        }

        public async Task<PagedList<TDynamic>> SelectPageAsync(int page, int pageSize, Expression<Func<TEntity, bool>> query = null,
            Func<IQueryLite<TEntity>, IQueryLite<TEntity>> includesOverride = null,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
            bool withDefaults = false)
        {
            var entities = await SelectEntityPageAsync(page, pageSize, query, includesOverride, orderBy);
            var list = await DynamicMapper.FromEntityRangeAsync(entities.Items, withDefaults);
            return new PagedList<TDynamic>(list, entities.Count);
        }

        #region Synchronous Operations

        public List<TDynamic> Select(Expression<Func<TEntity, bool>> query = null,

            Func<IQueryLite<TEntity>, IQueryLite<TEntity>> includesOverride = null, bool withDefaults = false)
        {
            return SelectAsync(query, includesOverride, withDefaults: withDefaults).GetAwaiter().GetResult();
        }

        public TDynamic SelectFirst(IQueryObject<TEntity> queryObject = null,
            Func<IQueryLite<TEntity>, IQueryLite<TEntity>> includesOverride = null,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
            bool withDefaults = false)
        {
            return SelectFirstAsync(queryObject?.Query(), includesOverride, orderBy, withDefaults).GetAwaiter().GetResult();
        }

        public TDynamic SelectFirst(Expression<Func<TEntity, bool>> query = null,
            Func<IQueryLite<TEntity>, IQueryLite<TEntity>> includesOverride = null,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
            bool withDefaults = false)
        {
            return SelectFirstAsync(query, includesOverride, orderBy, withDefaults).GetAwaiter().GetResult();
        }

        public PagedList<TDynamic> SelectPage(int page, int pageSize, Expression<Func<TEntity, bool>> query = null,
            Func<IQueryLite<TEntity>, IQueryLite<TEntity>> includesOverride = null,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
            bool withDefaults = false)
        {
            return SelectPageAsync(page, pageSize, query, includesOverride, orderBy, withDefaults).GetAwaiter().GetResult();
        }

        public PagedList<TDynamic> SelectPage(int page, int pageSize,
            IQueryObject<TEntity> queryObject = null, Func<IQueryLite<TEntity>, IQueryLite<TEntity>> includesOverride = null,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
            bool withDefaults = false)
        {
            return SelectPageAsync(page, pageSize, queryObject, includesOverride, orderBy, withDefaults).GetAwaiter().GetResult();
        }

        public TDynamic Select(int id, bool withDefaults = false, Func<IQueryLite<TEntity>, IQueryLite<TEntity>> includesOverride = null)
        {
            return SelectAsync(id, withDefaults, includesOverride).GetAwaiter().GetResult();
        }

        public List<TDynamic> Select(ICollection<int> ids, bool withDefaults = false,
            Func<IQueryLite<TEntity>, IQueryLite<TEntity>> includesOverride = null)
        {
            return SelectAsync(ids, withDefaults, includesOverride).GetAwaiter().GetResult();
        }

        public List<TDynamic> Select(IQueryObject<TEntity> queryObject = null,
            Func<IQueryLite<TEntity>, IQueryLite<TEntity>> includesOverride = null,
            bool withDefaults = false)
        {
            return SelectAsync(queryObject?.Query(), includesOverride, withDefaults: withDefaults).GetAwaiter().GetResult();
        }

        #endregion

        #region Helpers

        protected virtual IQueryFluent<TEntity> BuildQuery(IQueryFluent<TEntity> query)
        {
            return (BuildIncludes(new QueryLite<TEntity>(query)) as QueryLite<TEntity>)?.Query;
        }

        private async Task ProcessEntities(ICollection<TEntity> entities)
        {
            foreach (var entity in entities)
            {
                entity.OptionTypes = DynamicMapper.FilterByType(entity.IdObjectType);
            }
            //await SetBigValuesAsync(entities);
            await AfterSelect(entities);
        }

        private IQueryFluent<TEntity> BuildQueryFluent(Expression<Func<TEntity, bool>> query,
            Func<IQueryLite<TEntity>, IQueryLite<TEntity>> includesOverride, Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy)
        {
            var conditions = query ?? (p => p.StatusCode != (int) RecordStatusCode.Deleted);
            if (AdditionalDefaultConditions != null)
            {
                conditions = conditions.And(AdditionalDefaultConditions);
            }
            var queryFluent = CreateQuery(includesOverride ?? BuildIncludes, conditions);

            if (orderBy != null)
                queryFluent = queryFluent.OrderBy(orderBy);
            return queryFluent;
        }

        private IQueryFluent<TEntity> CreateQuery(Func<IQueryLite<TEntity>, IQueryLite<TEntity>> queryBuilder,
            Expression<Func<TEntity, bool>> condition = null)
        {
            if (condition == null)
                return
                    (queryBuilder(new QueryLite<TEntity>(ObjectRepository.Query().Include(p => p.OptionValues).ThenInclude(p => p.BigValue)))
                        as
                        QueryLite<TEntity>)?.Query;

            condition = (Expression<Func<TEntity, bool>>) QueryVisitor.Visit(condition);
            IQueryFluent<TEntity> res = ObjectRepository.Query(condition).Include(p => p.OptionValues).ThenInclude(p => p.BigValue);
            res = (queryBuilder(new QueryLite<TEntity>(res)) as QueryLite<TEntity>)?.Query;
            return res;
        }

        //protected async Task SetBigValuesAsync(IEnumerable<TEntity> entities, bool tracked = false)
        //{
        //    var bigValueIds = new Dictionary<long, List<TOptionValue>>();
        //    foreach (var value in entities.SelectMany(entity => entity.OptionValues.Where(v => v.IdBigString.HasValue && v.BigValue == null)))
        //    {
        //        List<TOptionValue> valuesExist;
        //        // ReSharper disable once PossibleInvalidOperationException
        //        // ReSharper disable once AssignNullToNotNullAttribute
        //        if (bigValueIds.TryGetValue(value.IdBigString.Value, out valuesExist))
        //        {
        //            valuesExist.Add(value);
        //        }
        //        else
        //        {
        //            bigValueIds.Add(value.IdBigString.Value, new List<TOptionValue> {value});
        //        }
        //    }
        //    if (bigValueIds.Any())
        //    {
        //        var bigValues =
        //            (await BigStringRepository.Query(b => bigValueIds.Keys.Contains(b.IdBigString)).SelectAsync(tracked));
        //        foreach (var bigValue in bigValues)
        //        {
        //            bigValueIds[bigValue.IdBigString].ForEach(v => v.BigValue = bigValue);
        //        }
        //    }
        //}

        //protected async Task SetBigValuesAsync(TEntity entity, bool tracked = false)
        //{
        //    var bigIdsList = entity.OptionValues.Where(v => v.IdBigString.HasValue && v.BigValue == null)
        //        .Select(v => v.IdBigString.Value)
        //        .ToList();
        //    if (bigIdsList.Any())
        //    {
        //        var bigValues =
        //            (await BigStringRepository.Query(b => bigIdsList.Contains(b.IdBigString)).SelectAsync(tracked))
        //                .ToDictionary
        //                (b => b.IdBigString, b => b);
        //        foreach (var value in entity.OptionValues)
        //        {
        //            if (value.IdBigString != null)
        //            {
        //                value.BigValue = bigValues[value.IdBigString.Value];
        //            }
        //        }
        //    }
        //}

        #endregion
    }
}