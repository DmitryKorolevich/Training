﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using VitalChoice.Caching.Interfaces;
using VitalChoice.Caching.Relational;
using VitalChoice.Caching.Services.Cache.Base;
using VitalChoice.Ecommerce.Domain.Helpers;
using VitalChoice.ObjectMapping.Interfaces;

namespace VitalChoice.Caching.Services.Cache
{
    public class EntityInternalCache<T> : IInternalEntityCache<T>
    {
        protected readonly IInternalEntityInfoStorage KeyStorage;
        protected readonly IInternalEntityCacheFactory CacheFactory;

        protected readonly CacheStorage<T> CacheStorage;

        public EntityInternalCache(IInternalEntityInfoStorage keyStorage, IInternalEntityCacheFactory cacheFactory,
            ITypeConverter typeConverter)
        {
            KeyStorage = keyStorage;
            CacheFactory = cacheFactory;
            CacheStorage = new CacheStorage<T>(keyStorage, cacheFactory, typeConverter);
        }

        public CacheResult<T> TryGetEntity(EntityKey key, RelationInfo relations)
        {
            CachedEntity<T> cached;
            var data = CacheStorage.GetCacheData(relations);
            if (data.Get(key, out cached))
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
                CachedEntity<T> cached;
                if (data.Get(key, out cached))
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
            CachedEntity<T> cached;
            var data = CacheStorage.GetCacheData(relationInfo);
            if (data.Get(key, out cached))
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
                CachedEntity<T> cached;
                if (data.Get(index, out cached))
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
            CachedEntity<T> cached;
            var data = CacheStorage.GetCacheData(relations);
            if (data.Get(conditionalInfo, key, out cached))
            {
                return cached;
            }
            return CacheGetResult.Update;
        }

        public IEnumerable<CacheResult<T>> TryGetEntities(ICollection<EntityIndex> indexes, EntityConditionalIndexInfo conditionalInfo, RelationInfo relations)
        {
            var data = CacheStorage.GetCacheData(relations);
            foreach (var index in indexes)
            {
                CachedEntity<T> cached;
                if (data.Get(conditionalInfo, index, out cached))
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
                CachedEntity<T> removed;
                if (!data.TryRemove(pk, out removed))
                    yield return CacheGetResult.NotFound;
                yield return removed;
            }
        }

        public bool TryRemove(T entity)
        {
            var pk = CacheStorage.GetPrimaryKeyValue(entity);
            var datas = CacheStorage.AllCacheDatas;
            return datas.Aggregate(true, (current, data) => current && data.TryRemove(pk));
        }

        public void Update(IEnumerable<T> entities, RelationInfo relationInfo)
        {
            var data = CacheStorage.GetCacheData(relationInfo);
            data.Update(entities);
        }

        public void Update(T entity, RelationInfo relationInfo)
        {
            if (entity == null)
                return;

            var data = CacheStorage.GetCacheData(relationInfo);
            data.Update(entity);
        }

        public void Update(T entity)
        {
            if (entity == null)
                return;

            var datas = CacheStorage.AllCacheDatas;
            foreach (var data in datas)
            {
                data.Update(entity, true);
            }
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

        public void UpdateAll(IEnumerable<T> entities, RelationInfo relationInfo)
        {
            var data = CacheStorage.GetCacheData(relationInfo);
            data.UpdateAll(entities);
        }

        public void MarkForUpdate(T entity)
        {
            var pk = CacheStorage.GetPrimaryKeyValue(entity);
            foreach (var data in CacheStorage.AllCacheDatas)
            {
                CachedEntity<T> cached;
                if (data.Get(pk, out cached))
                {
                    cached.NeedUpdate = true;
                }
                else if (data.FullCollection)
                {
                    data.NeedUpdate = true;
                }
            }
        }

        public void MarkForUpdate(IEnumerable<T> entities)
        {
            foreach (var entity in entities)
            {
                MarkForUpdate(entity);
            }
        }

        public CachedEntity Update(RelationInfo relations, object entity)
        {
            return Update(relations, (T) entity);
        }

        public IEnumerable<CachedEntity> Update(RelationInfo relations, IEnumerable<object> entity)
        {
            return Update(relations, entity.Cast<T>());
        }

        public void Update(IEnumerable<object> entities, RelationInfo relationInfo)
        {
            Update(entities.Cast<T>(), relationInfo);
        }

        public void Update(object entity, RelationInfo relationInfo)
        {
            Update((T) entity, relationInfo);
        }

        public void Update(object entity)
        {
            Update((T) entity);
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

        public void UpdateAll(IEnumerable<object> entities, RelationInfo relationInfo)
        {
            UpdateAll(entities.Cast<T>(), relationInfo);
        }

        public void MarkForUpdate(object entity)
        {
            MarkForUpdate((T) entity);
        }

        public void MarkForUpdate(IEnumerable<object> entities)
        {
            MarkForUpdate(entities.Cast<T>());
        }

        public bool TryRemove(object entity)
        {
            return TryRemove((T) entity);
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

        public void Dispose()
        {
            CacheStorage.Dispose();
        }
    }
}