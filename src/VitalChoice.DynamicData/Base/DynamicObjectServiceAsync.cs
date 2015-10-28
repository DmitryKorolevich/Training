using Microsoft.Framework.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using VitalChoice.Data.Helpers;
using VitalChoice.Data.Repositories;
using VitalChoice.Data.Services;
using VitalChoice.Data.UnitOfWork;
using VitalChoice.Domain.Entities;
using VitalChoice.Domain.Entities.eCommerce.Base;
using VitalChoice.Domain.Entities.eCommerce.History;
using VitalChoice.Domain.Exceptions;
using VitalChoice.DynamicData.Interfaces;
using VitalChoice.DynamicData.Validation;

namespace VitalChoice.DynamicData.Base
{
    public abstract class DynamicObjectServiceAsync<TDynamic, TEntity, TOptionType, TOptionValue> : ReadDynamicObjectServiceAsync<TDynamic, TEntity, TOptionType, TOptionValue>, IDynamicObjectServiceAsync<TDynamic, TEntity>
        where TEntity : DynamicDataEntity<TOptionValue, TOptionType>, new()
        where TOptionType : OptionType, new()
        where TOptionValue : OptionValue<TOptionType>, new()
        where TDynamic : MappedObject, new()
    {
        protected DynamicObjectServiceAsync(IDynamicObjectMapper<TDynamic, TEntity, TOptionType, TOptionValue> mapper,
            IReadRepositoryAsync<TEntity> objectRepository, IReadRepositoryAsync<TOptionType> optionTypesRepository,
            IReadRepositoryAsync<TOptionValue> optionValueRepositoryAsync,
            IReadRepositoryAsync<BigStringValue> bigStringRepository,
            IObjectLogItemExternalService objectLogItemExternalService,
            ILogger logger)
            : base(mapper, objectRepository, optionTypesRepository, bigStringRepository, optionValueRepositoryAsync, objectLogItemExternalService, logger)
        {
        }

        protected abstract IUnitOfWorkAsync CreateUnitOfWork();

        protected virtual Task AfterEntityChangesAsync(TDynamic model, TEntity entity, IUnitOfWorkAsync uow)
        {
            return Task.Delay(0);
        }

        protected virtual Task BeforeEntityChangesAsync(TDynamic model, TEntity entity, IUnitOfWorkAsync uow)
        {
            return Task.Delay(0);
        }

        public static ErrorBuilder<TDynamic> CreateError()
        {
            return new ErrorBuilder<TDynamic>(null);
        }

        protected virtual Task<List<MessageInfo>> Validate(TDynamic dynamic)
        {
            return Task.FromResult(new List<MessageInfo>());
        }

        protected virtual Task<List<MessageInfo>> ValidateDelete(int id)
        {
            return Task.FromResult(new List<MessageInfo>());
        }

        private async Task<List<MessageInfo>> ValidateCollection(ICollection<TDynamic> dynamicList)
        {
            List<MessageInfo> results = new List<MessageInfo>();
            foreach (var item in dynamicList)
            {
                results.AddRange(await Validate(item));
            }
            return results;
        }

        private async Task<List<MessageInfo>> ValidateDeleteCollection(ICollection<int> ids)
        {
            List<MessageInfo> results = new List<MessageInfo>();
            foreach (var item in ids)
            {
                results.AddRange(await ValidateDelete(item));
            }
            return results;
        }

        public async Task<TDynamic> InsertAsync(TDynamic model)
        {
            using (var uow = CreateUnitOfWork())
            {
                var entity = await InsertAsync(model, uow);
                entity = await SelectEntityAsync(entity.Id);
                await LogItemChanges(new[] {await Mapper.FromEntityAsync(entity)});
                return await Mapper.FromEntityAsync(entity);
            }
        }

        public async Task<TDynamic> UpdateAsync(TDynamic model)
        {
            using (var uow = CreateUnitOfWork())
            {
                var entity = await UpdateAsync(model, uow);
                entity = await SelectEntityAsync(entity.Id);

                await LogItemChanges(new [] {await Mapper.FromEntityAsync(entity)});
                return await Mapper.FromEntityAsync(entity);
            }
        }

        public async Task<List<TDynamic>> InsertRangeAsync(ICollection<TDynamic> models)
        {
            using (var uow = CreateUnitOfWork())
            {
                var entityIds = (await InsertRangeAsync(models, uow)).Select(e => e.Id).ToList();
                var entities = await SelectEntityListAsync(entityIds);
                await LogItemChanges(await Mapper.FromEntityRangeAsync(entities));
                return await Mapper.FromEntityRangeAsync(entities);
            }
        }

        public async Task<List<TDynamic>> UpdateRangeAsync(ICollection<TDynamic> models)
        {
            using (var uow = CreateUnitOfWork())
            {
                var entities = await UpdateRangeAsync(models, uow);
                entities = await SelectEntityListAsync(entities.Select(e => e.Id).ToList());

                await LogItemChanges(await Mapper.FromEntityRangeAsync(entities));
                return await Mapper.FromEntityRangeAsync(entities);
            }
        }

        public async Task<bool> DeleteAllAsync(ICollection<TDynamic> models, bool physically = false)
        {
            using (var uow = CreateUnitOfWork())
            {
                return await DeleteAllAsync(models, uow, physically);
            }
        }

        public async Task<bool> DeleteAsync(TDynamic model, bool physically = false)
        {
            using (var uow = CreateUnitOfWork())
            {
                return await DeleteAsync(model, uow, physically);
            }
        }

        public async Task<bool> DeleteAllAsync(ICollection<int> list, bool physically = false)
        {
            using (var uow = CreateUnitOfWork())
            {
                return await DeleteAllAsync(list, uow, physically);
            }
        }

        public async Task<bool> DeleteAsync(int id, bool physically = false)
        {
            using (var uow = CreateUnitOfWork())
            {
                return await DeleteAsync(id, uow, physically);
            }
        }

        public TDynamic Insert(TDynamic model)
        {
            var task = InsertAsync(model);
            return task.Result;
        }

        public TDynamic Update(TDynamic model)
        {
            var task = UpdateAsync(model);
            return task.Result;
        }

        public List<TDynamic> InsertRange(ICollection<TDynamic> models)
        {
            var task = InsertRangeAsync(models);
            return task.Result;
        }

        public List<TDynamic> UpdateRange(ICollection<TDynamic> models)
        {
            var task = UpdateRangeAsync(models);
            return task.Result;
        }

        public bool DeleteAll(ICollection<TDynamic> models, bool physically = false)
        {
            var task = DeleteAllAsync(models, physically);
            return task.Result;
        }

        public bool Delete(TDynamic model, bool physically = false)
        {
            var task = DeleteAsync(model, physically);
            return task.Result;
        }

        public bool DeleteAll(ICollection<int> list, bool physically = false)
        {
            var task = DeleteAllAsync(list, physically);
            return task.Result;
        }

        public bool Delete(int id, bool physically = false)
        {
            var task = DeleteAsync(id, physically);
            return task.Result;
        }

        protected virtual void LogItemInfoSetAfter(ObjectHistoryLogItem log, TDynamic model)
        {

        }

        protected virtual bool LogObject => true;

        protected virtual bool LogObjectFullData => false;

        private async Task LogItemChanges(ICollection<TDynamic> models)
        {
            try
            {
                if (LogObject)
                {
                    foreach (var model in models)
                    {
                        Mapper.RemoveSecurityInformation(model);
                    }
                    await ObjectLogItemExternalService.LogItems(models.Select(p=>(object)p).ToList(), LogObjectFullData);
                }
            }
            catch (Exception e)
            {
                Logger.LogError("[Object log error]",e);
            }
        }

        protected virtual async Task<List<TEntity>> InsertRangeAsync(ICollection<TDynamic> models, IUnitOfWorkAsync uow)
        {
            (await ValidateCollection(models)).Raise();
            List<GenericPair<TEntity, List<TOptionType>>> entities = new List<GenericPair<TEntity, List<TOptionType>>>();
            var productRepository = uow.RepositoryAsync<TEntity>();
            var optionTypes = await OptionTypesRepository.Query().SelectAsync(false);
            var toInsert =
                models.Select(
                    d =>
                        new GenericPair<TDynamic, ICollection<TOptionType>>(d,
                            Mapper.FilterByType(optionTypes, d.IdObjectType).ToList())).ToList();
            var mappedList = await Mapper.ToEntityRangeAsync(toInsert);
            foreach (var entity in mappedList.Select(p=>p.Entity).ToList())
            {
                if (entity == null)
                    continue;
                entity.OptionTypes = new List<TOptionType>();
                entities.Add(new GenericPair<TEntity, List<TOptionType>>(entity, optionTypes));
            }
            var toInsertList = entities.Select(e => e.Value1).ToList();
            await productRepository.InsertGraphRangeAsync(toInsertList);
            foreach (var entity in entities)
            {
                entity.Value1.OptionTypes = entity.Value2;
            }
            await uow.SaveChangesAsync();

            foreach (var mappedListItem in mappedList)
            {
                mappedListItem.Dynamic.Id = mappedListItem.Entity.Id;
            }

            return toInsertList;
        }

        protected virtual async Task<TEntity> InsertAsync(TDynamic model, IUnitOfWorkAsync uow)
        {
            (await Validate(model)).Raise();
            var optionTypes = await OptionTypesRepository.Query(GetOptionTypeQuery(model.IdObjectType)).SelectAsync(false);
            var entity = await Mapper.ToEntityAsync(model, optionTypes);
            if (entity == null)
                return null;
            entity.OptionTypes = new List<TOptionType>();
            var productRepository = uow.RepositoryAsync<TEntity>();
            await productRepository.InsertGraphAsync(entity);
            await uow.SaveChangesAsync(CancellationToken.None);

            model.Id = entity.Id;

            return entity;
        }

        protected virtual async Task<List<TEntity>> UpdateRangeAsync(ICollection<TDynamic> models, IUnitOfWorkAsync uow)
        {
            (await ValidateCollection(models)).Raise();
            var mainRepository = uow.RepositoryAsync<TEntity>();
            var valueRepository = uow.RepositoryAsync<TOptionValue>();
            var bigValueRepository = uow.RepositoryAsync<BigStringValue>();

            var ids = models.Select(m => m.Id).ToList();
            IQueryFluent<TEntity> query =
                mainRepository.Query(o => ids.Contains(o.Id) && o.StatusCode != (int)RecordStatusCode.Deleted)
                    .Include(p => p.OptionValues);
            query = BuildQuery(query);
            var entities = (await query.SelectAsync());
            if (!entities.Any())
                return new List<TEntity>();
            var items = entities.Join(models, entity => entity.Id, model => model.Id,
                (entity, model) => new DynamicEntityPair<TDynamic, TEntity>(model, entity)).ToList();
            var optionTypes = await OptionTypesRepository.Query().SelectAsync(false);
            foreach (var item in items)
            {
                item.Entity.OptionTypes = Mapper.FilterByType(optionTypes, item.Dynamic.IdObjectType).ToList();
            }
            await UpdateItems(uow, items, bigValueRepository, valueRepository);
            await mainRepository.UpdateRangeAsync(entities);
            await uow.SaveChangesAsync(CancellationToken.None);
            await AfterSelect(entities);

            return entities;
        }

        protected virtual async Task<TEntity> UpdateAsync(TDynamic model, IUnitOfWorkAsync uow)
        {
            (await Validate(model)).Raise();
            var mainRepository = uow.RepositoryAsync<TEntity>();
            var valueRepository = uow.RepositoryAsync<TOptionValue>();
            var bigValueRepository = uow.RepositoryAsync<BigStringValue>();
            IQueryFluent<TEntity> query =
                mainRepository.Query(o => o.Id == model.Id && o.StatusCode != (int)RecordStatusCode.Deleted)
                    .Include(p => p.OptionValues);
            query = BuildQuery(query);
            var entity = await query.SelectFirstOrDefaultAsync();
            if (entity == null)
                return null;

            entity.OptionTypes =
                await OptionTypesRepository.Query(GetOptionTypeQuery(model.IdObjectType)).SelectAsync(false);
            await
                UpdateItems(uow,
                    new List<DynamicEntityPair<TDynamic, TEntity>>
                    {
                        new DynamicEntityPair<TDynamic, TEntity>(model, entity)
                    }, bigValueRepository, valueRepository);
            await mainRepository.UpdateAsync(entity);
            await uow.SaveChangesAsync(CancellationToken.None);
            await AfterSelect(new List<TEntity> {entity});

            return entity;
        }

        private async Task<bool> DeleteAllAsync(ICollection<int> list, IUnitOfWorkAsync uow, bool physically)
        {
            (await ValidateDeleteCollection(list)).Raise();
            var mainRepository = uow.RepositoryAsync<TEntity>();
            var valuesRepository = uow.RepositoryAsync<TOptionValue>();
            if (!list.Any())
                return false;
            var query = mainRepository.Query(m => list.Contains(m.Id));
            if (physically)
            {
                query = query.Include(e => e.OptionValues);
            }
            var toDelete = await query.SelectAsync();
            if (!toDelete.Any())
                return false;
            if (!await DeleteAllAsync(toDelete, mainRepository, valuesRepository, physically))
                return false;
            await uow.SaveChangesAsync();
            return true;
        }

        private async Task UpdateItems(IUnitOfWorkAsync uow, ICollection<DynamicEntityPair<TDynamic, TEntity>> items,
            IRepositoryAsync<BigStringValue> bigValueRepository,
            IRepositoryAsync<TOptionValue> valueRepository)
        {
            await SetBigValuesAsync(items.Select(i => i.Entity), true);
            foreach (var pair in items)
            {
                await BeforeEntityChangesAsync(pair.Dynamic, pair.Entity, uow);
            }
            

            await
                bigValueRepository.DeleteAllAsync(
                    items.SelectMany(i => i.Entity.OptionValues).Where(o => o.BigValue != null).Select(o => o.BigValue));

            await valueRepository.DeleteAllAsync(items.SelectMany(i => i.Entity.OptionValues));

            await Mapper.UpdateEntityRangeAsync(items);

            await
                bigValueRepository.InsertRangeAsync(
                    items.SelectMany(i => i.Entity.OptionValues).Where(b => b.BigValue != null).Select(o => o.BigValue));

            foreach (var pair in items)
            {
                await AfterEntityChangesAsync(pair.Dynamic, pair.Entity, uow);
            }
        }

        protected virtual async Task<bool> DeleteAsync(int id, IUnitOfWorkAsync uow, bool physically)
        {
            (await ValidateDelete(id)).Raise();
            var mainRepository = uow.RepositoryAsync<TEntity>();
            var valuesRepository = uow.RepositoryAsync<TOptionValue>();
            var query = mainRepository.Query(m => m.Id == id);
            if (physically)
            {
                query = query.Include(e => e.OptionValues);
            }
            var toDelete = await query.SelectFirstOrDefaultAsync();
            if (toDelete == null)
                return false;
            if (!await DeleteAsync(toDelete, mainRepository, valuesRepository, physically))
                return false;
            await uow.SaveChangesAsync();
            return true;
        }

        private async Task<bool> DeleteAllAsync(ICollection<TDynamic> models, IUnitOfWorkAsync uow, bool physically)
        {
            var idList = models.Select(m => m.Id).ToList();
            return await DeleteAllAsync(idList, uow, physically);
        }

        private async Task<bool> DeleteAsync(TDynamic model, IUnitOfWorkAsync uow, bool physically)
        {
            return await DeleteAsync(model.Id, uow, physically);
        }

        private static async Task<bool> DeleteAllAsync(ICollection<TEntity> entities, IRepositoryAsync<TEntity> mainRepository, IRepositoryAsync<TOptionValue> valueRepository, bool physically)
        {
            if (physically)
            {
                await valueRepository.DeleteAllAsync(entities.SelectMany(e => e.OptionValues));
                return await mainRepository.DeleteAllAsync(entities);
            }
            foreach (var entity in entities)
            {
                entity.StatusCode = (int)RecordStatusCode.Deleted;
            }
            return await mainRepository.UpdateRangeAsync(entities);
        }

        private static async Task<bool> DeleteAsync(TEntity entity, IRepositoryAsync<TEntity> repository, IRepositoryAsync<TOptionValue> valueRepository, bool physically)
        {
            if (physically)
            {
                await valueRepository.DeleteAllAsync(entity.OptionValues);
                return await repository.DeleteAsync(entity);
            }
            entity.StatusCode = (int)RecordStatusCode.Deleted;
            return await repository.UpdateAsync(entity);
        }
    }
}