using Microsoft.Framework.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using VitalChoice.Data.Extensions;
using VitalChoice.Data.Helpers;
using VitalChoice.Data.Repositories;
using VitalChoice.Data.Services;
using VitalChoice.Domain.Entities;
using VitalChoice.Domain.Entities.eCommerce;
using VitalChoice.Domain.Entities.eCommerce.Base;
using VitalChoice.Domain.Entities.eCommerce.History;
using VitalChoice.Domain.Helpers;
using VitalChoice.Domain.Transfer.Base;
using VitalChoice.DynamicData.Helpers;
using VitalChoice.DynamicData.Interfaces;

namespace VitalChoice.DynamicData.Base
{
    public abstract /*partial*/ class DynamicReadServiceAsync<TDynamic, TEntity, TOptionType, TOptionValue> :
        IDynamicReadServiceAsync<TDynamic, TEntity>
        where TEntity : DynamicDataEntity<TOptionValue, TOptionType>, new()
        where TOptionType : OptionType, new()
        where TOptionValue : OptionValue<TOptionType>, new()
        where TDynamic : MappedObject, new()
    {
        protected readonly IDynamicMapper<TDynamic, TEntity, TOptionType, TOptionValue> Mapper;
        protected readonly IReadRepositoryAsync<TEntity> ObjectRepository;
        protected readonly IReadRepositoryAsync<TOptionType> OptionTypesRepository;
        protected readonly IReadRepositoryAsync<TOptionValue> OptionValuesRepository;
        protected readonly IReadRepositoryAsync<BigStringValue> BigStringRepository;
        protected readonly IObjectLogItemExternalService ObjectLogItemExternalService;
        protected readonly ILogger Logger;

        protected DynamicReadServiceAsync(
            IDynamicMapper<TDynamic, TEntity, TOptionType, TOptionValue> mapper,
            IReadRepositoryAsync<TEntity> objectRepository, IReadRepositoryAsync<TOptionType> optionTypesRepository,
            IReadRepositoryAsync<BigStringValue> bigStringRepository,
            IReadRepositoryAsync<TOptionValue> optionValuesRepository,
            IObjectLogItemExternalService objectLogItemExternalService,
            ILogger logger)
        {
            Mapper = mapper;
            ObjectRepository = objectRepository;
            OptionTypesRepository = optionTypesRepository;
            BigStringRepository = bigStringRepository;
            OptionValuesRepository = optionValuesRepository;
            ObjectLogItemExternalService = objectLogItemExternalService;
            Logger = logger;
        }

        #region Extension Points

        protected virtual Task AfterSelect(ICollection<TEntity> entity)
        {
            return Task.Delay(0);
        }

        protected virtual IQueryLite<TEntity> BuildQuery(IQueryLite<TEntity> query)
        {
            return query;
        }

        protected async Task<List<TEntity>> SelectEntitiesAsync(Expression<Func<TEntity, bool>> query = null,
            IDictionary<string, object> values = null,
            Func<IQueryLite<TEntity>, IQueryLite<TEntity>> includesOverride = null)
        {
            var optionTypes = await OptionTypesRepository.Query().SelectAsync(false);
            var queryFluent = await BuildQueryFluent(query, values, includesOverride, null, optionTypes);
            var entities = await queryFluent.SelectAsync(false);
            await ProcessEntities(entities, optionTypes);
            return entities;
        }

        protected async Task<TEntity> SelectEntityFirstAsync(Expression<Func<TEntity, bool>> query = null,
            IDictionary<string, object> values = null,
            Func<IQueryLite<TEntity>, IQueryLite<TEntity>> includesOverride = null,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null)
        {
            var optionTypes = await OptionTypesRepository.Query().SelectAsync(false);
            var queryFluent = await BuildQueryFluent(query, values, includesOverride, orderBy, optionTypes);
            var entity = await queryFluent.SelectFirstOrDefaultAsync(false);
            await ProcessEntities(new[] {entity}, optionTypes);
            return entity;
        }

        protected async Task<PagedList<TEntity>> SelectEntityPageAsync(int page, int pageSize,
            Expression<Func<TEntity, bool>> query = null,
            IDictionary<string, object> values = null,
            Func<IQueryLite<TEntity>, IQueryLite<TEntity>> includesOverride = null,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null)
        {
            var optionTypes = await OptionTypesRepository.Query().SelectAsync(false);
            var queryFluent = await BuildQueryFluent(query, values, includesOverride, orderBy, optionTypes);
            var entities = await queryFluent.SelectPageAsync(page, pageSize, false);
            await ProcessEntities(entities.Items, optionTypes);
            return entities;
        }

        #endregion

        public virtual async Task<TDynamic> CreatePrototypeAsync(int idObjectType)
        {
            var optionTypes = await OptionTypesRepository.Query(GetOptionTypeQuery(idObjectType)).SelectAsync(false);
            var entity = new TEntity {OptionTypes = optionTypes, IdObjectType = idObjectType};
            return await Mapper.FromEntityAsync(entity, true);
        }

        public virtual async Task<TModel> CreatePrototypeForAsync<TModel>(int idObjectType)
            where TModel : class, new()
        {
            return Mapper.ToModel<TModel>(await CreatePrototypeAsync(idObjectType));
        }

        public async Task<TDynamic> SelectAsync(int id, bool withDefaults = false,
            Func<IQueryLite<TEntity>, IQueryLite<TEntity>> includesOverride = null)
        {
            return
                await
                    Mapper.FromEntityAsync(
                        await
                            SelectEntityFirstAsync(o => o.Id == id && o.StatusCode != (int) RecordStatusCode.Deleted,
                                includesOverride: includesOverride), withDefaults);
        }

        public async Task<List<TDynamic>> SelectAsync(ICollection<int> ids, bool withDefaults = false,
            Func<IQueryLite<TEntity>, IQueryLite<TEntity>> includesOverride = null)
        {
            return
                await
                    Mapper.FromEntityRangeAsync(
                        await
                            SelectEntitiesAsync(o => ids.Contains(o.Id) && o.StatusCode != (int) RecordStatusCode.Deleted,
                                includesOverride: includesOverride), withDefaults);
        }

        public Task<List<TDynamic>> SelectAsync(IDictionary<string, object> values = null, IQueryObject<TEntity> queryObject = null,
            Func<IQueryLite<TEntity>, IQueryLite<TEntity>> includesOverride = null, bool withDefaults = false)
        {
            return SelectAsync(queryObject?.Query(), values, includesOverride, withDefaults);
        }

        public async Task<List<TDynamic>> SelectAsync(Expression<Func<TEntity, bool>> query = null,
            IDictionary<string, object> values = null,
            Func<IQueryLite<TEntity>, IQueryLite<TEntity>> includesOverride = null,
            bool withDefaults = false)
        {
            return await Mapper.FromEntityRangeAsync(await SelectEntitiesAsync(query, values, includesOverride), withDefaults);
        }

        public Task<TDynamic> SelectFirstAsync(IDictionary<string, object> values = null, IQueryObject<TEntity> queryObject = null,
            Func<IQueryLite<TEntity>, IQueryLite<TEntity>> includesOverride = null,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
            bool withDefaults = false)
        {
            return SelectFirstAsync(queryObject?.Query(), values, includesOverride, orderBy, withDefaults);
        }

        public async Task<TDynamic> SelectFirstAsync(Expression<Func<TEntity, bool>> query = null, IDictionary<string, object> values = null,
            Func<IQueryLite<TEntity>, IQueryLite<TEntity>> includesOverride = null,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
            bool withDefaults = false)
        {
            return await Mapper.FromEntityAsync(await SelectEntityFirstAsync(query, values, includesOverride, orderBy), withDefaults);
        }

        public Task<PagedList<TDynamic>> SelectPageAsync(int page, int pageSize, IDictionary<string, object> values = null,
            IQueryObject<TEntity> queryObject = null, Func<IQueryLite<TEntity>, IQueryLite<TEntity>> includesOverride = null,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
            bool withDefaults = false)
        {
            return SelectPageAsync(page, pageSize, queryObject?.Query(), values, includesOverride, orderBy, withDefaults);
        }

        public async Task<PagedList<TDynamic>> SelectPageAsync(int page, int pageSize, Expression<Func<TEntity, bool>> query = null,
            IDictionary<string, object> values = null, Func<IQueryLite<TEntity>, IQueryLite<TEntity>> includesOverride = null,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
            bool withDefaults = false)
        {
            var entities = await SelectEntityPageAsync(page, pageSize, query, values, includesOverride, orderBy);
            return new PagedList<TDynamic>(await Mapper.FromEntityRangeAsync(entities.Items, withDefaults), entities.Count);
        }

        #region Synchronous Operations

        public TDynamic CreatePrototype(int idObjectType)
        {
            var task = CreatePrototypeAsync(idObjectType);
            return task.Result;
        }

        public TModel CreatePrototypeFor<TModel>(int idObjectType)
            where TModel : class, new()
        {
            var task = CreatePrototypeForAsync<TModel>(idObjectType);
            return task.Result;
        }

        public List<TDynamic> Select(Expression<Func<TEntity, bool>> query = null,
            IDictionary<string, object> values = null,
            Func<IQueryLite<TEntity>, IQueryLite<TEntity>> includesOverride = null, bool withDefaults = false)
        {
            return SelectAsync(query, values, includesOverride, withDefaults).Result;
        }

        public TDynamic SelectFirst(IDictionary<string, object> values = null, IQueryObject<TEntity> queryObject = null,
            Func<IQueryLite<TEntity>, IQueryLite<TEntity>> includesOverride = null,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
            bool withDefaults = false)
        {
            return SelectFirstAsync(values, queryObject, includesOverride, orderBy, withDefaults).Result;
        }

        public TDynamic SelectFirst(Expression<Func<TEntity, bool>> query = null, IDictionary<string, object> values = null,
            Func<IQueryLite<TEntity>, IQueryLite<TEntity>> includesOverride = null,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
            bool withDefaults = false)
        {
            return SelectFirstAsync(query, values, includesOverride, orderBy, withDefaults).Result;
        }

        public PagedList<TDynamic> SelectPage(int page, int pageSize, Expression<Func<TEntity, bool>> query = null,
            IDictionary<string, object> values = null, Func<IQueryLite<TEntity>, IQueryLite<TEntity>> includesOverride = null,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
            bool withDefaults = false)
        {
            return SelectPageAsync(page, pageSize, query, values, includesOverride, orderBy, withDefaults).Result;
        }

        public PagedList<TDynamic> SelectPage(int page, int pageSize, IDictionary<string, object> values = null,
            IQueryObject<TEntity> queryObject = null, Func<IQueryLite<TEntity>, IQueryLite<TEntity>> includesOverride = null,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
            bool withDefaults = false)
        {
            return SelectPageAsync(page, pageSize, values, queryObject, includesOverride, orderBy, withDefaults).Result;
        }

        public TDynamic Select(int id, bool withDefaults = false, Func<IQueryLite<TEntity>, IQueryLite<TEntity>> includesOverride = null)
        {
            return SelectAsync(id, withDefaults, includesOverride).Result;
        }

        public List<TDynamic> Select(ICollection<int> ids, bool withDefaults = false,
            Func<IQueryLite<TEntity>, IQueryLite<TEntity>> includesOverride = null)
        {
            return SelectAsync(ids, withDefaults, includesOverride).Result;
        }

        public List<TDynamic> Select(IDictionary<string, object> values = null, IQueryObject<TEntity> queryObject = null,
            Func<IQueryLite<TEntity>, IQueryLite<TEntity>> includesOverride = null,
            bool withDefaults = false)
        {
            return SelectAsync(queryObject?.Query(), values, includesOverride, withDefaults).Result;
        }

        #endregion

        #region Helpers

        protected IQueryFluent<TEntity> BuildQuery(IQueryFluent<TEntity> query)
        {
            return (BuildQuery(new QueryLite<TEntity>(query)) as QueryLite<TEntity>)?.Query;
        }

        public IQueryOptionType<TOptionType> GetOptionTypeQuery(TEntity entity)
        {
            return Mapper.GetOptionTypeQuery().WithObjectType(entity.IdObjectType);
        }

        public IQueryOptionType<TOptionType> GetOptionTypeQuery(int? idObjectType)
        {
            return Mapper.GetOptionTypeQuery().WithObjectType(idObjectType);
        }

        private async Task ProcessEntities(ICollection<TEntity> entities, List<TOptionType> optionTypes)
        {
            foreach (var entity in entities)
            {
                entity.OptionTypes = optionTypes.Where(GetOptionTypeQuery(entity).Query().CacheCompile()).ToList();
            }
            await SetBigValuesAsync(entities);
            await AfterSelect(entities);
        }

        private async Task<IQueryFluent<TEntity>> BuildQueryFluent(Expression<Func<TEntity, bool>> query,
            IDictionary<string, object> values, Func<IQueryLite<TEntity>, IQueryLite<TEntity>> includesOverride,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy, List<TOptionType> optionTypes)
        {
            IQueryFluent<TEntity> queryFluent;
            if (values != null)
            {
                var searchValues = BuildSearchValues(values, optionTypes);
                var valuesSelector = CreateValuesSelector(searchValues);
                var optionValues =
                    (await
                        OptionValuesRepository.Query(valuesSelector)
                            .SelectAsync(Mapper.ObjectIdSelector))
                        .Distinct().ToList();
                if (query != null)
                {
                    queryFluent = CreateQuery(includesOverride ?? BuildQuery,
                        query.And(p => optionValues.Contains(p.Id)));
                }
                else
                {
                    queryFluent = CreateQuery(includesOverride ?? BuildQuery,
                        p => p.StatusCode != (int) RecordStatusCode.Deleted && optionValues.Contains(p.Id));
                }
            }
            else
            {
                queryFluent = CreateQuery(includesOverride ?? BuildQuery,
                    query ?? (p => p.StatusCode != (int) RecordStatusCode.Deleted));
            }
            if (orderBy != null)
                queryFluent = queryFluent.OrderBy(orderBy);
            return queryFluent;
        }

        private Dictionary<int, GenericPair<string, TOptionType>> BuildSearchValues(IDictionary<string, object> values,
            List<TOptionType> optionTypes)
        {
            var optionTypesToSearch =
                optionTypes.Where(Mapper.GetOptionTypeQuery().WithNames(values.Keys).Query().CacheCompile());
            Dictionary<int, GenericPair<string, TOptionType>> searchValues = optionTypesToSearch.ToDictionary(t => t.Id,
                t =>
                    new GenericPair<string, TOptionType>(
                        MapperTypeConverter.ConvertToOptionValue(values[t.Name], (FieldType) t.IdFieldType), t));
            return searchValues;
        }

        private IQueryFluent<TEntity> CreateQuery(Func<IQueryLite<TEntity>, IQueryLite<TEntity>> queryBuilder,
            Expression<Func<TEntity, bool>> condition = null)
        {
            if (condition == null)
                return
                    (queryBuilder(new QueryLite<TEntity>(ObjectRepository.Query().Include(p => p.OptionValues))) as
                        QueryLite<TEntity>)?.Query;

            IQueryFluent<TEntity> res = ObjectRepository.Query(condition).Include(p => p.OptionValues);
            res = (queryBuilder(new QueryLite<TEntity>(res)) as QueryLite<TEntity>)?.Query;
            return res;
        }

        private static Expression<Func<TOptionValue, bool>> CreateValuesSelector(
            Dictionary<int, GenericPair<string, TOptionType>> searchValues)
        {
            Expression<Func<TOptionValue, bool>> valuesSelector = null;
            foreach (var searchPair in searchValues)
            {
                if (valuesSelector == null)
                {
                    if ((FieldType) searchPair.Value.Value2.IdFieldType == FieldType.String)
                    {
                        valuesSelector =
                            v => v.IdOptionType == searchPair.Key && v.Value.Contains(searchPair.Value.Value1);
                    }
                    else
                    {
                        valuesSelector =
                            v => v.IdOptionType == searchPair.Key && v.Value == searchPair.Value.Value1;
                    }
                }
                else
                {
                    if ((FieldType) searchPair.Value.Value2.IdFieldType == FieldType.String)
                    {
                        valuesSelector =
                            valuesSelector.Or(
                                v => v.IdOptionType == searchPair.Key && v.Value.Contains(searchPair.Value.Value1));
                    }
                    else
                    {
                        valuesSelector =
                            valuesSelector.Or(
                                v => v.IdOptionType == searchPair.Key && v.Value == searchPair.Value.Value1);
                    }
                }
            }
            return valuesSelector;
        }

        protected async Task SetBigValuesAsync(IEnumerable<TEntity> entities, bool tracked = false)
        {
            var bigValueIds = new Dictionary<long, List<TOptionValue>>();
            foreach (var value in entities.SelectMany(entity => entity.OptionValues.Where(v => v.IdBigString.HasValue)))
            {
                List<TOptionValue> valuesExist;
                // ReSharper disable once PossibleInvalidOperationException
                // ReSharper disable once AssignNullToNotNullAttribute
                if (bigValueIds.TryGetValue(value.IdBigString.Value, out valuesExist))
                {
                    valuesExist.Add(value);
                }
                else
                {
                    bigValueIds.Add(value.IdBigString.Value, new List<TOptionValue> {value});
                }
            }
            if (bigValueIds.Any())
            {
                var bigValues =
                    (await BigStringRepository.Query(b => bigValueIds.Keys.Contains(b.IdBigString)).SelectAsync(tracked));
                foreach (var bigValue in bigValues)
                {
                    bigValueIds[bigValue.IdBigString].ForEach(v => v.BigValue = bigValue);
                }
            }
        }

        protected async Task SetBigValuesAsync(TEntity entity, bool tracked = false)
        {
            var bigIdsList = entity.OptionValues.Where(v => v.IdBigString.HasValue)
                .Select(v => v.IdBigString.Value)
                .ToList();
            if (bigIdsList.Any())
            {
                var bigValues =
                    (await BigStringRepository.Query(b => bigIdsList.Contains(b.IdBigString)).SelectAsync(tracked))
                        .ToDictionary
                        (b => b.IdBigString, b => b);
                foreach (var value in entity.OptionValues)
                {
                    if (value.IdBigString != null)
                    {
                        value.BigValue = bigValues[value.IdBigString.Value];
                    }
                }
            }
        }

        #endregion
    }
}