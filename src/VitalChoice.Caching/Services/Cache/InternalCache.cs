using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Logging;
using VitalChoice.Caching.Extensions;
using VitalChoice.Caching.Interfaces;
using VitalChoice.Caching.Relational;
using VitalChoice.Caching.Services.Cache.Base;
using VitalChoice.Data.Extensions;
using VitalChoice.Ecommerce.Domain.Helpers;

namespace VitalChoice.Caching.Services.Cache
{
    public class InternalCache<T> : IInternalCache<T>
    {
        public ICacheData GetCacheData(RelationInfo relationInfo)
        {
            return CacheStorage.GetCacheData(relationInfo);
        }

        public EntityInfo EntityInfo { get; }
        protected readonly IInternalEntityCacheFactory CacheFactory;

        protected readonly CacheStorage<T> CacheStorage;

        public InternalCache(EntityInfo entityInfo, IInternalEntityCacheFactory cacheFactory, ILoggerFactory loggerFactory)
        {
            EntityInfo = entityInfo;
            CacheFactory = cacheFactory;
            CacheStorage = new CacheStorage<T>(entityInfo, cacheFactory, loggerFactory);
        }

        public CacheResult<T> TryGetEntity(EntityKey key, RelationInfo relations, ICacheStateManager stateManager, bool tracked)
        {
            var data = CacheStorage.GetCacheData(relations);
            var cached = data.Get(key);
            if (cached != null)
            {
                return new CacheResult<T>(cached, stateManager, tracked);
            }
            return CacheGetResult.Update;
        }

        public IEnumerable<CacheResult<T>> TryGetEntities(ICollection<EntityKey> primaryKeys, RelationInfo relations, ICacheStateManager stateManager, bool tracked)
        {
            var data = CacheStorage.GetCacheData(relations);
            foreach (var key in primaryKeys)
            {
                var cached = data.Get(key);
                if (cached != null)
                {
                    if (cached.NeedUpdate)
                    {
                        yield return CacheGetResult.Update;
                        yield break;
                    }
                }
                else
                {
                    yield return CacheGetResult.Update;
                    yield break;
                }
                yield return new CacheResult<T>(cached, stateManager, tracked);
            }
        }

        public CacheResult<T> TryGetEntity(EntityIndex key, RelationInfo relationInfo, ICacheStateManager stateManager, bool tracked)
        {
            var data = CacheStorage.GetCacheData(relationInfo);
            var cached = data.Get(key);
            if (cached != null)
            {
                return new CacheResult<T>(cached, stateManager, tracked);
            }
            return CacheGetResult.Update;
        }

        public IEnumerable<CacheResult<T>> TryGetEntities(ICollection<EntityIndex> indexes, RelationInfo relations, ICacheStateManager stateManager, bool tracked)
        {
            var data = CacheStorage.GetCacheData(relations);
            foreach (var index in indexes)
            {
                var cached = data.Get(index);
                if (cached != null)
                {
                    using (cached.Lock())
                    {
                        if (cached.NeedUpdate)
                        {
                            yield return CacheGetResult.Update;
                            yield break;
                        }
                        yield return new CacheResult<T>(cached, stateManager, tracked);
                    }
                }
                else
                {
                    yield return CacheGetResult.Update;
                    yield break;
                }
            }
        }

        public CacheResult<T> TryGetEntity(EntityIndex key, EntityConditionalIndexInfo conditionalInfo, RelationInfo relations, ICacheStateManager stateManager, bool tracked)
        {
            var data = CacheStorage.GetCacheData(relations);
            var cached = data.Get(conditionalInfo, key);
            if (cached != null)
            {
                return new CacheResult<T>(cached, stateManager, tracked);
            }
            return CacheGetResult.Update;
        }

        public IEnumerable<CacheResult<T>> TryGetEntities(ICollection<EntityIndex> indexes, EntityConditionalIndexInfo conditionalInfo,
            RelationInfo relations, ICacheStateManager stateManager, bool tracked)
        {
            var data = CacheStorage.GetCacheData(relations);
            foreach (var index in indexes)
            {
                var cached = data.Get(conditionalInfo, index);
                if (cached != null)
                {
                    using (cached.Lock())
                    {
                        if (cached.NeedUpdate)
                        {
                            yield return CacheGetResult.Update;
                            yield break;
                        }
                        yield return new CacheResult<T>(cached, stateManager, tracked);
                    }
                }
                else
                {
                    yield return CacheGetResult.Update;
                    yield break;
                }
            }
        }

