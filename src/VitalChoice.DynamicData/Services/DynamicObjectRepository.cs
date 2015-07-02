using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using VitalChoice.Data.Helpers;
using VitalChoice.Data.Repositories;
using VitalChoice.Data.UnitOfWork;
using VitalChoice.Domain.Entities;
using VitalChoice.Domain.Entities.eCommerce.Base;
using VitalChoice.DynamicData.Interfaces.Services;

namespace VitalChoice.DynamicData.Services
{
    public abstract class DynamicObjectRepository<TDynamic, TEntity, TOptionType, TOptionValue>
        where TEntity : DynamicDataEntity<TOptionValue, TOptionType>, new()
        where TOptionType : OptionType, new()
        where TOptionValue : OptionValue<TOptionType>, new()
        where TDynamic : MappedObject, new()
    {
        private readonly IDynamicObjectMapper<TDynamic, TEntity, TOptionType, TOptionValue> _mapper;
        private readonly IReadRepositoryAsync<TOptionType> _optionTypesRepository;

        protected DynamicObjectRepository(
            IDynamicObjectMapper<TDynamic, TEntity, TOptionType, TOptionValue> mapper,
            IReadRepositoryAsync<TOptionType> optionTypesRepository)
        {
            _mapper = mapper;
            _optionTypesRepository = optionTypesRepository;
        }

        public async Task<TEntity> InsertAsync(TDynamic model, IQueryObject<TOptionType> typesQuery, IUnitOfWorkAsync uow)
        {
            //(await ValidateProductAsync(model)).Raise();

            var optionTypes = await _optionTypesRepository.Query(typesQuery).SelectAsync(false);
            var entity = _mapper.ToEntity(model, optionTypes);
            if (entity == null)
                return null;
            entity.OptionTypes = new List<TOptionType>();
            var productRepository = uow.RepositoryAsync<TEntity>();
            await productRepository.InsertGraphAsync(entity);
            await uow.SaveChangesAsync(CancellationToken.None);
            entity.OptionTypes = optionTypes;
            return entity;
        }

        protected virtual async Task PostUpdateInternalAsync(TDynamic model, TEntity entity, ICollection<TOptionType> optionTypes, IUnitOfWorkAsync uow)
        {
            var bigValueRepository = uow.RepositoryAsync<BigStringValue>();
            await
                bigValueRepository.InsertRangeAsync(
                    entity.OptionValues.Where(b => b.BigValue != null).Select(o => o.BigValue).ToList());
        }

        protected virtual async Task PreUpdateInternalAsync(TDynamic model, TEntity entity, ICollection<TOptionType> optionTypes, IUnitOfWorkAsync uow)
        {
            var bigValueRepository = uow.RepositoryAsync<BigStringValue>();
            await SetBigValuesAsync(entity, bigValueRepository, true);
            await
                bigValueRepository.DeleteAllAsync(
                    entity.OptionValues.Where(o => o.BigValue != null).Select(o => o.BigValue).ToList());
        }

        public async Task<TEntity> UpdateAsync(TDynamic model, IQueryObject<TOptionType> typesQuery, IUnitOfWorkAsync uow)
        {
            var mainRepository = uow.RepositoryAsync<TEntity>();
            var valueRepository = uow.RepositoryAsync<TOptionValue>();

            var entity = (await mainRepository.Query(o => o.Id == model.Id && o.StatusCode != RecordStatusCode.Deleted)
                .Include(p => p.OptionValues)
                .SelectAsync()).FirstOrDefault();
            if (entity == null)
                return null;

            entity.OptionTypes =
                await _optionTypesRepository.Query(typesQuery).SelectAsync(false);

            await PreUpdateInternalAsync(model, entity, entity.OptionTypes, uow);

            await valueRepository.DeleteAllAsync(entity.OptionValues);

            _mapper.UpdateEntity(model, entity);
            await PostUpdateInternalAsync(model, entity, entity.OptionTypes, uow);
            await mainRepository.UpdateAsync(entity);
            await uow.SaveChangesAsync(CancellationToken.None);
            return entity;
        }

        private static async Task SetBigValuesAsync(TEntity entity, IRepositoryAsync<BigStringValue> bigStringValueRepository, bool tracked = false)
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

        public async Task<bool> DeleteAllAsync(ICollection<TDynamic> models, IUnitOfWorkAsync uow)
        {
            var mainRepository = uow.RepositoryAsync<TEntity>();
            var idList = models.Select(m => m.Id).ToList();
            if (!idList.Any())
                return false;
            var toDelete = await mainRepository.Query(m => idList.Contains(m.Id)).SelectAsync();
            if (!toDelete.Any())
                return false;
            if (!await DeleteAllAsync(toDelete, mainRepository))
                return false;
            await uow.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteAsync(TDynamic model, IUnitOfWorkAsync uow)
        {
            var mainRepository = uow.RepositoryAsync<TEntity>();
            var toDelete = (await mainRepository.Query(m => m.Id == model.Id).SelectAsync()).FirstOrDefault();
            if (toDelete == null)
                return false;
            if (!await DeleteAsync(toDelete, mainRepository))
                return false;
            await uow.SaveChangesAsync();
            return true;
        }

        private static async Task<bool> DeleteAllAsync(ICollection<TEntity> entities, IRepositoryAsync<TEntity> repository)
        {
            foreach (var entity in entities)
            {
                entity.StatusCode = RecordStatusCode.Deleted;
            }
            return await repository.UpdateRangeAsync(entities);
        }

        private static async Task<bool> DeleteAsync(TEntity entity, IRepositoryAsync<TEntity> repository)
        {
            entity.StatusCode = RecordStatusCode.Deleted;
            return await repository.UpdateAsync(entity);
        }
    }
}