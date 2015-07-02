using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using VitalChoice.Data.Helpers;
using VitalChoice.Data.Repositories;
using VitalChoice.Data.UnitOfWork;
using VitalChoice.Domain.Entities;
using VitalChoice.Domain.Entities.eCommerce.Base;
using VitalChoice.DynamicData.Interfaces;
using VitalChoice.DynamicData.Interfaces.Services;
using VitalChoice.DynamicData.Services;

namespace VitalChoice.DynamicData.Base
{
    public abstract class DynamicObjectRepositoryAsync<TDynamic, TEntity, TOptionType, TOptionValue> : ReadDynamicObjectRepositoryAsync<TDynamic, TEntity, TOptionType, TOptionValue>, IDynamicObjectRepositoryAsync<TDynamic, TEntity>
        where TEntity : DynamicDataEntity<TOptionValue, TOptionType>, new()
        where TOptionType : OptionType, new()
        where TOptionValue : OptionValue<TOptionType>, new()
        where TDynamic : MappedObject, new()
    {
        protected DynamicObjectRepositoryAsync(IDynamicObjectMapper<TDynamic, TEntity, TOptionType, TOptionValue> mapper, IUnitOfWorkAsync readUow) : base(mapper, readUow)
        {
        }

        protected abstract IUnitOfWorkAsync CreateUnitOfWork();

        public async Task<TEntity> InsertAsync(TDynamic model)
        {
            using (var uow = CreateUnitOfWork())
            {
                return await InsertAsync(model, OptionTypeQuery, uow);
            }
        }

        public async Task<TEntity> UpdateAsync(TDynamic model)
        {
            using (var uow = CreateUnitOfWork())
            {
                return await UpdateAsync(model, OptionTypeQuery, uow);
            }
        }

        public async Task<List<TEntity>> InsertRangeAsync(ICollection<TDynamic> models)
        {
            using (var uow = CreateUnitOfWork())
            {
                return await InsertRangeAsync(models, OptionTypeQuery, uow);
            }
        }

        public async Task<List<TEntity>> UpdateRangeAsync(ICollection<TDynamic> models)
        {
            using (var uow = CreateUnitOfWork())
            {
                return await UpdateRangeAsync(models, OptionTypeQuery, uow);
            }
        }

        public async Task<bool> DeleteAllAsync(ICollection<TDynamic> models)
        {
            using (var uow = CreateUnitOfWork())
            {
                return await DeleteAllAsync(models, uow);
            }
        }

        public async Task<bool> DeleteAsync(TDynamic model)
        {
            using (var uow = CreateUnitOfWork())
            {
                return await DeleteAsync(model, uow);
            }
        }

        public TEntity Insert(TDynamic model)
        {
            var task = InsertAsync(model);
            task.Wait();
            return task.Result;
        }

        public TEntity Update(TDynamic model)
        {
            var task = UpdateAsync(model);
            task.Wait();
            return task.Result;
        }

        public List<TEntity> InsertRange(ICollection<TDynamic> models)
        {
            var task = InsertRangeAsync(models);
            task.Wait();
            return task.Result;
        }

        public List<TEntity> UpdateRange(ICollection<TDynamic> models)
        {
            var task = UpdateRangeAsync(models);
            task.Wait();
            return task.Result;
        }

        public bool DeleteAll(ICollection<TDynamic> models)
        {
            var task = DeleteAllAsync(models);
            task.Wait();
            return task.Result;
        }

        public bool Delete(TDynamic model)
        {
            var task = DeleteAsync(model);
            task.Wait();
            return task.Result;
        }

        private async Task<List<TEntity>> InsertRangeAsync(ICollection<TDynamic> models, IQueryObject<TOptionType> typesQuery, IUnitOfWorkAsync uow)
        {
            List<TEntity> entities = new List<TEntity>();
            var optionTypes = await OptionTypesRepository.Query(typesQuery).SelectAsync(false);
            var productRepository = uow.RepositoryAsync<TEntity>();
            foreach (var model in models)
            {
                var entity = Mapper.ToEntity(model, optionTypes);
                if (entity == null)
                    continue;
                entity.OptionTypes = new List<TOptionType>();
                entities.Add(entity);
            }
            await productRepository.InsertGraphRangeAsync(entities);
            foreach (var entity in entities)
            {
                entity.OptionTypes = optionTypes;
            }
            await uow.SaveChangesAsync();
            return entities;
        }

        private async Task<TEntity> InsertAsync(TDynamic model, IQueryObject<TOptionType> typesQuery, IUnitOfWorkAsync uow)
        {
            var optionTypes = await OptionTypesRepository.Query(typesQuery).SelectAsync(false);
            var entity = Mapper.ToEntity(model, optionTypes);
            if (entity == null)
                return null;
            entity.OptionTypes = new List<TOptionType>();
            var productRepository = uow.RepositoryAsync<TEntity>();
            await productRepository.InsertGraphAsync(entity);
            await uow.SaveChangesAsync(CancellationToken.None);
            entity.OptionTypes = optionTypes;
            return entity;
        }

        protected virtual Task AfterUpdateAsync(TDynamic model, TEntity entity, IUnitOfWorkAsync uow)
        {
            return Task.Delay(0);
        }

        protected virtual Task BeforeUpdateAsync(TDynamic model, TEntity entity, IUnitOfWorkAsync uow)
        {
            return Task.Delay(0);
        }

        private async Task<List<TEntity>> UpdateRangeAsync(ICollection<TDynamic> models, IQueryObject<TOptionType> typesQuery, IUnitOfWorkAsync uow)
        {
            var mainRepository = uow.RepositoryAsync<TEntity>();
            var valueRepository = uow.RepositoryAsync<TOptionValue>();
            var bigValueRepository = uow.RepositoryAsync<BigStringValue>();

            var ids = models.Select(m => m.Id).ToList();
            var entities = (await mainRepository.Query(o => ids.Contains(o.Id) && o.StatusCode != RecordStatusCode.Deleted)
                .Include(p => p.OptionValues)
                .SelectAsync());
            if (!entities.Any())
                return new List<TEntity>();
            var items = entities.Join(models, entity => entity.Id, model => model.Id,
                (entity, model) => new KeyValuePair<TDynamic, TEntity> (model, entity));

            var optionTypes = await OptionTypesRepository.Query(typesQuery).SelectAsync(false);

            foreach (var item in items)
            {
                item.Value.OptionTypes = optionTypes;
                await UpdateItem(uow, item, bigValueRepository, valueRepository);
            }
            await mainRepository.UpdateRangeAsync(entities);
            await uow.SaveChangesAsync(CancellationToken.None);
            return entities;
        }

        private async Task UpdateItem(IUnitOfWorkAsync uow, KeyValuePair<TDynamic, TEntity> item,
            IRepositoryAsync<BigStringValue> bigValueRepository,
            IRepositoryAsync<TOptionValue> valueRepository)
        {
            await SetBigValuesAsync(item.Value, bigValueRepository, true);
            await BeforeUpdateAsync(item.Key, item.Value, uow);

            await
                bigValueRepository.DeleteAllAsync(
                    item.Value.OptionValues.Where(o => o.BigValue != null).Select(o => o.BigValue).ToList());

            await valueRepository.DeleteAllAsync(item.Value.OptionValues);

            Mapper.UpdateEntity(item.Key, item.Value);

            await
                bigValueRepository.InsertRangeAsync(
                    item.Value.OptionValues.Where(b => b.BigValue != null).Select(o => o.BigValue).ToList());

            await AfterUpdateAsync(item.Key, item.Value, uow);
        }

        private async Task<TEntity> UpdateAsync(TDynamic model, IQueryObject<TOptionType> typesQuery, IUnitOfWorkAsync uow)
        {
            var mainRepository = uow.RepositoryAsync<TEntity>();
            var valueRepository = uow.RepositoryAsync<TOptionValue>();
            var bigValueRepository = uow.RepositoryAsync<BigStringValue>();

            var entity = (await mainRepository.Query(o => o.Id == model.Id && o.StatusCode != RecordStatusCode.Deleted)
                .Include(p => p.OptionValues)
                .SelectAsync()).FirstOrDefault();
            if (entity == null)
                return null;

            entity.OptionTypes = await OptionTypesRepository.Query(typesQuery).SelectAsync(false);
            await
                UpdateItem(uow, new KeyValuePair<TDynamic, TEntity>(model, entity), bigValueRepository, valueRepository);
            await mainRepository.UpdateAsync(entity);
            await uow.SaveChangesAsync(CancellationToken.None);
            return entity;
        }

        private async Task<bool> DeleteAllAsync(ICollection<TDynamic> models, IUnitOfWorkAsync uow)
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

        private async Task<bool> DeleteAsync(TDynamic model, IUnitOfWorkAsync uow)
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