        public IEnumerable<CacheResult<T>> GetWhere(RelationInfo relations, Func<T, bool> whereFunc, ICacheStateManager stateManager, bool tracked)
        {
            var data = CacheStorage.GetCacheData(relations);
            if (!data.FullCollection)
            {
                yield return CacheGetResult.NotFound;
                yield break;
            }

            var allItems = data.GetAll();
            if (data.Empty || allItems.Any(cached => cached.NeedUpdate))
            {
                yield return CacheGetResult.Update;
                yield break;
            }
            foreach (var cached in allItems.Where(cached => whereFunc(cached)))
            {
                yield return new CacheResult<T>(cached, stateManager, tracked);
            }
        }

        public bool IsFullCollection(RelationInfo relations)
        {
            return CacheStorage.GetCacheData(relations).FullCollection;
        }

        public IEnumerable<CacheResult<T>> GetAll(RelationInfo relations, ICacheStateManager stateManager, bool tracked)
        {
            var data = CacheStorage.GetCacheData(relations);
            if (!data.FullCollection)
            {
                yield return CacheGetResult.NotFound;
                yield break;
            }
            var allItems = data.GetAll();
            if (data.NeedUpdate || data.Empty || allItems.Any(cached => cached.NeedUpdate))
            {
                yield return CacheGetResult.Update;
                yield break;
            }
            foreach (var cached in allItems)
            {
                yield return new CacheResult<T>(cached, stateManager, tracked);
            }
        }

        public IEnumerable<CacheResult<T>> TryRemoveWithResult(T entity, ICacheStateManager stateManager, bool tracked)
        {
            var pk = EntityInfo.PrimaryKey.GetPrimaryKeyValue(entity);
            var datas = CacheStorage.AllCacheDatas;
            foreach (var data in datas)
            {
                var removed = data.TryRemove(pk);
                if (removed == null)
                {
                    yield return CacheGetResult.NotFound;
                }
                yield return new CacheResult<T>(removed, stateManager, tracked);
            }
        }

        public bool TryRemove(T entity)
        {
            var pk = EntityInfo.PrimaryKey.GetPrimaryKeyValue(entity);
            return TryRemove(pk);
        }

        public bool Update(IEnumerable<T> entities, RelationInfo relationInfo, object dbContext)
        {
            var data = CacheStorage.GetCacheData(relationInfo);
            return data.Update(entities, dbContext);
        }

        public bool Update(T entity, RelationInfo relationInfo, object dbContext)
        {
            if (entity == null)
                return false;

            var data = CacheStorage.GetCacheData(relationInfo);
            return data.Update(entity, dbContext) != null;
        }

        public bool Update(T entity, DbContext context, object dbContext)
        {
            if (entity == null)
                return false;

            var stateManager = (ICacheStateManager) context?.StateManager;

            var result = CacheStorage.AllCacheDatas.Count > 0;
            // ReSharper disable once LoopCanBeConvertedToQuery
            foreach (var data in CacheStorage.AllCacheDatas)
            {
                result = result && data.UpdateKeepRelations(entity, stateManager, context) != null;
            }
            return result;
        }

        public CachedEntity<T> Update(RelationInfo relations, T entity, object dbContext)
        {
            var data = CacheStorage.GetCacheData(relations);
            return data.Update(entity, dbContext);
        }

        public IEnumerable<CachedEntity<T>> Update(RelationInfo relations, IEnumerable<T> entities, object dbContext)
        {
            var data = CacheStorage.GetCacheData(relations);
            return entities.Select(entity => data.Update(entity, dbContext));
        }

        public bool UpdateAll(IEnumerable<T> entities, RelationInfo relationInfo, object dbContext)
        {
            var data = CacheStorage.GetCacheData(relationInfo);
            return data.UpdateAll(entities, dbContext);
        }

        public EntityKey MarkForUpdate(T entity, object dbContext)
        {
            var pk = EntityInfo.PrimaryKey.GetPrimaryKeyValue(entity);
            MarkForUpdateByPrimaryKey(pk, dbContext, null);
            return pk;
        }

        public ICollection<EntityKey> MarkForUpdate(IEnumerable<T> entities, object dbContext)
        {
            var pks = new HashSet<EntityKey>(entities.Select(e => EntityInfo.PrimaryKey.GetPrimaryKeyValue(e)));
            MarkForUpdateByPrimaryKey(pks, dbContext, null);
            return pks;
        }

