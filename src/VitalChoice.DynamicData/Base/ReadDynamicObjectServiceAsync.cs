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
using VitalChoice.DynamicData.Helpers;
using VitalChoice.DynamicData.Interfaces;

namespace VitalChoice.DynamicData.Base
{
    public abstract partial class ReadDynamicObjectServiceAsync<TDynamic, TEntity, TOptionType, TOptionValue> : IReadDynamicObjectServiceAsync<TDynamic, TEntity>
        where TEntity : DynamicDataEntity<TOptionValue, TOptionType>, new()
        where TOptionType : OptionType, new()
        where TOptionValue : OptionValue<TOptionType>, new()
        where TDynamic : MappedObject, new()
    {
        protected readonly IDynamicObjectMapper<TDynamic, TEntity, TOptionType, TOptionValue> Mapper;
        protected readonly IReadRepositoryAsync<TEntity> ObjectRepository;
        protected readonly IReadRepositoryAsync<TOptionType> OptionTypesRepository;
        protected readonly IReadRepositoryAsync<TOptionValue> OptionValuesRepository;
        protected readonly IReadRepositoryAsync<BigStringValue> BigStringRepository;
        protected readonly IObjectLogItemExternalService ObjectLogItemExternalService;
        protected readonly ILogger Logger;

        protected ReadDynamicObjectServiceAsync(
            IDynamicObjectMapper<TDynamic, TEntity, TOptionType, TOptionValue> mapper,
            IReadRepositoryAsync<TEntity> objectRepository, IReadRepositoryAsync<TOptionType> optionTypesRepository,
            IReadRepositoryAsync<BigStringValue> bigStringRepository, IReadRepositoryAsync<TOptionValue> optionValuesRepository,
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
        
        protected virtual Task AfterSelect(List<TEntity> entity)
        {
            return Task.Delay(0);
        }

        protected virtual IQueryFluent<TEntity> BuildQuery(IQueryFluent<TEntity> query)
        {
            return query;
        }

        #endregion

        public virtual async Task<TDynamic> CreatePrototypeAsync(int idObjectType)
        {
            var optionTypes = await OptionTypesRepository.Query(GetOptionTypeQuery(idObjectType)).SelectAsync(false);
            var entity = new TEntity {OptionTypes = optionTypes, IdObjectType = idObjectType };
            return await Mapper.FromEntityAsync(entity, true);
        }

        public virtual async Task<TModel> CreatePrototypeForAsync<TModel>(int idObjectType) 
            where TModel : class, new()
        {
            return Mapper.ToModel<TModel>(await CreatePrototypeAsync(idObjectType));
        }

        public async Task<TDynamic> SelectAsync(int id, bool withDefaults)
        {
            return await Mapper.FromEntityAsync(await SelectEntityAsync(id), withDefaults);
        }

        public async Task<List<TDynamic>> SelectAsync(ICollection<int> ids, bool withDefaults)
        {
            return await Mapper.FromEntityRangeAsync(await SelectEntityListAsync(ids), withDefaults);
        }

        public async Task<List<TDynamic>> SelectAsync(bool withDefaults)
        {
            var res = CreateQuery(BuildQuery, p => p.StatusCode != (int)RecordStatusCode.Deleted);
            return await Mapper.FromEntityRangeAsync(await SelectListAsync(res), withDefaults);
        }

        public async Task<List<TDynamic>> SelectAsync(IQueryObject<TEntity> queryObject, bool withDefaults)
        {
            if (queryObject == null)
                throw new ArgumentNullException(nameof(queryObject));
            var res = CreateQuery(BuildQuery, queryObject.Query());
            return await Mapper.FromEntityRangeAsync(await SelectListAsync(res), withDefaults);
        }

        public async Task<List<TDynamic>> SelectAsync(Expression<Func<TEntity, bool>> query, bool withDefaults)
        {
            var res = CreateQuery(BuildQuery, query);
            return await Mapper.FromEntityRangeAsync(await SelectListAsync(res), withDefaults);
        }

        public async Task<List<TDynamic>> SelectAsync(IDictionary<string, object> values, bool withDefaults)
        {
            var optionTypes = await OptionTypesRepository.Query().SelectAsync(false);
            var searchValues = BuildSearchValues(values, optionTypes);
            var valuesSelector = CreateValuesSelector(searchValues);
            var optionValues =
                (await
                    OptionValuesRepository.Query(valuesSelector)
                        .SelectAsync(Mapper.ObjectIdSelector)).Distinct().ToList();
            var res = CreateQuery(BuildQuery, p => p.StatusCode != (int)RecordStatusCode.Deleted && optionValues.Contains(p.Id));
            return await Mapper.FromEntityRangeAsync(await SelectListAsync(res), withDefaults);
        }

        public async Task<List<TDynamic>> SelectAsync(IDictionary<string, object> values,
            Expression<Func<TEntity, bool>> query,
            bool withDefaults)
        {
            var optionTypes = await OptionTypesRepository.Query().SelectAsync(false);
            var searchValues = BuildSearchValues(values, optionTypes);
            var valuesSelector = CreateValuesSelector(searchValues);
            var optionValues =
                (await
                    OptionValuesRepository.Query(valuesSelector)
                        .SelectAsync(Mapper.ObjectIdSelector))
                    .Distinct().ToList();
            var res = CreateQuery(BuildQuery,
                query.And(p => p.StatusCode != (int) RecordStatusCode.Deleted).And(p => optionValues.Contains(p.Id)));
            return await Mapper.FromEntityRangeAsync(await SelectListAsync(res), withDefaults);
        }

        protected virtual async Task<List<TEntity>> SelectListAsync(IQueryFluent<TEntity> query)
        {
            var entities = await query.SelectAsync(false);
            var optionTypes = await OptionTypesRepository.Query().SelectAsync(false);
            foreach (var entity in entities)
            {
                entity.OptionTypes = optionTypes.Where(GetOptionTypeQuery(entity).Query().Compile()).ToList();
            }
            await SetBigValuesAsync(entities);
            await AfterSelect(entities);
            return entities;
        }

        protected virtual async Task<TEntity> SelectItemAsync(IQueryFluent<TEntity> query)
        {
            var entity = (await query.SelectAsync(false)).FirstOrDefault();

            if (entity != null)
            {
                await SetBigValuesAsync(entity);
                entity.OptionTypes = await OptionTypesRepository.Query(GetOptionTypeQuery(entity)).SelectAsync(false);
                await AfterSelect(new List<TEntity> {entity});
                return entity;
            }
            return null;
        }

        protected async Task<TEntity> SelectEntityAsync(int id)
        {
            var res = CreateQuery(BuildQuery, p => p.Id == id && p.StatusCode != (int)RecordStatusCode.Deleted);
            return await SelectItemAsync(res);
        }

        protected async Task<List<TEntity>> SelectEntityListAsync(ICollection<int> ids)
        {
            var res = CreateQuery(BuildQuery, p => ids.Contains(p.Id) && p.StatusCode != (int)RecordStatusCode.Deleted);
            return await SelectListAsync(res);
        }

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

        #region Synchronous Operations

        public TDynamic Select(int id, bool withDefaults)
        {
            var task = SelectAsync(id, withDefaults);
            return task.Result;
        }

        public List<TDynamic> Select(ICollection<int> ids, bool withDefaults)
        {
            var task = SelectAsync(ids, withDefaults);
            return task.Result;
        }

        public List<TDynamic> Select(bool withDefaults)
        {
            var task = SelectAsync(withDefaults);
            return task.Result;
        }

        public List<TDynamic> Select(IQueryObject<TEntity> queryObject, bool withDefaults)
        {
            var task = SelectAsync(queryObject, withDefaults);
            return task.Result;
        }

        public List<TDynamic> Select(Expression<Func<TEntity, bool>> query, bool withDefaults)
        {
            var task = SelectAsync(query, withDefaults);
            return task.Result;
        }

        public List<TDynamic> Select(IDictionary<string, object> values, bool withDefaults)
        {
            var task = SelectAsync(values, withDefaults);
            return task.Result;
        }

        public List<TDynamic> Select(IDictionary<string, object> values, Expression<Func<TEntity, bool>> query, bool withDefaults)
        {
            var task = SelectAsync(values, query, withDefaults);
            return task.Result;
        }

        #endregion

        #region Helpers

        public IQueryOptionType<TOptionType> GetOptionTypeQuery(TEntity entity)
        {
            return Mapper.GetOptionTypeQuery().WithObjectType(entity.IdObjectType);
        }

        public IQueryOptionType<TOptionType> GetOptionTypeQuery(int? idObjectType)
        {
            return Mapper.GetOptionTypeQuery().WithObjectType(idObjectType);
        }

        private Dictionary<int, GenericPair<string, TOptionType>> BuildSearchValues(IDictionary<string, object> values,
            List<TOptionType> optionTypes)
        {
            var optionTypesToSearch =
                optionTypes.Where(Mapper.GetOptionTypeQuery().WithNames(values.Keys).Query().Compile());
            Dictionary<int, GenericPair<string, TOptionType>> searchValues = optionTypesToSearch.ToDictionary(t => t.Id,
                t =>
                    new GenericPair<string, TOptionType>(
                        MapperTypeConverter.ConvertToOptionValue(values[t.Name], (FieldType) t.IdFieldType), t));
            return searchValues;
        }

        private IQueryFluent<TEntity> CreateQuery(Func<IQueryFluent<TEntity>, IQueryFluent<TEntity>> queryBuilder, Expression<Func<TEntity, bool>> condition = null)
        {
            if (condition == null)
                return queryBuilder(ObjectRepository.Query().Include(p => p.OptionValues));

            IQueryFluent<TEntity> res = ObjectRepository.Query(condition).Include(p => p.OptionValues);
            res = queryBuilder(res);
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