using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using VitalChoice.Data.Helpers;
using VitalChoice.Data.Repositories;
using VitalChoice.Data.UnitOfWork;
using VitalChoice.Domain.Entities;
using VitalChoice.Domain.Entities.eCommerce.Base;
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
            IReadRepositoryAsync<BigStringValue> bigStringRepository)
            : base(mapper, objectRepository, optionTypesRepository, bigStringRepository)
        {
        }

        protected abstract IUnitOfWorkAsync CreateUnitOfWork();

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
                return await SelectAsync(entity.Id);
            }
        }

        public async Task<TDynamic> UpdateAsync(TDynamic model)
        {
            using (var uow = CreateUnitOfWork())
            {
                var entity = await UpdateAsync(model, uow);
                return Mapper.FromEntity(entity);
            }
        }

        public async Task<List<TDynamic>> InsertRangeAsync(ICollection<TDynamic> models)
        {
            using (var uow = CreateUnitOfWork())
            {
                var entities = (await InsertRangeAsync(models, uow)).Select(e => e.Id).ToList();
                return await SelectAsync(entities);
            }
        }

        public async Task<List<TDynamic>> UpdateRangeAsync(ICollection<TDynamic> models)
        {
            using (var uow = CreateUnitOfWork())
            {
                var entities = await UpdateRangeAsync(models, uow);
                return Mapper.FromEntityRange(entities);
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

        public async Task<bool> DeleteAllAsync(ICollection<int> list)
        {
            using (var uow = CreateUnitOfWork())
            {
                return await DeleteAllAsync(list, uow);
            }
        }

        public async Task<bool> DeleteAsync(int id)
        {
            using (var uow = CreateUnitOfWork())
            {
                return await DeleteAsync(id, uow);
            }
        }

        public TDynamic Insert(TDynamic model)
        {
            var task = InsertAsync(model);
            task.Wait();
            return task.Result;
        }

        public TDynamic Update(TDynamic model)
        {
            var task = UpdateAsync(model);
            task.Wait();
            return task.Result;
        }

        public List<TDynamic> InsertRange(ICollection<TDynamic> models)
        {
            var task = InsertRangeAsync(models);
            task.Wait();
            return task.Result;
        }

        public List<TDynamic> UpdateRange(ICollection<TDynamic> models)
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

        public bool DeleteAll(ICollection<int> list)
        {
            var task = DeleteAllAsync(list);
            task.Wait();
            return task.Result;
        }

        public bool Delete(int id)
        {
            var task = DeleteAsync(id);
            task.Wait();
            return task.Result;
        }

        protected virtual async Task<List<TEntity>> InsertRangeAsync(ICollection<TDynamic> models, IUnitOfWorkAsync uow)
        {
            (await ValidateCollection(models)).Raise();
            List<Pair<TEntity, List<TOptionType>>> entities = new List<Pair<TEntity, List<TOptionType>>>();
            var productRepository = uow.RepositoryAsync<TEntity>();
            foreach (var model in models)
            {
                var optionTypes = await OptionTypesRepository.Query(GetOptionTypeQuery(model.IdObjectType)).SelectAsync(false);
                var entity = Mapper.ToEntity(model, optionTypes);
                if (entity == null)
                    continue;
                entity.OptionTypes = new List<TOptionType>();
                entities.Add(new Pair<TEntity, List<TOptionType>>(entity, optionTypes));
            }
            var toInsertList = entities.Select(e => e.FirstValue).ToList();
            await productRepository.InsertGraphRangeAsync(toInsertList);
            foreach (var entity in entities)
            {
                entity.FirstValue.OptionTypes = entity.SecondValue;
            }
            await uow.SaveChangesAsync();
            return toInsertList;
        }

        protected virtual async Task<TEntity> InsertAsync(TDynamic model, IUnitOfWorkAsync uow)
        {
            (await Validate(model)).Raise();
            var optionTypes = await OptionTypesRepository.Query(GetOptionTypeQuery(model.IdObjectType)).SelectAsync(false);
            var entity = Mapper.ToEntity(model, optionTypes);
            if (entity == null)
                return null;
            entity.OptionTypes = new List<TOptionType>();
            var productRepository = uow.RepositoryAsync<TEntity>();
            await productRepository.InsertGraphAsync(entity);
            await uow.SaveChangesAsync(CancellationToken.None);
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

        protected virtual async Task<List<TEntity>> UpdateRangeAsync(ICollection<TDynamic> models, IUnitOfWorkAsync uow)
        {
            (await ValidateCollection(models)).Raise();
            var mainRepository = uow.RepositoryAsync<TEntity>();
            var valueRepository = uow.RepositoryAsync<TOptionValue>();
            var bigValueRepository = uow.RepositoryAsync<BigStringValue>();

            var ids = models.Select(m => m.Id).ToList();
            IQueryFluent<TEntity> query =
                mainRepository.Query(o => ids.Contains(o.Id) && o.StatusCode != RecordStatusCode.Deleted)
                    .Include(p => p.OptionValues);
            query = BuildQuery(query);
            var entities = (await query.SelectAsync());
            if (!entities.Any())
                return new List<TEntity>();
            var items = entities.Join(models, entity => entity.Id, model => model.Id,
                (entity, model) => new Pair<TDynamic, TEntity> (model, entity));

            foreach (var item in items)
            {
                item.SecondValue.OptionTypes = await OptionTypesRepository.Query(GetOptionTypeQuery(item.FirstValue.IdObjectType)).SelectAsync(false); ;
                await UpdateItem(uow, item, bigValueRepository, valueRepository);
            }
            await mainRepository.UpdateRangeAsync(entities);
            await uow.SaveChangesAsync(CancellationToken.None);
            return entities;
        }

        protected virtual async Task<TEntity> UpdateAsync(TDynamic model, IUnitOfWorkAsync uow)
        {
            (await Validate(model)).Raise();
            var mainRepository = uow.RepositoryAsync<TEntity>();
            var valueRepository = uow.RepositoryAsync<TOptionValue>();
            var bigValueRepository = uow.RepositoryAsync<BigStringValue>();
            IQueryFluent<TEntity> query =
                mainRepository.Query(o => o.Id == model.Id && o.StatusCode != RecordStatusCode.Deleted)
                    .Include(p => p.OptionValues);
            query = BuildQuery(query);
            var entity = await query.SelectFirstOrDefaultAsync();
            if (entity == null)
                return null;

            entity.OptionTypes = await OptionTypesRepository.Query(GetOptionTypeQuery(model.IdObjectType)).SelectAsync(false);
            await
                UpdateItem(uow, new Pair<TDynamic, TEntity>(model, entity), bigValueRepository, valueRepository);
            await mainRepository.UpdateAsync(entity);
            await uow.SaveChangesAsync(CancellationToken.None);
            return entity;
        }

        private async Task<bool> DeleteAllAsync(ICollection<int> list, IUnitOfWorkAsync uow)
        {
            (await ValidateDeleteCollection(list)).Raise();
            var mainRepository = uow.RepositoryAsync<TEntity>();
            if (!list.Any())
                return false;
            var toDelete = await mainRepository.Query(m => list.Contains(m.Id)).SelectAsync();
            if (!toDelete.Any())
                return false;
            if (!await DeleteAllAsync(toDelete, mainRepository))
                return false;
            await uow.SaveChangesAsync();
            return true;
        }

        private async Task UpdateItem(IUnitOfWorkAsync uow, Pair<TDynamic, TEntity> item,
            IRepositoryAsync<BigStringValue> bigValueRepository,
            IRepositoryAsync<TOptionValue> valueRepository)
        {
            await SetBigValuesAsync(item.SecondValue, bigValueRepository, true);
            await BeforeUpdateAsync(item.FirstValue, item.SecondValue, uow);

            await
                bigValueRepository.DeleteAllAsync(
                    item.SecondValue.OptionValues.Where(o => o.BigValue != null).Select(o => o.BigValue).ToList());

            await valueRepository.DeleteAllAsync(item.SecondValue.OptionValues);

            Mapper.UpdateEntity(item.FirstValue, item.SecondValue);

            await
                bigValueRepository.InsertRangeAsync(
                    item.SecondValue.OptionValues.Where(b => b.BigValue != null).Select(o => o.BigValue).ToList());

            await AfterUpdateAsync(item.FirstValue, item.SecondValue, uow);
        }

        private async Task<bool> DeleteAsync(int id, IUnitOfWorkAsync uow)
        {
            (await ValidateDelete(id)).Raise();
            var mainRepository = uow.RepositoryAsync<TEntity>();
            var toDelete = (await mainRepository.Query(m => m.Id == id).SelectAsync()).FirstOrDefault();
            if (toDelete == null)
                return false;
            if (!await DeleteAsync(toDelete, mainRepository))
                return false;
            await uow.SaveChangesAsync();
            return true;
        }

        private async Task<bool> DeleteAllAsync(ICollection<TDynamic> models, IUnitOfWorkAsync uow)
        {
            var idList = models.Select(m => m.Id).ToList();
            return await DeleteAllAsync(idList, uow);
        }

        private async Task<bool> DeleteAsync(TDynamic model, IUnitOfWorkAsync uow)
        {
            return await DeleteAsync(model.Id, uow);
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