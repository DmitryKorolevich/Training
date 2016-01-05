using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using VitalChoice.Caching.Interfaces;
using VitalChoice.Caching.Relational;
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

        public CacheGetResult TryGetEntity(EntityKey key, RelationInfo relations, out T entity)
        {
            CachedEntity<T> cached;
            var data = CacheStorage.GetCacheData(relations);
            if (data.Get(key, out cached))
            {
                entity = cached;
                return cached.NeedUpdate ? CacheGetResult.Update : CacheGetResult.Found;
            }
            entity = default(T);
            return CacheGetResult.Update;
        }

        public CacheGetResult TryGetEntities(ICollection<EntityKey> primaryKeys, RelationInfo relations, Expression<Func<T, bool>> whereExpression, out List<T> entities)
        {
            entities = new List<T>(primaryKeys.Count);
            var data = CacheStorage.GetCacheData(relations);
            var whereFunc = whereExpression?.CacheCompile();
            foreach (var key in primaryKeys)
            {
                CachedEntity<T> cached;
                if (data.Get(key, out cached))
                {
                    if (cached.NeedUpdate)
                        return CacheGetResult.Update;
                }
                else
                {
                    return CacheGetResult.Update;
                }
                if (whereFunc?.Invoke(cached) ?? true)
                    entities.Add(cached);
            }
            return CacheGetResult.Found;
        }

        public CacheGetResult TryGetEntity(EntityIndex key, RelationInfo relationInfo, out T entity)
        {
            CachedEntity<T> cached;
            var data = CacheStorage.GetCacheData(relationInfo);
            if (data.Get(key, out cached))
            {
                entity = cached;
                return cached.NeedUpdate ? CacheGetResult.Update : CacheGetResult.Found;
            }
            entity = default(T);
            return CacheGetResult.Update;
        }

        public CacheGetResult TryGetEntities(ICollection<EntityIndex> indexes, RelationInfo relations, Expression<Func<T, bool>> whereExpression, out List<T> entities)
        {
            entities = new List<T>(indexes.Count);
            var data = CacheStorage.GetCacheData(relations);
            var whereFunc = whereExpression?.CacheCompile();
            foreach (var index in indexes)
            {
                CachedEntity<T> cached;
                if (data.Get(index, out cached))
                {
                    if (cached.NeedUpdate)
                        return CacheGetResult.Update;
                }
                else
                {
                    return CacheGetResult.Update;
                }
                if (whereFunc?.Invoke(cached) ?? true)
                    entities.Add(cached);
            }
            return CacheGetResult.Found;
        }

        public CacheGetResult TryGetEntity(EntityIndex key, EntityConditionalIndexInfo conditionalInfo, RelationInfo relations, out T entity)
        {
            CachedEntity<T> cached;
            var data = CacheStorage.GetCacheData(relations);
            if (data.Get(conditionalInfo, key, out cached))
            {
                entity = cached;
                return cached.NeedUpdate ? CacheGetResult.Update : CacheGetResult.Found;
            }
            entity = default(T);
            return CacheGetResult.Update;
        }

        public CacheGetResult TryGetEntities(ICollection<EntityIndex> indexes, EntityConditionalIndexInfo conditionalInfo, RelationInfo relations,
            Expression<Func<T, bool>> whereExpression, out List<T> entities)
        {
            entities = new List<T>(indexes.Count);
            var data = CacheStorage.GetCacheData(relations);
            var whereFunc = whereExpression?.CacheCompile();
            foreach (var index in indexes)
            {
                CachedEntity<T> cached;
                if (data.Get(conditionalInfo, index, out cached))
                {
                    if (cached.NeedUpdate)
                        return CacheGetResult.Update;
                }
                else
                {
                    return CacheGetResult.Update;
                }
                if (whereFunc?.Invoke(cached) ?? true)
                    entities.Add(cached);
            }
            return CacheGetResult.Found;
        }

        public CacheGetResult GetWhere(RelationInfo relations, Func<T, bool> whereFunc, out List<T> entities)
        {
            var data = CacheStorage.GetCacheData(relations);
            if (!data.Empty && !data.FullCollection)
            {
                entities = null;
                return CacheGetResult.NotFound;
            }
            if (data.GetAll().Any(cached => cached.NeedUpdate))
            {
                entities = null;
                return CacheGetResult.Update;
            }
            entities = data.GetAll().Where(cached => whereFunc(cached)).Select(cached => cached.Entity).ToList();
            return CacheGetResult.Found;
        }

        public CacheGetResult GetWhere(RelationInfo relations, Expression<Func<T, bool>> whereExpression, out List<T> entities)
        {
            var whereFunc = whereExpression.CacheCompile();
            return GetWhere(relations, whereFunc, out entities);
        }

        public CacheGetResult GetAll(RelationInfo relations, out List<T> entities)
        {
            var data = CacheStorage.GetCacheData(relations);
            if (!data.Empty && !data.FullCollection)
            {
                entities = null;
                return CacheGetResult.NotFound;
            }
            if (data.GetAll().Any(cached => cached.NeedUpdate))
            {
                entities = null;
                return CacheGetResult.Update;
            }
            entities = data.GetAll().Select(cached => cached.Entity).ToList();
            return CacheGetResult.Found;
        }

        public CacheGetResult GetFirstWhere(RelationInfo relations, Func<T, bool> whereFunc, out T entity)
        {
            var data = CacheStorage.GetCacheData(relations);
            if (!data.Empty && !data.FullCollection)
            {
                entity = default(T);
                return CacheGetResult.NotFound;
            }
            if (data.GetAll().Any(cached => cached.NeedUpdate))
            {
                entity = default(T);
                return CacheGetResult.Update;
            }
            entity = data.GetAll().FirstOrDefault(cached => whereFunc(cached));
            return CacheGetResult.Found;
        }

        public CacheGetResult GetFirstWhere(RelationInfo relations, Expression<Func<T, bool>> whereExpression, out T entity)
        {
            var whereFunc = whereExpression.CacheCompile();
            return GetFirstWhere(relations, whereFunc, out entity);
        }

        public CacheGetResult GetFirst(RelationInfo relations, out T entity)
        {
            var data = CacheStorage.GetCacheData(relations);
            if (!data.Empty && !data.FullCollection)
            {
                entity = default(T);
                return CacheGetResult.NotFound;
            }
            var first = data.GetAll().FirstOrDefault();
            if (first?.NeedUpdate ?? true)
            {
                entity = default(T);
                return CacheGetResult.Update;
            }
            entity = first;
            return CacheGetResult.Found;
        }

        public bool TryRemove(T entity, out List<T> removedList)
        {
            var pk = CacheStorage.GetPrimaryKeyValue(entity);
            var datas = CacheStorage.AllCacheDatas;
            var result = true;
            removedList = new List<T>(datas.Count);
            foreach (var data in datas)
            {
                CachedEntity<T> removed;
                result = result & data.TryRemove(pk, out removed);
                removedList.Add(removed);
            }
            return result;
        }

        public bool TryRemove(T entity)
        {
            var pk = CacheStorage.GetPrimaryKeyValue(entity);
            var datas = CacheStorage.AllCacheDatas;
            return datas.Aggregate(true, (current, data) => current & data.TryRemove(pk));
        }

        public void Update(IEnumerable<T> entities, RelationInfo relationInfo)
        {
            var data = CacheStorage.GetCacheData(relationInfo);
            data.Update(entities, relationInfo);
        }

        public void Update(T entity, RelationInfo relationInfo)
        {
            var data = CacheStorage.GetCacheData(relationInfo);
            data.Update(entity, relationInfo);
        }

        public CachedEntity<T> Update(RelationInfo relations, T entity)
        {
            var data = CacheStorage.GetCacheData(relations);
            return data.Update(entity, relations);
        }

        public IEnumerable<CachedEntity<T>> Update(RelationInfo relations, IEnumerable<T> entities)
        {
            var data = CacheStorage.GetCacheData(relations);
            return entities.Select(entity => data.Update(entity, relations));
        }

        public void UpdateAll(IEnumerable<T> entities, RelationInfo relationInfo)
        {
            var data = CacheStorage.GetCacheData(relationInfo);
            data.UpdateAll(entities, relationInfo);
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
    }
}