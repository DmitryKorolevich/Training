using Microsoft.EntityFrameworkCore;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using VitalChoice.Caching.Extensions;
using VitalChoice.Caching.Interfaces;
using VitalChoice.Caching.Relational;
using VitalChoice.Caching.Relational.ChangeTracking;
using VitalChoice.Caching.Services.Cache.Base;
using VitalChoice.Data.Extensions;
using VitalChoice.Ecommerce.Domain.Helpers;
using VitalChoice.ObjectMapping.Interfaces;
using VitalChoice.Profiling.Base;

namespace VitalChoice.Caching.Services.Cache
{
    public class EntityInternalCache<T> : IInternalEntityCache<T>
    {
        public ICacheData GetCacheData(RelationInfo relationInfo)
        {
            return CacheStorage.GetCacheData(relationInfo);
        }

        public EntityInfo EntityInfo { get; }
        private readonly IEntityInfoStorage _infoStorage;
        protected readonly IInternalEntityCacheFactory CacheFactory;

        protected readonly CacheStorage<T> CacheStorage;

        public EntityInternalCache(EntityInfo entityInfo, IEntityInfoStorage infoStorage, IInternalEntityCacheFactory cacheFactory)
        {
            EntityInfo = entityInfo;
            //KeyStorage = keyStorage;
            _infoStorage = infoStorage;
            CacheFactory = cacheFactory;
            CacheStorage = new CacheStorage<T>(entityInfo, infoStorage, cacheFactory);
        }

        public CacheResult<T> TryGetEntity(EntityKey key, RelationInfo relations)
        {
            var data = CacheStorage.GetCacheData(relations);
            var cached = data.Get(key);
            if (cached != null)
            {
                return cached;
            }
            return CacheGetResult.Update;
        }

