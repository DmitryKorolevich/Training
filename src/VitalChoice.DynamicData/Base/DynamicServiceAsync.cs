using Microsoft.Extensions.Logging;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Internal;
using VitalChoice.Data.Helpers;
using VitalChoice.Data.Repositories;
using VitalChoice.Data.Services;
using VitalChoice.Data.UOW;
using VitalChoice.DynamicData.Extensions;
using VitalChoice.DynamicData.Helpers;
using VitalChoice.DynamicData.Interfaces;
using VitalChoice.DynamicData.Validation;
using VitalChoice.Ecommerce.Domain.Dynamic;
using VitalChoice.Ecommerce.Domain.Entities;
using VitalChoice.Ecommerce.Domain.Entities.Base;
using VitalChoice.Ecommerce.Domain.Entities.History;
using VitalChoice.Ecommerce.Domain.Exceptions;
using VitalChoice.Ecommerce.Domain.Helpers;
using VitalChoice.ObjectMapping.Services;

namespace VitalChoice.DynamicData.Base
{
    public abstract partial class DynamicServiceAsync<TDynamic, TEntity, TOptionType, TOptionValue> :
        DynamicReadServiceAsync<TDynamic, TEntity, TOptionType, TOptionValue>, IExtendedDynamicServiceAsync<TDynamic, TEntity, TOptionType, TOptionValue>
        where TEntity : DynamicDataEntity<TOptionValue, TOptionType>, new()
        where TOptionType : OptionType, new()
        where TOptionValue : OptionValue<TOptionType>, new()
        where TDynamic : MappedObject, new()
    {
        //private readonly DirectMapper<TEntity> _directMapper;
        //protected readonly bool IsAfterChangesOverriden;

        protected DynamicServiceAsync(IDynamicMapper<TDynamic, TEntity, TOptionType, TOptionValue> mapper,
            IRepositoryAsync<TEntity> objectRepository,
            IReadRepositoryAsync<TOptionValue> optionValueRepositoryAsync,
            IReadRepositoryAsync<BigStringValue> bigStringRepository,
            IObjectLogItemExternalService objectLogItemExternalService,
            DynamicExtensionsRewriter queryVisitor,
            //DirectMapper<TEntity> directMapper, 
            ILogger logger, IDynamicEntityOrderingExtension<TEntity> orderingExtension)
            : base(
                mapper, objectRepository, bigStringRepository, optionValueRepositoryAsync,
                objectLogItemExternalService, logger, queryVisitor, orderingExtension)
        {
            //_directMapper = directMapper;
            //var instanceType = GetType();
            //IsAfterChangesOverriden =
            //    instanceType.GetTypeInfo().GetMethod(nameof(AfterEntityChangesAsync), BindingFlags.NonPublic | BindingFlags.DeclaredOnly) !=
            //    null;
        }

        protected abstract IUnitOfWorkAsync CreateUnitOfWork();

        protected virtual Task AfterEntityChangesAsync(TDynamic model, TEntity updated, IUnitOfWorkAsync uow)
        {
            return TaskCache.CompletedTask;
        }

        protected virtual Task BeforeEntityChangesAsync(TDynamic model, TEntity entity, IUnitOfWorkAsync uow)
        {
            return TaskCache.CompletedTask;
        }

        protected virtual void LogItemInfoSetAfter(ObjectHistoryLogItem log, TDynamic model)
        {

        }

        protected virtual bool LogObject => true;

        protected virtual bool LogObjectFullData => false;

        public static ErrorBuilder<TDynamic> CreateError()
        {
            return new ErrorBuilder<TDynamic>(null);
        }

        protected virtual Task<List<MessageInfo>> ValidateAsync(TDynamic dynamic)
        {
            return Task.FromResult(new List<MessageInfo>());
        }

        protected virtual Task<List<MessageInfo>> ValidateDeleteAsync(int id)
        {
            return Task.FromResult(new List<MessageInfo>());
        }

        public async Task<TDynamic> InsertAsync(TDynamic model)
        {
            using (var uow = CreateUnitOfWork())
            {
                return await InsertAsync(uow, model);
            }
        }

        public async Task<TDynamic> UpdateAsync(TDynamic model)
        {
            using (var uow = CreateUnitOfWork())
            {
                return await UpdateAsync(uow, model);
            }
        }

        public async Task<List<TDynamic>> InsertRangeAsync(ICollection<TDynamic> models)
        {
            using (var uow = CreateUnitOfWork())
            {
                return await InsertRangeAsync(uow, models);
            }
        }

        public async Task<List<TDynamic>> UpdateRangeAsync(ICollection<TDynamic> models)
        {
            using (var uow = CreateUnitOfWork())
            {
                return await UpdateRangeAsync(uow, models);
            }
        }

        public async Task<bool> DeleteAllAsync(ICollection<TDynamic> models, bool physically = false)
        {
            using (var uow = CreateUnitOfWork())
            {
                return await DeleteAllAsync(uow, models, physically);
            }
        }

        public async Task<bool> DeleteAsync(TDynamic model, bool physically = false)
        {
            using (var uow = CreateUnitOfWork())
            {
                return await DeleteAsync(uow, model, physically);
            }
        }

        public async Task<bool> DeleteAllAsync(ICollection<int> list, bool physically = false)
        {
            using (var uow = CreateUnitOfWork())
            {
                return await DeleteAllAsync(uow, list, physically);
            }
        }

        public async Task<bool> DeleteAsync(int id, bool physically = false)
        {
            using (var uow = CreateUnitOfWork())
            {
                return await DeleteAsync(uow, id, physically);
            }
        }

        protected async Task LogItemChanges(ICollection<TDynamic> models)
        {
            try
            {
                if (LogObject)
                {
                    foreach (var model in models)
                    {
                        DynamicMapper.SecureObject(model);
                    }
                    await
                        ObjectLogItemExternalService.LogItems(models, LogObjectFullData);
                }
            }
            catch (Exception e)
            {
                Logger.LogError("[Object log error]", e);
            }
        }

        protected virtual async Task<List<TEntity>> InsertRangeAsync(ICollection<TDynamic> models, IUnitOfWorkAsync uow)
        {
            foreach (var model in models)
            {
                DynamicMapper.SecureObject(model);
            }
            (await ValidateCollection(models)).Raise();
            var productRepository = uow.RepositoryAsync<TEntity>();
            var toInsert =
                models.Select(
                    d =>
                        new GenericObjectPair<TDynamic, ICollection<TOptionType>>(d,
                            DynamicMapper.FilterByType(d.IdObjectType))).ToArray();
            var mappedList = await DynamicMapper.ToEntityRangeAsync(toInsert);
            var toInsertList = mappedList.Select(e => e.Entity).ToList();
            await productRepository.InsertGraphRangeAsync(toInsertList);
            await uow.SaveChangesAsync();

            foreach (var entity in toInsertList)
            {
                RestoreIdsFromEntity(entity);
            }

            return toInsertList;
        }

        protected virtual async Task<TEntity> InsertAsync(TDynamic model, IUnitOfWorkAsync uow)
        {
            DynamicMapper.SecureObject(model);
            (await ValidateAsync(model)).Raise();
            var optionTypes = DynamicMapper.FilterByType(model.IdObjectType);
            var entity = await DynamicMapper.ToEntityAsync(model, optionTypes);
            if (entity == null)
                return null;
            var productRepository = uow.RepositoryAsync<TEntity>();
            await productRepository.InsertGraphAsync(entity);
            await uow.SaveChangesAsync(CancellationToken.None);
            RestoreIdsFromEntity(entity);
            return entity;
        }

        protected virtual async Task<List<TEntity>> UpdateRangeAsync(ICollection<TDynamic> models, IUnitOfWorkAsync uow)
        {
            foreach (var model in models)
            {
                DynamicMapper.SecureObject(model);
            }
            (await ValidateCollection(models)).Raise();
            var mainRepository = uow.RepositoryAsync<TEntity>();
            var bigValueRepository = uow.RepositoryAsync<BigStringValue>();

            var ids = models.Select(m => m.Id).ToList();
            IQueryFluent<TEntity> query =
                mainRepository.Query(o => ids.Contains(o.Id) && o.StatusCode != (int) RecordStatusCode.Deleted)
                    .Include(p => p.OptionValues)
                    .ThenInclude(p => p.BigValue);
            query = BuildQuery(query);
            if (query != null)
            {
                var entities = await query.SelectAsync(true);
                if (entities.Count == 0)
                    return new List<TEntity>();
                var items = entities.Join(models, entity => entity.Id, model => model.Id,
                    (entity, model) =>
                        new DynamicEntityPair<TDynamic, TEntity>(model, entity))
                    .ToList();
                foreach (var item in items)
                {
                    item.Entity.OptionTypes = DynamicMapper.FilterByType(item.Dynamic.IdObjectType);
                }
                await UpdateItems(uow, items, bigValueRepository);
                await uow.SaveChangesAsync();
                foreach (var entity in entities)
                {
                    RestoreIdsFromEntity(entity);
                }
                return entities;
            }
            throw new ApiException($"BuildQuery failed UpdateRangeAsync<{GetType()}>");
        }

        protected virtual async Task<TEntity> UpdateAsync(TDynamic model, IUnitOfWorkAsync uow)
        {
            DynamicMapper.SecureObject(model);
            (await ValidateAsync(model)).Raise();
            var mainRepository = uow.RepositoryAsync<TEntity>();
            var bigValueRepository = uow.RepositoryAsync<BigStringValue>();
            IQueryFluent<TEntity> query =
                mainRepository.Query(o => o.Id == model.Id && o.StatusCode != (int) RecordStatusCode.Deleted)
                    .Include(p => p.OptionValues)
                    .ThenInclude(p => p.BigValue);
            query = BuildQuery(query);
            var entity = await query.SelectFirstOrDefaultAsync(true);
            if (entity == null)
                return null;
            entity.OptionTypes = DynamicMapper.FilterByType(model.IdObjectType);
            await
                UpdateItems(uow,
                    new List<DynamicEntityPair<TDynamic, TEntity>>
                    {
                        new DynamicEntityPair<TDynamic, TEntity>(model, entity) /*{ InitialEntity = initialEntity}*/
                    }, bigValueRepository);
            await uow.SaveChangesAsync();
            RestoreIdsFromEntity(entity);
            return entity;
        }

        protected static async Task SyncDbCollections<T, TValue>(IUnitOfWorkAsync uow, IEnumerable<T> updated, bool removePhysically)
            where T : DynamicDataEntity<TValue>
            where TValue : OptionValue
        {
            IRepositoryAsync<TValue> optionValuesRepository = uow.RepositoryAsync<TValue>();
            IRepositoryAsync<T> objectRepository = uow.RepositoryAsync<T>();
            foreach (var obj in updated)
            {
                if (obj == null)
                    continue;

                if (obj.StatusCode == (int)RecordStatusCode.Deleted)
                {
                    if (removePhysically)
                    {
                        await optionValuesRepository.DeleteAllAsync(obj.OptionValues);
                        await objectRepository.DeleteAsync(obj);
                    }
                }
                else
                {
                    if (obj.Id == 0)
                    {
                        await objectRepository.InsertGraphAsync(obj);
                    }
                }
            }
        }

        private static void RestoreIdsFromEntity(object entity)
        {
            var entityType = entity?.GetType();
            if (entityType != null)
            {
                var dynamicEntity = entity as DynamicDataEntity;
                var dynamicObject = dynamicEntity?.MappedObject as MappedObject;

                if (dynamicObject == null)
                    return;

                dynamicObject.Id = dynamicEntity.Id;
                dynamicEntity.MappedObject = null;
                var cache = DynamicTypeCache.GetTypeCacheNoMap(entityType);
                foreach (var property in cache.Properties)
                {
                    var itemType = property.Value.PropertyType.TryGetElementType(typeof(ICollection<>));
                    bool isCollection;
                    if (itemType == null)
                    {
                        isCollection = false;
                        itemType = property.Value.PropertyType;
                    }
                    else
                    {
                        isCollection = true;
                    }
                    if (itemType.GetBaseTypes().Any(t => t == typeof(DynamicDataEntity)))
                    {
                        if (isCollection)
                        {
                            var collection = property.Value.Get(entity) as IEnumerable;
                            if (collection != null)
                            {
                                foreach (DynamicDataEntity item in collection)
                                {
                                    RestoreIdsFromEntity(item);
                                }
                            }
                        }
                        else
                        {
                            var prop = property.Value.Get(entity) as DynamicDataEntity;
                            if (prop != null)
                            {
                                RestoreIdsFromEntity(prop);
                            }
                        }
                    }
                }
            }
        }

        private async Task<TDynamic> InsertAsync(IUnitOfWorkAsync uow, TDynamic model)
        {
            //TODO: lock writing DB until we read result (Low, race condition for logging)
            var entity = await InsertAsync(model, uow);
            int id = entity.Id;
            entity = await SelectEntityFirstAsync(o => o.Id == id);
            await LogItemChanges(new[] { await DynamicMapper.FromEntityAsync(entity) });
            return await DynamicMapper.FromEntityAsync(entity);
        }

        private async Task<TDynamic> UpdateAsync(IUnitOfWorkAsync uow, TDynamic model)
        {
            //TODO: lock writing DB until we read result (Low, race condition for logging)
            var entity = await UpdateAsync(model, uow);
            int id = entity.Id;
            entity = await SelectEntityFirstAsync(o => o.Id == id);
            await LogItemChanges(new[] { await DynamicMapper.FromEntityAsync(entity) });
            return await DynamicMapper.FromEntityAsync(entity);
        }

        private async Task<List<TDynamic>> InsertRangeAsync(IUnitOfWorkAsync uow, ICollection<TDynamic> models)
        {
            //TODO: lock writing DB until we read result (Low, race condition for logging)
            var entityIds = (await InsertRangeAsync(models, uow)).Select(e => e.Id).ToList();
            var entities = await SelectEntitiesAsync(o => entityIds.Contains(o.Id));
            await LogItemChanges(await DynamicMapper.FromEntityRangeAsync(entities));
            return await DynamicMapper.FromEntityRangeAsync(entities);
        }

        private async Task<List<TDynamic>> UpdateRangeAsync(IUnitOfWorkAsync uow, ICollection<TDynamic> models)
        {
            //TODO: lock writing DB until we read result (Low, race condition for logging)
            var entities = await UpdateRangeAsync(models, uow);
            var entityIds = entities.Select(e => e.Id).ToArray();
            entities = await SelectEntitiesAsync(o => entityIds.Contains(o.Id));

            await LogItemChanges(await DynamicMapper.FromEntityRangeAsync(entities));
            return await DynamicMapper.FromEntityRangeAsync(entities);
        }

        protected Task<bool> DeleteAllAsync(IUnitOfWorkAsync uow, ICollection<TDynamic> models, bool physically)
        {
            var idList = models.Select(m => m.Id).ToList();
            return DeleteAllAsync(uow, idList, physically);
        }

        protected Task<bool> DeleteAsync(IUnitOfWorkAsync uow, TDynamic model, bool physically)
        {
            return DeleteAsync(uow, model.Id, physically);
        }

        protected virtual async Task<bool> DeleteAsync(IUnitOfWorkAsync uow, int id, bool physically)
        {
            (await ValidateDeleteAsync(id)).Raise();
            var mainRepository = uow.RepositoryAsync<TEntity>();
            var valuesRepository = uow.RepositoryAsync<TOptionValue>();
            var query = mainRepository.Query(m => m.Id == id);
            if (physically)
            {
                query = query.Include(e => e.OptionValues);
            }
            var toDelete = await query.SelectFirstOrDefaultAsync(true);
            if (toDelete == null)
                return false;
            if (!await DeleteAsync(toDelete, mainRepository, valuesRepository, physically))
                return false;
            await uow.SaveChangesAsync();
            return true;
        }

        protected virtual async Task<bool> DeleteAllAsync(IUnitOfWorkAsync uow, ICollection<int> list, bool physically)
        {
            (await ValidateDeleteCollection(list)).Raise();
            var mainRepository = uow.RepositoryAsync<TEntity>();
            var valuesRepository = uow.RepositoryAsync<TOptionValue>();
            if (list.Count == 0)
                return false;
            var query = mainRepository.Query(m => list.Contains(m.Id));
            if (physically)
            {
                query = query.Include(e => e.OptionValues);
            }
            var toDelete = await query.SelectAsync(true);
            if (toDelete.Count == 0)
                return false;
            if (!await DeleteAllAsync(toDelete, mainRepository, valuesRepository, physically))
                return false;
            await uow.SaveChangesAsync();
            return true;
        }

        private async Task UpdateItems(IUnitOfWorkAsync uow, ICollection<DynamicEntityPair<TDynamic, TEntity>> items,
            IRepositoryAsync<BigStringValue> bigValueRepository)
        {
            foreach (var pair in items)
            {
                await BeforeEntityChangesAsync(pair.Dynamic, pair.Entity, uow);
            }

            var bigValuesBeforeUpdate = items.SelectMany(i => i.Entity.OptionValues).Where(o => o.BigValue != null).Select(o => o.BigValue).ToArray();

            await DynamicMapper.UpdateEntityRangeAsync(items);

            var removedBigValues =
                bigValuesBeforeUpdate.ExceptKeyedWith(
                    items.SelectMany(i => i.Entity.OptionValues).Where(o => o.BigValue != null).Select(o => o.BigValue), b => b.IdBigString);

            await bigValueRepository.DeleteAllAsync(removedBigValues);
            await
                bigValueRepository.InsertRangeAsync(
                    items.SelectMany(i => i.Entity.OptionValues)
                        .Where(b => b.BigValue != null && b.IdBigString == null)
                        .Select(b => b.BigValue));

            foreach (var pair in items)
            {
                await AfterEntityChangesAsync(pair.Dynamic, pair.Entity, uow);
            }
        }

        private async Task<List<MessageInfo>> ValidateCollection(ICollection<TDynamic> dynamicList)
        {
            List<MessageInfo> results = new List<MessageInfo>();
            foreach (var item in dynamicList)
            {
                results.AddRange(await ValidateAsync(item));
            }
            return results;
        }

        private async Task<List<MessageInfo>> ValidateDeleteCollection(ICollection<int> ids)
        {
            List<MessageInfo> results = new List<MessageInfo>();
            foreach (var item in ids)
            {
                results.AddRange(await ValidateDeleteAsync(item));
            }
            return results;
        }

        private static async Task<bool> DeleteAllAsync(ICollection<TEntity> entities,
            IRepositoryAsync<TEntity> mainRepository, IRepositoryAsync<TOptionValue> valueRepository, bool physically)
        {
            if (physically)
            {
                return await mainRepository.DeleteAllAsync(entities) && await valueRepository.DeleteAllAsync(entities.SelectMany(e => e.OptionValues));
            }
            foreach (var entity in entities)
            {
                entity.StatusCode = (int) RecordStatusCode.Deleted;
            }
            return await mainRepository.UpdateRangeAsync(entities);
        }

        private static async Task<bool> DeleteAsync(TEntity entity, IRepositoryAsync<TEntity> repository,
            IRepositoryAsync<TOptionValue> valueRepository, bool physically)
        {
            if (physically)
            {
                return await repository.DeleteAsync(entity) && await valueRepository.DeleteAllAsync(entity.OptionValues);
            }
            entity.StatusCode = (int) RecordStatusCode.Deleted;
            return await repository.UpdateAsync(entity);
        }
    }