        public EntityKey MarkForAdd(T entity, object dbContext)
        {

            foreach (var data in CacheStorage.AllCacheDatas)
            {
                lock (data)
                {
                    if (data.FullCollection)
                    {
                        data.NeedUpdate = true;
                    }
                }
            }
            Update(entity, (DbContext) null, dbContext);
            var pk = EntityInfo.PrimaryKey.GetPrimaryKeyValue(entity);
            var foreignKeys = EntityInfo.ForeignKeys.GetForeignKeyValues(entity);
            MarkForUpdateForeignKeys(foreignKeys, dbContext);
            MarkForUpdateDependent(pk, dbContext);
            return pk;
        }

        public ICollection<EntityKey> MarkForAdd(ICollection<T> entities, object dbContext)
        {
            return MarkForAddInternal(entities, dbContext);
        }

        public CachedEntity Update(RelationInfo relations, object entity, object dbContext)
        {
            return Update(relations, (T) entity, dbContext);
        }

        public IEnumerable<CachedEntity> Update(RelationInfo relations, IEnumerable<object> entity, object dbContext)
        {
            return Update(relations, entity.Cast<T>(), dbContext);
        }

        public bool Update(IEnumerable<object> entities, RelationInfo relationInfo, object dbContext)
        {
            return Update(entities.Cast<T>(), relationInfo, dbContext);
        }

        public bool Update(object entity, RelationInfo relationInfo, object dbContext)
        {
            return Update((T) entity, relationInfo, dbContext);
        }

        public bool Update(object entity, DbContext context, object dbContext)
        {
            return Update((T) entity, context, dbContext);
        }

        public void SetNull(IEnumerable<EntityKey> keys, RelationInfo relationInfo)
        {
            var data = CacheStorage.GetCacheData(relationInfo);
            data.SetNull(keys);
        }

        public void SetNull(EntityKey key, RelationInfo relationInfo)
        {
            var data = CacheStorage.GetCacheData(relationInfo);
            data.SetNull(key);
        }

        public bool UpdateAll(IEnumerable<object> entities, RelationInfo relationInfo, object dbContext)
        {
            return UpdateAll(entities.Cast<T>(), relationInfo, dbContext);
        }

        public EntityKey MarkForUpdate(object entity, object dbContext)
        {
            return MarkForUpdate((T) entity, dbContext);
        }

        public ICollection<EntityKey> MarkForUpdate(IEnumerable<object> entities, object dbContext)
        {
            var pks = entities.Select(e => EntityInfo.PrimaryKey.GetPrimaryKeyValue(e)).ToList();
            MarkForUpdateByPrimaryKey(pks, dbContext, null);
            return pks;
        }

        public void MarkForUpdateByPrimaryKey(EntityKey pk, object dbContext, string hasRelation)
        {
            if (pk == null)
                return;
            IEnumerable<ICacheData<T>> cacheDatas = CacheStorage.AllCacheDatas;
            if (!string.IsNullOrWhiteSpace(hasRelation))
            {
                cacheDatas = cacheDatas.Where(c => c.GetHasRelation(hasRelation));
            }
            MarkForUpdateInternal(pk, cacheDatas, hasRelation, dbContext);
        }

        public void MarkForUpdateByPrimaryKey(ICollection<EntityKey> pks, object dbContext, string hasRelation)
        {
            if (pks == null)
                return;
            IEnumerable<ICacheData<T>> cacheDatas = CacheStorage.AllCacheDatas;
            if (!string.IsNullOrWhiteSpace(hasRelation))
            {
                cacheDatas = cacheDatas.Where(c => c.GetHasRelation(hasRelation)).ToArray();
            }
            MarkForUpdateInternal(pks, cacheDatas, hasRelation, dbContext);
        }

        public EntityKey MarkForAdd(object entity, object dbContext)
        {
            return MarkForAdd((T) entity, dbContext);
        }

        public ICollection<EntityKey> MarkForAdd(ICollection<object> entities, object dbContext)
        {
            return MarkForAddInternal(entities, dbContext);
        }

        public bool TryRemove(object entity)
        {
            return TryRemove((T) entity);
        }

        public bool TryRemove(EntityKey pk)
        {
            if (pk == null)
                return false;
            var datas = CacheStorage.AllCacheDatas;
            datas.ForEach(data =>
            {
                data.TryRemove(pk);
            });
            return true;
        }

        public bool ItemExistWithoutRelations(EntityKey pk)
        {
            return CacheStorage.AllCacheDatas.Any(d => d.Relations.Relations.Count == 0 && d.ItemExist(pk));
        }

