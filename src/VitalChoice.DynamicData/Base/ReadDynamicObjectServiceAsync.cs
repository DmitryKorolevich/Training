using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using VitalChoice.Data.Helpers;
using VitalChoice.Data.Repositories;
using VitalChoice.Domain.Entities;
using VitalChoice.Domain.Entities.eCommerce.Base;
using VitalChoice.DynamicData.Interfaces;

namespace VitalChoice.DynamicData.Base
{
    public abstract class ReadDynamicObjectServiceAsync<TDynamic, TEntity, TOptionType, TOptionValue> : IReadDynamicObjectRepositoryAsync<TDynamic, TEntity>
        where TEntity : DynamicDataEntity<TOptionValue, TOptionType>, new()
        where TOptionType : OptionType, new()
        where TOptionValue : OptionValue<TOptionType>, new()
        where TDynamic : MappedObject, new()
    {
        protected readonly IReadRepositoryAsync<TEntity> ObjectRepository;
        protected readonly IDynamicObjectMapper<TDynamic, TEntity, TOptionType, TOptionValue> Mapper;
        protected readonly IReadRepositoryAsync<TOptionType> OptionTypesRepository;
        protected readonly IReadRepositoryAsync<BigStringValue> BigStringRepository;

        public IQueryObject<TOptionType> GetOptionTypeQuery(TEntity entity)
        {
            return Mapper.GetOptionTypeQuery(entity.IdObjectType);
        }

        public IQueryObject<TOptionType> GetOptionTypeQuery(int? idObjectType)
        {
            return Mapper.GetOptionTypeQuery(idObjectType);
        }

        protected ReadDynamicObjectServiceAsync(
            IDynamicObjectMapper<TDynamic, TEntity, TOptionType, TOptionValue> mapper,
            IReadRepositoryAsync<TEntity> objectRepository, IReadRepositoryAsync<TOptionType> optionTypesRepository,
            IReadRepositoryAsync<BigStringValue> bigStringRepository)
        {
            Mapper = mapper;
            ObjectRepository = objectRepository;
            OptionTypesRepository = optionTypesRepository;
            BigStringRepository = bigStringRepository;
        }

        public async Task<TDynamic> SelectAsync(int id)
        {
            return await SelectAsync(id, false);
        }

        public async Task<List<TDynamic>> SelectAsync(ICollection<int> ids)
        {
            return await SelectAsync(ids, false);
        }

        public async Task<List<TDynamic>> SelectAsync()
        {
            return await SelectAsync(false);
        }

        public async Task<List<TDynamic>> SelectAsync(IQueryObject<TEntity> queryObject)
        {
            return await SelectAsync(queryObject, false);
        }

        public async Task<List<TDynamic>> SelectAsync(Expression<Func<TEntity, bool>> query)
        {
            return await SelectAsync(query, false);
        }

        protected virtual IQueryFluent<TEntity> BuildQuery(IQueryFluent<TEntity> query)
        {
            return query;
        }

        protected virtual Task AfterSelect(TEntity entity)
        {
            return Task.Delay(0);
        }

        public async Task<TDynamic> SelectAsync(int id, bool withDefaults)
        {
            var entity = await SelectEntityAsync(id);
            if (entity != null)
            {
                return Mapper.FromEntity(entity, withDefaults);
            }

            return null;
        }

        protected virtual async Task<TEntity> SelectEntityAsync(int id)
        {
            IQueryFluent<TEntity> res = ObjectRepository.Query(
                p => p.Id == id && p.StatusCode != RecordStatusCode.Deleted)
                .Include(p => p.OptionValues);
            res = BuildQuery(res);
            var entity = (await res.SelectAsync(false)).FirstOrDefault();

            if (entity != null)
            {
                await SetBigValuesAsync(entity, BigStringRepository);
                entity.OptionTypes = await OptionTypesRepository.Query(GetOptionTypeQuery(entity)).SelectAsync(false);
                await AfterSelect(entity);
                return entity;
            }

            return null;
        }

        public virtual async Task<List<TDynamic>> SelectAsync(ICollection<int> ids, bool withDefaults)
        {
            IQueryFluent<TEntity> res = ObjectRepository.Query(
                p => ids.Contains(p.Id) && p.StatusCode != RecordStatusCode.Deleted)
                .Include(p => p.OptionValues);
            res = BuildQuery(res);
            var entities = await res.SelectAsync(false);
            foreach (var entity in entities)
            {
                await SetBigValuesAsync(entity, BigStringRepository);
                entity.OptionTypes = await OptionTypesRepository.Query(GetOptionTypeQuery(entity)).SelectAsync(false);
                await AfterSelect(entity);
            }
            return Mapper.FromEntityRange(entities, withDefaults);
        }

        public virtual Task<List<TDynamic>> SelectAsync(bool withDefaults)
        {
            throw new NotImplementedException();
        }

        public virtual Task<List<TDynamic>> SelectAsync(IQueryObject<TEntity> queryObject, bool withDefaults)
        {
            throw new NotImplementedException();
        }

        public virtual Task<List<TDynamic>> SelectAsync(Expression<Func<TEntity, bool>> query, bool withDefaults)
        {
            throw new NotImplementedException();
        }

        public TDynamic Select(int id)
        {
            var task = SelectAsync(id);
            task.Wait();
            return task.Result;
        }

        public List<TDynamic> Select(ICollection<int> ids)
        {
            var task = SelectAsync(ids);
            task.Wait();
            return task.Result;
        }

        public List<TDynamic> Select()
        {
            var task = SelectAsync();
            task.Wait();
            return task.Result;
        }

        public List<TDynamic> Select(IQueryObject<TEntity> queryObject)
        {
            var task = SelectAsync(queryObject);
            task.Wait();
            return task.Result;
        }

        public List<TDynamic> Select(Expression<Func<TEntity, bool>> query)
        {
            var task = SelectAsync(query);
            task.Wait();
            return task.Result;
        }
        

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

        protected static async Task SetBigValuesAsync(TEntity entity, IReadRepositoryAsync<BigStringValue> bigStringValueRepository, bool tracked = false)
        {
            var bigIdsList = entity.OptionValues.Where(v => v.IdBigString != null)
                .Select(v => v.IdBigString.Value)
                .ToList();
            if (bigIdsList.Any())
            {
                var bigValues =
                    (await bigStringValueRepository.Query(b => bigIdsList.Contains(b.IdBigString)).SelectAsync(tracked))
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
    }
}