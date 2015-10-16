using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using VitalChoice.Data.Extensions;
using VitalChoice.Data.Helpers;
using VitalChoice.Data.Repositories;
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
        protected readonly IReadRepositoryAsync<ObjectHistoryLogItem> ObjectHistoryLogItemRepository;

        protected ReadDynamicObjectServiceAsync(
            IDynamicObjectMapper<TDynamic, TEntity, TOptionType, TOptionValue> mapper,
            IReadRepositoryAsync<TEntity> objectRepository, IReadRepositoryAsync<TOptionType> optionTypesRepository,
            IReadRepositoryAsync<BigStringValue> bigStringRepository, IReadRepositoryAsync<TOptionValue> optionValuesRepository,
            IReadRepositoryAsync<ObjectHistoryLogItem> objectHistoryLogItemRepository)
        {
            Mapper = mapper;
            ObjectRepository = objectRepository;
            OptionTypesRepository = optionTypesRepository;
            BigStringRepository = bigStringRepository;
            OptionValuesRepository = optionValuesRepository;
            ObjectHistoryLogItemRepository = objectHistoryLogItemRepository;
        }

        #region Extension Points

        public abstract ObjectType IdObjectType { get;}

        protected virtual Task AfterSelect(TEntity entity)
        {
            return Task.Delay(0);
        }

        protected virtual Task AfterSelect(List<TEntity> entity)
        {
            return Task.Delay(0);
        }

        protected virtual IQueryFluent<TEntity> BuildQuery(IQueryFluent<TEntity> query)
        {
            return query;
        }

        protected virtual async Task<TDynamic> SelectItemAsync(
            Func<IQueryFluent<TEntity>, IQueryFluent<TEntity>> queryBuilder, Expression<Func<TEntity, bool>> condition, bool withDefaults)
        {
            if (queryBuilder == null)
                throw new ArgumentNullException(nameof(queryBuilder));

            var res = CreateQuery(queryBuilder, condition);
            return await SelectItemInternal(res, withDefaults);
        }

        protected virtual async Task<List<TDynamic>> SelectListAsync(
            Func<IQueryFluent<TEntity>, IQueryFluent<TEntity>> queryBuilder, Expression<Func<TEntity, bool>> condition, bool withDefaults)
        {
            if (queryBuilder == null)
                throw new ArgumentNullException(nameof(queryBuilder));

            var res = CreateQuery(queryBuilder, condition);
            return await SelectListInternal(res, withDefaults);
        }

        #endregion

        public virtual async Task<TDynamic> CreatePrototypeAsync(int? idObjectType = null)
        {
            var optionTypes = await OptionTypesRepository.Query(GetOptionTypeQuery(idObjectType)).SelectAsync(false);
            var entity = new TEntity {OptionTypes = optionTypes, IdObjectType = idObjectType };
            return await Mapper.FromEntityAsync(entity, true);
        }

        public virtual async Task<TModel> CreatePrototypeForAsync<TModel>(int? idObjectType = null) 
            where TModel : class, new()
        {
            return Mapper.ToModel<TModel>(await CreatePrototypeAsync(idObjectType));
        }

        public async Task<TDynamic> SelectAsync(int id, bool withDefaults)
        {
            return
                await SelectItemAsync(BuildQuery, p => p.Id == id && p.StatusCode != (int)RecordStatusCode.Deleted, withDefaults);
        }

        public async Task<List<TDynamic>> SelectAsync(ICollection<int> ids, bool withDefaults)
        {
            return
                await
                    SelectListAsync(BuildQuery, p => ids.Contains(p.Id) && p.StatusCode != (int)RecordStatusCode.Deleted,
                        withDefaults);
        }

        public async Task<List<TDynamic>> SelectAsync(bool withDefaults)
        {
            return
                await
                    SelectListAsync(BuildQuery, p => p.StatusCode != (int)RecordStatusCode.Deleted, withDefaults);
        }

        public async Task<List<TDynamic>> SelectAsync(IQueryObject<TEntity> queryObject, bool withDefaults)
        {
            if (queryObject == null)
                throw new ArgumentNullException(nameof(queryObject));

            return
                await
                    SelectListAsync(BuildQuery, queryObject.Query(), withDefaults);
        }

        public async Task<List<TDynamic>> SelectAsync(Expression<Func<TEntity, bool>> query, bool withDefaults)
        {
            return
                await
                    SelectListAsync(BuildQuery, query, withDefaults);
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
            return
                await
                    SelectListAsync(BuildQuery,
                        p => p.StatusCode != (int)RecordStatusCode.Deleted && optionValues.Contains(p.Id), withDefaults);
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
            return
                await
                    SelectListAsync(BuildQuery,
                        query.And(p => p.StatusCode != (int)RecordStatusCode.Deleted).And(p => optionValues.Contains(p.Id)),
                        withDefaults);
        }

        public TDynamic CreatePrototype(int? idObjectType = null)
        {
            var task = CreatePrototypeAsync(idObjectType);
            task.Wait();
            return task.Result;
        }

        public TModel CreatePrototypeFor<TModel>(int? idObjectType = null)
            where TModel : class, new()
        {
            var task = CreatePrototypeForAsync<TModel>(idObjectType);
            task.Wait();
            return task.Result;
        }

        #region Synchronous Operations

        public TDynamic Select(int id, bool withDefaults)
        {
            var task = SelectAsync(id, withDefaults);
            task.Wait();
            return task.Result;
        }

        public List<TDynamic> Select(ICollection<int> ids, bool withDefaults)
        {
            var task = SelectAsync(ids, withDefaults);
            task.Wait();
            return task.Result;
        }

        public List<TDynamic> Select(bool withDefaults)
        {
            var task = SelectAsync(withDefaults);
            task.Wait();
            return task.Result;
        }

        public List<TDynamic> Select(IQueryObject<TEntity> queryObject, bool withDefaults)
        {
            var task = SelectAsync(queryObject, withDefaults);
            task.Wait();
            return task.Result;
        }

        public List<TDynamic> Select(Expression<Func<TEntity, bool>> query, bool withDefaults)
        {
            var task = SelectAsync(query, withDefaults);
            task.Wait();
            return task.Result;
        }

        public List<TDynamic> Select(IDictionary<string, object> values, bool withDefaults)
        {
            var task = SelectAsync(values, withDefaults);
            task.Wait();
            return task.Result;
        }

        public List<TDynamic> Select(IDictionary<string, object> values, Expression<Func<TEntity, bool>> query, bool withDefaults)
        {
            var task = SelectAsync(values, query, withDefaults);
            task.Wait();
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

        private async Task<List<TDynamic>> SelectListInternal(IQueryFluent<TEntity> res, bool withDefaults)
        {
            var entities = await res.SelectAsync(false);
            var optionTypes = await OptionTypesRepository.Query().SelectAsync(false);
            foreach (var entity in entities)
            {
                entity.OptionTypes = optionTypes.Where(GetOptionTypeQuery(entity).Query().Compile()).ToList();
            }
            await SetBigValuesAsync(entities);
            await AfterSelect(entities);
            return Mapper.FromEntityRange(entities, withDefaults);
        }

        private async Task<TDynamic> SelectItemInternal(IQueryFluent<TEntity> res, bool withDefaults)
        {
            var entity = (await res.SelectAsync(false)).FirstOrDefault();

            if (entity != null)
            {
                await SetBigValuesAsync(entity);
                entity.OptionTypes = await OptionTypesRepository.Query(GetOptionTypeQuery(entity)).SelectAsync(false);
                await AfterSelect(entity);
                return Mapper.FromEntity(entity, withDefaults);
            }
            return null;
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