        public IEnumerable<CacheResult<T>> TryGetEntities(ICollection<EntityKey> primaryKeys, RelationInfo relations)
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
                yield return cached;
            }
        }

        public CacheResult<T> TryGetEntity(EntityIndex key, RelationInfo relationInfo)
        {
            var data = CacheStorage.GetCacheData(relationInfo);
            var cached = data.Get(key);
            if (cached != null)
            {
                return cached;
            }
            return CacheGetResult.Update;
        }

        public IEnumerable<CacheResult<T>> TryGetEntities(ICollection<EntityIndex> indexes, RelationInfo relations)
        {
            var data = CacheStorage.GetCacheData(relations);
            foreach (var index in indexes)
            {
                var cached = data.Get(index);
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
                yield return cached;
            }
        }

        public CacheResult<T> TryGetEntity(EntityIndex key, EntityConditionalIndexInfo conditionalInfo, RelationInfo relations)
        {
            var data = CacheStorage.GetCacheData(relations);
            var cached = data.Get(conditionalInfo, key);
            if (cached != null)
            {
                return cached;
            }
            return CacheGetResult.Update;
        }

        public IEnumerable<CacheResult<T>> TryGetEntities(ICollection<EntityIndex> indexes, EntityConditionalIndexInfo conditionalInfo,
            RelationInfo relations)
        {
            var data = CacheStorage.GetCacheData(relations);
            foreach (var index in indexes)
            {
                var cached = data.Get(conditionalInfo, index);
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
                yield return cached;
            }
        }

        public IEnumerable<CacheResult<T>> GetWhere(RelationInfo relations, Func<T, bool> whereFunc)
        {
            var data = CacheStorage.GetCacheData(relations);
            if (!data.FullCollection)
            {
                yield return CacheGetResult.NotFound;
                yield break;
            }
            lock (data.LockObj)
            {
                var allItems = data.GetAll();
                if (data.Empty || allItems.Any(cached => cached.NeedUpdate))
                {
                    yield return CacheGetResult.Update;
                    yield break;
                }
                foreach (var cached in allItems.Where(cached => whereFunc(cached)))
                {
                    yield return cached;
                }
            }
        }

        public bool IsFullCollection(RelationInfo relations)
        {
            return CacheStorage.GetCacheData(relations).FullCollection;
        }

        public IEnumerable<CacheResult<T>> GetAll(RelationInfo relations)
        {
            var data = CacheStorage.GetCacheData(relations);
            if (!data.FullCollection)
            {
                yield return CacheGetResult.NotFound;
                yield break;
            }
            lock (data.LockObj)
            {
                var allItems = data.GetAll();
                if (data.NeedUpdate || data.Empty || allItems.Any(cached => cached.NeedUpdate))
                {
                    yield return CacheGetResult.Update;
                    yield break;
                }
                foreach (var cached in allItems)
                {
                    yield return cached;
                }
            }
        }

        public IEnumerable<CacheResult<T>> TryRemoveWithResult(T entity)
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
                yield return removed;
            }
        }

        public bool TryRemove(T entity)
        {
            var pk = EntityInfo.PrimaryKey.GetPrimaryKeyValue(entity);
            return TryRemove(pk);
        }

        public bool Update(IEnumerable<T> entities, RelationInfo relationInfo)
        {
            var data = CacheStorage.GetCacheData(relationInfo);
            return data.Update(entities);
        }

        public bool Update(T entity, RelationInfo relationInfo)
        {
            if (entity == null)
                return false;

            var data = CacheStorage.GetCacheData(relationInfo);
            return data.Update(entity) != null;
        }

        public bool Update(T entity, DbContext context)
        {
            if (entity == null)
                return false;

            var trackData = _infoStorage.GetTrackData(context);

            var result = CacheStorage.AllCacheDatas.Any();
            // ReSharper disable once LoopCanBeConvertedToQuery
            foreach (var data in CacheStorage.AllCacheDatas)
            {
                result = result && data.UpdateKeepRelations(entity, trackData) != null;
            }
            return result;
        }

        public CachedEntity<T> Update(RelationInfo relations, T entity)
        {
            var data = CacheStorage.GetCacheData(relations);
            return data.Update(entity);
        }

        public IEnumerable<CachedEntity<T>> Update(RelationInfo relations, IEnumerable<T> entities)
        {
            var data = CacheStorage.GetCacheData(relations);
            return entities.Select(entity => data.Update(entity));
        }

        public bool UpdateAll(IEnumerable<T> entities, RelationInfo relationInfo)
        {
            var data = CacheStorage.GetCacheData(relationInfo);
            return data.UpdateAll(entities);
        }

        public EntityKey MarkForUpdate(T entity)
        {
            var pk = EntityInfo.PrimaryKey.GetPrimaryKeyValue(entity);
            MarkForUpdate(pk);
            return pk;
        }

        public ICollection<EntityKey> MarkForUpdate(IEnumerable<T> entities)
        {
            var pks = new HashSet<EntityKey>(entities.Select(e => EntityInfo.PrimaryKey.GetPrimaryKeyValue(e)));
            MarkForUpdate(pks);
            return pks;
        }

        public EntityKey MarkForAdd(T entity)
        {
#if DEBUG
            using (var scope = new ProfilingScope($"<{typeof(T).Name} Mark Add 1>"))
#endif
            {
                foreach (var data in CacheStorage.AllCacheDatas)
                {
                    if (data.FullCollection)
                    {
                        data.NeedUpdate = true;
                    }
                }
                Update(entity, (DbContext)null);
                var pk = EntityInfo.PrimaryKey.GetPrimaryKeyValue(entity);
                var foreignKeys = EntityInfo.ForeignKeys.GetForeignKeyValues(entity);
                MarkForUpdateForeignKeys(foreignKeys);
#if DEBUG
                MarkForUpdateDependent(pk, scope);
#else
                MarkForUpdateDependent(pk);
#endif
                return pk;
            }
        }

        public ICollection<EntityKey> MarkForAdd(ICollection<T> entities)
        {
            return MarkForAddInternal(entities);
        }

        public CachedEntity Update(RelationInfo relations, object entity)
        {
            return Update(relations, (T) entity);
        }

        public IEnumerable<CachedEntity> Update(RelationInfo relations, IEnumerable<object> entity)
        {
            return Update(relations, entity.Cast<T>());
        }

        public bool Update(IEnumerable<object> entities, RelationInfo relationInfo)
        {
            return Update(entities.Cast<T>(), relationInfo);
        }

        public bool Update(object entity, RelationInfo relationInfo)
        {
            return Update((T) entity, relationInfo);
        }

        public bool Update(object entity, DbContext context)
        {
            return Update((T) entity, context);
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

        public bool UpdateAll(IEnumerable<object> entities, RelationInfo relationInfo)
        {
            return UpdateAll(entities.Cast<T>(), relationInfo);
        }

        public EntityKey MarkForUpdate(object entity)
        {
            return MarkForUpdate((T) entity);
        }

        public ICollection<EntityKey> MarkForUpdate(IEnumerable<object> entities)
        {
            var pks = entities.Select(e => EntityInfo.PrimaryKey.GetPrimaryKeyValue(e)).ToList();
            MarkForUpdate(pks);
            return pks;
        }

        public void MarkForUpdate(EntityKey pk, string hasRelation = null)
        {
            if (pk == null)
                return;
            IEnumerable<ICacheData<T>> cacheDatas = CacheStorage.AllCacheDatas;
            if (!string.IsNullOrWhiteSpace(hasRelation))
            {
                cacheDatas = cacheDatas.Where(c => c.GetHasRelation(hasRelation));
            }
            MarkForUpdateInternal(pk, cacheDatas, hasRelation);
        }

        public void MarkForUpdate(ICollection<EntityKey> pks, string hasRelation = null)
        {
            if (pks == null)
                return;
            IEnumerable<ICacheData<T>> cacheDatas = CacheStorage.AllCacheDatas;
            if (!string.IsNullOrWhiteSpace(hasRelation))
            {
                cacheDatas = cacheDatas.Where(c => c.GetHasRelation(hasRelation)).ToArray();
            }
            MarkForUpdateInternal(pks, cacheDatas, hasRelation);
        }

        public EntityKey MarkForAdd(object entity)
        {
            return MarkForAdd((T) entity);
        }

        public ICollection<EntityKey> MarkForAdd(ICollection<object> entities)
        {
            return MarkForAddInternal(entities);
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

        public bool ItemExist(EntityKey pk)
        {
            return CacheStorage.AllCacheDatas.Any(d => d.ItemExist(pk));
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

        private ICollection<EntityKey> MarkForAddInternal<TItem>(ICollection<TItem> entities)
        {
            List<EntityKey> pks = new List<EntityKey>();
            if (entities.Count == 0)
                return pks;
#if DEBUG
            using (var scope = new ProfilingScope($"<{typeof(T).Name}> Mark Add {entities.Count}"))
#endif
            {
                foreach (var data in CacheStorage.AllCacheDatas)
                {
                    if (data.FullCollection)
                    {
                        data.NeedUpdate = true;
                    }
                }
                var foreignKeys = EntityInfo.ForeignKeys.GetForeignKeysValues(entities);
                foreach (var entity in entities)
                {
                    Update(entity, (DbContext)null);
                    pks.Add(EntityInfo.PrimaryKey.GetPrimaryKeyValue(entity));
                }

                MarkForUpdateForeignKeys(foreignKeys);
                foreach (var pk in pks)
                {
#if DEBUG
                    MarkForUpdateDependent(pk, scope);
#else
                    MarkForUpdateDependent(pk);
#endif
                }
                return pks;
            }
        }

        private void MarkForUpdateInternal(ICollection<EntityKey> pks, IEnumerable<ICacheData<T>> cacheDatas, string markRelated)
        {
            if (pks.Count == 0)
                return;
#if DEBUG
            using (var scope = new ProfilingScope($"<{typeof(T).Name}> Mark Update (Related: {markRelated}) {pks.Count}"))
#endif
            {
                foreach (var data in cacheDatas)
                {
                    var cachedList = pks.Where(p => p.IsValid).Select(pk => data.Get(pk)).ToArray();

                    foreach (var cached in cachedList)
                    {
                        if (cached == null && data.FullCollection)
                        {
                            data.NeedUpdate = true;
                        }
                    }
                    var foreignKeys = new Dictionary<EntityForeignKeyInfo, HashSet<EntityForeignKey>>();
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

                        if (cached.NeedUpdate)
                            return false;

                        cached.NeedUpdate = true;
                        return true;
                    }).SelectMany(c => c.ForeignKeys).GroupBy(k => k.Key))
                    {
                        var keySet = new HashSet<EntityForeignKey>();
                        keySet.AddRange(group.Select(g => g.Value));
                        foreignKeys.Add(group.Key, keySet);
                    }
                    MarkForUpdateForeignKeys(foreignKeys);
                }
                foreach (var pk in pks)
                {
#if DEBUG
                    MarkForUpdateDependent(pk, scope);
#else
                    MarkForUpdateDependent(pk);
#endif
                }
            }
        }

        private void MarkForUpdateInternal(EntityKey pk, IEnumerable<ICacheData<T>> cacheDatas, string markRelated)
        {
            if (!pk.IsValid)
                return;
#if DEBUG
            using (var scope = new ProfilingScope($"<{typeof(T).Name}> Mark Update (Related: {markRelated}) 1"))
#endif
            {
                foreach (var data in cacheDatas)
                {
                    var cached = data.Get(pk);
                    if (cached != null)
                    {
                        if (!string.IsNullOrWhiteSpace(markRelated) && !cached.NeedUpdateRelated.Contains(markRelated))
                        {
                            cached.NeedUpdateRelated.Add(markRelated);
                            MarkForUpdateForeignKeys(cached.ForeignKeys);
                        }
                        else if (!cached.NeedUpdate)
                        {
                            cached.NeedUpdate = true;
                            MarkForUpdateForeignKeys(cached.ForeignKeys);
                        }
                    }
                    else if (data.FullCollection)
                    {
                        data.NeedUpdate = true;
                    }
                }
#if DEBUG
                MarkForUpdateDependent(pk, scope);
#else
                MarkForUpdateDependent(pk);
#endif
            }
        }