        public bool ItemExistAndNotNull(EntityKey pk)
        {
            return CacheStorage.AllCacheDatas.Any(d =>
            {
                var cached = d.Get(pk);
                return cached?.EntityUntyped != null;
            });
        }

        public bool GetCacheExist(RelationInfo relationInfo)
        {
            return CacheStorage.GetCacheExist(relationInfo);
        }

        public bool GetIsCacheFullCollection(RelationInfo relationInfo)
        {
            return CacheStorage.GetIsCacheFullCollection(relationInfo);
        }

        public IEnumerable<ICacheData> GetAllCaches()
        {
            return CacheStorage.AllCacheDatas;
        }

        public void Dispose()
        {
            CacheStorage.Dispose();
        }

        public bool CanAddUpCache()
        {
            return CacheFactory.CanAddUpCache();
        }

        private ICollection<EntityKey> MarkForAddInternal<TItem>(ICollection<TItem> entities, object dbContext)
        {
            List<EntityKey> pks = new List<EntityKey>();
            if (entities.Count == 0)
                return pks;
            foreach (var data in CacheStorage.AllCacheDatas)
            {
                lock (data)
                {
                    if (data.FullCollection)
                    {
                        data.NeedUpdate = true;
                    }
                    var cachedList = pks.Where(p => p.IsValid).Select(pk => data.Get(pk)).ToArray();
                    var lockList = cachedList.Where(c => c != null).Select(c => c.Lock()).ToArray();
                    try
                    {
                        foreach (var cached in cachedList)
                        {
                            cached?.SetNeedUpdate(true, dbContext);
                        }
                    }
                    finally
                    {
                        lockList.ForEach(l => l.Dispose());
                    }
                }
            }
            var foreignKeys = EntityInfo.ForeignKeys.GetForeignKeysValues(entities);
            foreach (var entity in entities)
            {
                Update(entity, (DbContext) null, dbContext);
                pks.Add(EntityInfo.PrimaryKey.GetPrimaryKeyValue(entity));
            }

            MarkForUpdateForeignKeys(foreignKeys, dbContext);
            foreach (var pk in pks)
            {
                MarkForUpdateDependent(pk, dbContext);
            }
            return pks;
        }

        private void MarkForUpdateInternal(ICollection<EntityKey> pks, IEnumerable<ICacheData<T>> cacheDatas, string markRelated, object dbContext)
        {
            if (pks.Count == 0)
                return;
            foreach (var data in cacheDatas)
            {
                var cachedList = pks.Where(p => p.IsValid).Select(pk => data.Get(pk)).ToArray();
                var lockList = cachedList.Where(c => c != null).Select(c => c.Lock()).ToArray();
                try
                {
                    Dictionary<EntityForeignKeyInfo, HashSet<EntityForeignKey>> foreignKeys =
                        new Dictionary<EntityForeignKeyInfo, HashSet<EntityForeignKey>>();
                    lock (data)
                    {
                        foreach (var cached in cachedList)
                        {
                            if (cached == null && data.FullCollection)
                            {
                                data.NeedUpdate = true;
                            }
                        }
                        foreach (var group in cachedList.Where(cached =>
                        {
                            if (cached == null)
                            {
                                return false;
                            }
                            if (!string.IsNullOrWhiteSpace(markRelated) && !cached.NeedUpdateRelated.Contains(markRelated))
                            {
                                cached.NeedUpdateRelated.Add(markRelated);
                                return true;
                            }
                            if (cached.NeedUpdateEntity)
                            {
                                cached.NeedUpdateRelated.AddRange(EntityInfo.ImplicitUpdateMarkedEntities.Where(r => data.GetHasRelation(r)));
                                return false;
                            }
                            cached.NeedUpdateRelated.AddRange(EntityInfo.ImplicitUpdateMarkedEntities.Where(r => data.GetHasRelation(r)));
                            cached.SetNeedUpdate(true, dbContext);

                            return true;
                        }).Where(c => c.ForeignKeys != null).SelectMany(c => c.ForeignKeys).Where(f => f.Key != null).GroupBy(k => k.Key))
                        {
                            var keySet = new HashSet<EntityForeignKey>();
                            keySet.AddRange(group.Select(g => g.Value));
                            foreignKeys.Add(group.Key, keySet);
                        }
                        MarkForUpdateForeignKeys(foreignKeys, dbContext);
                    }
                }
                finally
                {
                    lockList.ForEach(l => l.Dispose());
                }
            }
            foreach (var pk in pks)
            {
                MarkForUpdateDependent(pk, dbContext);
            }
        }

