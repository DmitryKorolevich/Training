﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using VitalChoice.Caching.Interfaces;
using VitalChoice.Caching.Relational;
using VitalChoice.Caching.Services.Cache.Base;
using VitalChoice.Data.Extensions;
using VitalChoice.Ecommerce.Domain.Helpers;
using VitalChoice.ObjectMapping.Interfaces;

namespace VitalChoice.Caching.Services.Cache
{
    public class EntityInternalCache<T> : IInternalEntityCache<T>
    {
        protected readonly IEntityInfoStorage KeyStorage;
        protected readonly IInternalEntityCacheFactory CacheFactory;

        protected readonly CacheStorage<T> CacheStorage;

        public EntityInternalCache(IEntityInfoStorage keyStorage, IInternalEntityCacheFactory cacheFactory)
        {
            KeyStorage = keyStorage;
            CacheFactory = cacheFactory;
            CacheStorage = new CacheStorage<T>(keyStorage, cacheFactory);
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
                    }
                }
                else
                {
                    yield return CacheGetResult.Update;
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
                        yield return CacheGetResult.Update;
                }
                else
                {
                    yield return CacheGetResult.Update;
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
                        yield return CacheGetResult.Update;
                }
                else
                {
                    yield return CacheGetResult.Update;
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
            }
            var allItems = data.GetAll();
            if (data.Empty || allItems.Any(cached => cached.NeedUpdate))
            {
                yield return CacheGetResult.Update;
            }
            foreach (var cached in allItems.Where(cached => whereFunc(cached)))
            {
                yield return cached;
            }
        }

        public IEnumerable<CacheResult<T>> GetAll(RelationInfo relations)
        {
            if (!CacheStorage.GetCacheExist(relations))
                yield return CacheGetResult.Update;
            var data = CacheStorage.GetCacheData(relations);
            if (!data.FullCollection)
            {
                yield return CacheGetResult.NotFound;
            }
            var allItems = data.GetAll();
            if (data.Empty || allItems.Any(cached => cached.NeedUpdate))
            {
                yield return CacheGetResult.Update;
            }
            foreach (var cached in allItems)
            {
                yield return cached;
            }
        }

        public IEnumerable<CacheResult<T>> TryRemoveWithResult(T entity)
        {
            var pk = CacheStorage.GetPrimaryKeyValue(entity);
            var datas = CacheStorage.AllCacheDatas;
            foreach (var data in datas)
            {
                var removed = data.TryRemove(pk);
                if (removed == null)
                    yield return CacheGetResult.NotFound;
                yield return removed;
            }
        }

        public bool TryRemove(T entity)
        {
            var pk = CacheStorage.GetPrimaryKeyValue(entity);
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

        public bool Update(T entity)
        {
            if (entity == null)
                return false;

            MarkForUpdate(entity);
            return false;
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
            var pk = CacheStorage.GetPrimaryKeyValue(entity);
            MarkForUpdate(pk);
            return pk;
        }

        public IEnumerable<EntityKey> MarkForUpdate(IEnumerable<T> entities)
        {
            return entities.Select(MarkForUpdate);
        }

        public EntityKey MarkForAdd(T entity)
        {
            var pk = GetPrimaryKeyValue(entity);
            var foreignKeys = GetForeignKeyValues(entity);
            MarkForUpdateForeignKeys(foreignKeys);
            MarkForUpdateDependent(pk);
            return pk;
        }

        public IEnumerable<EntityKey> MarkForAdd(IEnumerable<T> entities)
        {
            return entities.Select(MarkForAdd);
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

        public bool Update(object entity)
        {
            return Update((T) entity);
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

        public IEnumerable<EntityKey> MarkForUpdate(IEnumerable<object> entities)
        {
            return MarkForUpdate(entities.Cast<T>());
        }

        public void MarkForUpdate(EntityKey pk)
        {
            MarkForUpdate(pk, null);
        }

        public void MarkForUpdate(EntityKey pk, string hasRelation)
        {
            if (pk == null)
                return;
            IEnumerable<ICacheData<T>> cacheDatas = CacheStorage.AllCacheDatas;
            if (!string.IsNullOrWhiteSpace(hasRelation))
            {
                cacheDatas = cacheDatas.Where(c => c.GetHasRelation(hasRelation));
            }
            MarkForUpdateInternal(pk, cacheDatas);
        }

        public void MarkForUpdate(IEnumerable<EntityKey> pks)
        {
            foreach (var pk in pks)
            {
                MarkForUpdate(pk);
            }
        }

        public void MarkForUpdate(IEnumerable<EntityKey> pks, string hasRelation)
        {
            if (pks == null)
                return;
            IEnumerable<ICacheData<T>> cacheDatas = CacheStorage.AllCacheDatas;
            if (!string.IsNullOrWhiteSpace(hasRelation))
            {
                cacheDatas = cacheDatas.Where(c => c.GetHasRelation(hasRelation)).ToArray();
            }
            foreach (var pk in pks)
            {
                MarkForUpdateInternal(pk, cacheDatas);
            }
        }

        public EntityKey MarkForAdd(object entity)
        {
            return MarkForAdd((T) entity);
        }

        public IEnumerable<EntityKey> MarkForAdd(IEnumerable<object> entities)
        {
            return MarkForAdd(entities.Cast<T>());
        }

        public bool TryRemove(object entity)
        {
            return TryRemove((T) entity);
        }

        public bool TryRemove(EntityKey pk)
        {
            if (pk == null)
                return false;
            MarkForUpdateDependent(pk);
            var datas = CacheStorage.AllCacheDatas;
            datas.ForEach(data =>
            {
                var cached = data.TryRemove(pk);
                if (cached != null)
                {
                    MarkForUpdateForeignKeys(cached.ForeignKeys);
                }
            });
            return true;
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

        public EntityForeignKey GetForeignKeyValue(T entity, EntityForeignKeyInfo foreignKeyInfo)
        {
            return CacheStorage.GetForeignKeyValue(entity, foreignKeyInfo);
        }

        public ICollection<KeyValuePair<EntityForeignKeyInfo, EntityForeignKey>> GetForeignKeyValues(T entity)
        {
            return CacheStorage.GetForeignKeyValues(entity);
        }

        public EntityIndex GetNonUniqueIndexValue(T entity, EntityCacheableIndexInfo indexInfo)
        {
            return CacheStorage.GetNonUniqueIndexValue(entity, indexInfo);
        }

        public ICollection<KeyValuePair<EntityCacheableIndexInfo, EntityIndex>> GetNonUniqueIndexValues(T entity)
        {
            return CacheStorage.GetNonUniqueIndexValues(entity);
        }

        public EntityKey GetPrimaryKeyValue(T entity)
        {
            return CacheStorage.GetPrimaryKeyValue(entity);
        }

        public EntityIndex GetIndexValue(T entity)
        {
            return CacheStorage.GetIndexValue(entity);
        }

        public EntityIndex GetConditionalIndexValue(T entity, EntityConditionalIndexInfo conditionalInfo)
        {
            return CacheStorage.GetConditionalIndexValue(entity, conditionalInfo);
        }

        public EntityForeignKey GetForeignKeyValue(object entity, EntityForeignKeyInfo foreignKeyInfo)
        {
            return CacheStorage.GetForeignKeyValue(entity, foreignKeyInfo);
        }

        public ICollection<KeyValuePair<EntityForeignKeyInfo, EntityForeignKey>> GetForeignKeyValues(object entity)
        {
            return CacheStorage.GetForeignKeyValues(entity);
        }

        public EntityIndex GetNonUniqueIndexValue(object entity, EntityCacheableIndexInfo indexInfo)
        {
            return CacheStorage.GetNonUniqueIndexValue(entity, indexInfo);
        }

        public ICollection<KeyValuePair<EntityCacheableIndexInfo, EntityIndex>> GetNonUniqueIndexValues(object entity)
        {
            return CacheStorage.GetNonUniqueIndexValues(entity);
        }

        public EntityKey GetPrimaryKeyValue(object entity)
        {
            return CacheStorage.GetPrimaryKeyValue(entity);
        }

        public EntityIndex GetIndexValue(object entity)
        {
            return CacheStorage.GetIndexValue(entity);
        }

        public EntityIndex GetConditionalIndexValue(object entity, EntityConditionalIndexInfo conditionalInfo)
        {
            return CacheStorage.GetConditionalIndexValue(entity, conditionalInfo);
        }

        public ICollection<KeyValuePair<Type, EntityCacheableIndexRelationInfo>> DependentTypes => CacheStorage.DependentTypes;

        public void Dispose()
        {
            CacheStorage.Dispose();
        }

        private void MarkForUpdateInternal(EntityKey pk, IEnumerable<ICacheData<T>> cacheDatas)
        {
            foreach (var data in cacheDatas)
            {
                var cached = data.Get(pk);
                if (cached != null)
                {
                    if (!cached.NeedUpdate)
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
            MarkForUpdateDependent(pk);
        }

        private void MarkForUpdateDependent(EntityKey pk)
        {
            if (DependentTypes == null)
                return;

            foreach (var dependentType in DependentTypes)
            {
                if (CacheFactory.CacheExist(dependentType.Key))
                {
                    var cache = CacheFactory.GetCache(dependentType.Key);
                    var cacheDatas = cache.GetAllCaches().Where(c => !c.Empty && c.GetHasRelation(dependentType.Value.Name));
                    foreach (var data in cacheDatas)
                    {
                        var cachedItems = data.GetUntyped(dependentType.Value, dependentType.Value.KeyMapping.MapPrincipalToForeign(pk));
                        if (cachedItems != null)
                        {
                            cache.MarkForUpdate(
                                cachedItems.Where(c => !c.NeedUpdate)
                                    .Select(entity => cache.GetPrimaryKeyValue(entity.EntityUntyped)), dependentType.Value.Name);
                        }
                    }
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
                        var collection = foreignKey.Value.Values[0].Value as IEnumerable;
                        if (collection != null)
                        {
                            cache.MarkForUpdate(collection.Cast<object>().Select(item => cache.GetPrimaryKeyValue(item)),
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

        public bool CanAddUpCache()
        {
            return CacheFactory.CanAddUpCache();
        }
    }
}