#if DEBUG
        private void MarkForUpdateDependent(EntityKey pk, ProfilingScope scope)
#else
        private void MarkForUpdateDependent(EntityKey pk)
#endif
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
                        var cachedItems = data.GetUntyped(dependentType.Value, dependentType.Value.KeyMapping.MapPrincipalToForeign(pk));
                        if (cachedItems != null)
                        {
                            var itemsList = cachedItems.Where(c => !c.NeedUpdate)
                                .Select(entity => cache.EntityInfo.PrimaryKey.GetPrimaryKeyValue(entity.EntityUntyped)).ToArray();
                            cache.MarkForUpdate(itemsList, dependentType.Value.Name);
                            if (!itemsList.Any())
                            {
#if DEBUG
                                scope.AddScopeData($"{dependentType.Key.Name} empty result {index}");
#endif
                            }
                        }
                        else
                        {
#if DEBUG
                            scope.AddScopeData($"{dependentType.Key.Name} not found {index}");
#endif
                        }
                    }
                }
                else
                {
#if DEBUG
                    scope.AddScopeData($"{dependentType.Key.Name} cache doesn't exist");
#endif
                }
            }
        }

        private void MarkForUpdateForeignKeys(IDictionary<EntityForeignKeyInfo, HashSet<EntityForeignKey>> foreignKeys)
        {
            if (foreignKeys == null)
                return;

            foreach (var keyGroup in foreignKeys.Where(k => CacheFactory.CacheExist(k.Key.DependentType)))
            {
                var cache = CacheFactory.GetCache(keyGroup.Key.DependentType);
                var collectionForeignKey = keyGroup.Key as EntityForeignKeyCollectionInfo;
                if (collectionForeignKey != null)
                {
                    var collection =
                        keyGroup.Value.Where(k => k != null && k.IsValid && k.Values[0].Value is IEnumerable<object>)
                            .SelectMany(k => k.Values[0].Value as IEnumerable<object>);
                    cache.MarkForUpdate(collection.Select(item => cache.EntityInfo.PrimaryKey.GetPrimaryKeyValue(item)).ToArray(), keyGroup.Key.Name);
                }
                else
                {
                    var itemPks = keyGroup.Value.Select(fk => keyGroup.Key.KeyMapping.MapForeignToPrincipal(fk));
                    cache.MarkForUpdate(itemPks.ToArray(), keyGroup.Key.Name);
                }
            }
        }

        private void MarkForUpdateForeignKeys(IEnumerable<KeyValuePair<EntityForeignKeyInfo, EntityForeignKey>> foreignKeys)
        {
            if (foreignKeys == null)
                return;

            foreach (var foreignKey in foreignKeys)
            {
                if (foreignKey.Value != null && foreignKey.Value.IsValid && CacheFactory.CacheExist(foreignKey.Key.DependentType))
                {
                    var cache = CacheFactory.GetCache(foreignKey.Key.DependentType);
                    var collectionForeignKey = foreignKey.Key as EntityForeignKeyCollectionInfo;
                    if (collectionForeignKey != null)
                    {
                        var collection = foreignKey.Value.Values[0].Value as IEnumerable<object>;
                        if (collection != null)
                        {
                            cache.MarkForUpdate(collection.Select(item => cache.EntityInfo.PrimaryKey.GetPrimaryKeyValue(item)).ToArray(),
                                foreignKey.Key.Name);
                        }
                    }
                    else
                    {
                        var itemPk = foreignKey.Key.KeyMapping.MapForeignToPrincipal(foreignKey.Value);
                        cache.MarkForUpdate(itemPk, foreignKey.Key.Name);
                    }
                }
            }
        }
    }
}