        private void MarkForUpdateInternal(EntityKey pk, IEnumerable<ICacheData<T>> cacheDatas, string markRelated, object dbContext)
        {
            if (!pk.IsValid)
                return;
            foreach (var data in cacheDatas)
            {
                var cached = data.Get(pk);
                lock (data)
                {
                    if (cached != null)
                    {
                        using (cached.Lock())
                        {
                            if (!string.IsNullOrWhiteSpace(markRelated) && !cached.NeedUpdateRelated.Contains(markRelated))
                            {
                                cached.NeedUpdateRelated.Add(markRelated);
                                MarkForUpdateForeignKeys(cached.ForeignKeys, dbContext);
                            }
                            else if (!cached.NeedUpdateEntity)
                            {
                                cached.NeedUpdateRelated.AddRange(EntityInfo.ImplicitUpdateMarkedEntities.Where(r => data.GetHasRelation(r)));
                                cached.SetNeedUpdate(true, dbContext);
                                MarkForUpdateForeignKeys(cached.ForeignKeys, dbContext);
                            }
                            else
                            {
                                cached.NeedUpdateRelated.AddRange(EntityInfo.ImplicitUpdateMarkedEntities.Where(r => data.GetHasRelation(r)));
                            }
                        }
                    }
                    else if (data.FullCollection)
                    {
                        data.NeedUpdate = true;
                    }
                }
            }
            MarkForUpdateDependent(pk, dbContext);
        }

        private void MarkForUpdateDependent(EntityKey pk, object dbContext)
        {
            if (EntityInfo.DependentTypes == null)
                return;

            foreach (var dependentType in EntityInfo.DependentTypes)
            {
                if (CacheFactory.CacheExist(dependentType.Key))
                {
                    var cache = CacheFactory.GetCache(dependentType.Key);
                    var cacheDatas = cache.GetAllCaches().Where(c => !c.Empty && c.GetHasRelation(dependentType.Value.Name));
                    foreach (var data in cacheDatas)
                    {
                        var index = dependentType.Value.KeyMapping.MapPrincipalToForeign(pk);
                        var cachedItems = data.GetUntyped(dependentType.Value, index)?.ToArray();
                        if (cachedItems != null)
                        {
                            var lockList = cachedItems.Select(c => c.Lock()).ToArray();
                            try
                            {
                                var itemsList =
                                    cachedItems.Where(c => !c.NeedUpdateEntity)
                                        .Select(cached => cache.EntityInfo.PrimaryKey.GetPrimaryKeyValue(cached.EntityUntyped))
                                        .ToArray();
                                cache.MarkForUpdateByPrimaryKey(itemsList, dbContext, dependentType.Value.Name);
                            }
                            finally
                            {
                                lockList.ForEach(l => l.Dispose());
                            }
                        }
                    }
                }
            }
        }

        private void MarkForUpdateForeignKeys(IDictionary<EntityForeignKeyInfo, HashSet<EntityForeignKey>> foreignKeys, object dbContext)
        {
            if (foreignKeys == null)
                return;

            foreach (var keyGroup in foreignKeys.Where(k => CacheFactory.CacheExist(k.Key.DependentType)))
            {
                var cache = CacheFactory.GetCache(keyGroup.Key.DependentType);
                var itemPks = keyGroup.Value.Select(fk => keyGroup.Key.KeyMapping.MapForeignToPrincipal(fk));
                cache.MarkForUpdateByPrimaryKey(itemPks.ToArray(), dbContext, keyGroup.Key.Name);
            }
        }

        private void MarkForUpdateForeignKeys(IEnumerable<KeyValuePair<EntityForeignKeyInfo, EntityForeignKey>> foreignKeys, object dbContext)
        {
            if (foreignKeys == null)
                return;

            foreach (var foreignKey in foreignKeys)
            {
                if (foreignKey.Value != null && foreignKey.Value.IsValid && CacheFactory.CacheExist(foreignKey.Key.DependentType))
                {
                    var cache = CacheFactory.GetCache(foreignKey.Key.DependentType);
                    var itemPk = foreignKey.Key.KeyMapping.MapForeignToPrincipal(foreignKey.Value);
                    cache.MarkForUpdateByPrimaryKey(itemPk, dbContext, foreignKey.Key.Name);
                }
            }
        }
    }
}