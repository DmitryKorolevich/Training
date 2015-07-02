using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using VitalChoice.Data.Helpers;
using VitalChoice.Data.Repositories;
using VitalChoice.Data.UnitOfWork;
using VitalChoice.Domain.Entities;
using VitalChoice.Domain.Entities.eCommerce.Base;
using VitalChoice.DynamicData.Interfaces;
using VitalChoice.DynamicData.Interfaces.Services;

namespace VitalChoice.DynamicData.Base
{
    public abstract class ReadDynamicObjectRepositoryAsync<TDynamic, TEntity, TOptionType, TOptionValue> : IReadDynamicObjectRepositoryAsync<TDynamic, TEntity>
        where TEntity : DynamicDataEntity<TOptionValue, TOptionType>, new()
        where TOptionType : OptionType, new()
        where TOptionValue : OptionValue<TOptionType>, new()
        where TDynamic : MappedObject, new()
    {
        protected readonly IReadRepositoryAsync<TEntity> ObjectRepository;
        protected readonly IDynamicObjectMapper<TDynamic, TEntity, TOptionType, TOptionValue> Mapper;
        protected readonly IReadRepositoryAsync<TOptionType> OptionTypesRepository;
        protected readonly IReadRepositoryAsync<BigStringValue> BigStringRepository;
        private readonly IUnitOfWorkAsync _uow;

        protected abstract IQueryObject<TOptionType> OptionTypeQuery { get; }

        protected ReadDynamicObjectRepositoryAsync(IDynamicObjectMapper<TDynamic, TEntity, TOptionType, TOptionValue> mapper, IUnitOfWorkAsync readUow)
        {
            Mapper = mapper;
            _uow = readUow;
            ObjectRepository = readUow.ReadRepositoryAsync<TEntity>();
            OptionTypesRepository = readUow.ReadRepositoryAsync<TOptionType>();
            BigStringRepository = readUow.ReadRepositoryAsync<BigStringValue>();
        }

        public async Task<TDynamic> SelectAsync(int id)
        {
            return await SelectAsync(id, false);
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

        public virtual Task AfterSelect(TEntity entity)
        {
            return Task.Delay(0);
        }

        public async Task<TDynamic> SelectAsync(int id, bool withDefaults)
        {
            IQueryFluent<TEntity> res = ObjectRepository.Query(
                p => p.Id == id && p.StatusCode != RecordStatusCode.Deleted)
                .Include(p => p.OptionValues);
            var entity = (await res.SelectAsync(false)).FirstOrDefault();

            if (entity != null)
            {
                await SetBigValuesAsync(entity, BigStringRepository);
                entity.OptionTypes = await OptionTypesRepository.Query(OptionTypeQuery).SelectAsync(false);
                await AfterSelect(entity);
                return Mapper.FromEntity(entity, withDefaults);
            }

            return null;
        }

        public Task<List<TDynamic>> SelectAsync(bool withDefaults)
        {
            throw new NotImplementedException();
        }

        public Task<List<TDynamic>> SelectAsync(IQueryObject<TEntity> queryObject, bool withDefaults)
        {
            throw new NotImplementedException();
        }

        public Task<List<TDynamic>> SelectAsync(Expression<Func<TEntity, bool>> query, bool withDefaults)
        {
            throw new NotImplementedException();
        }

        public TDynamic Select(int id)
        {
            var task = SelectAsync(id);
            task.Wait();
            return task.Result;
        }

        public List<TDynamic> Select()
        {
            throw new NotImplementedException();
        }

        public List<TDynamic> Select(IQueryObject<TEntity> queryObject)
        {
            throw new NotImplementedException();
        }

        public List<TDynamic> Select(Expression<Func<TEntity, bool>> query)
        {
            throw new NotImplementedException();
        }

        

        public TDynamic Select(int id, bool withDefaults)
        {
            var task = SelectAsync(id, withDefaults);
            task.Wait();
            return task.Result;
        }

        public List<TDynamic> Select(bool withDefaults)
        {
            throw new NotImplementedException();
        }

        public List<TDynamic> Select(IQueryObject<TEntity> queryObject, bool withDefaults)
        {
            throw new NotImplementedException();
        }

        public List<TDynamic> Select(Expression<Func<TEntity, bool>> query, bool withDefaults)
        {
            throw new NotImplementedException();
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

        #region IDisposable
        private bool _disposedValue;

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposedValue)
            {
                if (disposing)
                {
                    _uow.Dispose();
                }

                _disposedValue = true;
            }
        }

        public void Dispose()
        {
            Dispose(true);
        }
        #endregion
    }
}