    public partial class DynamicServiceAsync<TDynamic, TEntity, TOptionType, TOptionValue>
    {
        public TDynamic Insert(TDynamic model)
        {
            return InsertAsync(model).GetAwaiter().GetResult();
        }

        public TDynamic Update(TDynamic model)
        {
            return UpdateAsync(model).GetAwaiter().GetResult();
        }

        public List<TDynamic> InsertRange(ICollection<TDynamic> models)
        {
            return InsertRangeAsync(models).GetAwaiter().GetResult();
        }

        public List<TDynamic> UpdateRange(ICollection<TDynamic> models)
        {
            return UpdateRangeAsync(models).GetAwaiter().GetResult();
        }

        public bool DeleteAll(ICollection<TDynamic> models, bool physically = false)
        {
            return DeleteAllAsync(models, physically).GetAwaiter().GetResult();
        }

        public bool Delete(TDynamic model, bool physically = false)
        {
            return DeleteAsync(model, physically).GetAwaiter().GetResult();
        }

        public bool DeleteAll(ICollection<int> list, bool physically = false)
        {
            return DeleteAllAsync(list, physically).GetAwaiter().GetResult();
        }

        public bool Delete(int id, bool physically = false)
        {
            return DeleteAsync(id, physically).GetAwaiter().GetResult();
        }
    }
}