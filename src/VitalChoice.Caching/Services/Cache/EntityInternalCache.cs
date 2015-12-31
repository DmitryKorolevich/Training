using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using Microsoft.Data.Entity.Internal;
using VitalChoice.Caching.Expressions;
using VitalChoice.Caching.Interfaces;
using VitalChoice.Caching.Relational;
using VitalChoice.Ecommerce.Domain;
using VitalChoice.Ecommerce.Domain.Helpers;
using VitalChoice.ObjectMapping.Base;
using VitalChoice.ObjectMapping.Interfaces;

namespace VitalChoice.Caching.Services.Cache
{
    public class EntityInternalCache<T> : IInternalEntityCache<T>
    {
        protected readonly IInternalEntityInfoStorage KeyStorage;
        protected readonly IInternalEntityCacheFactory CacheFactory;
        private readonly ITypeConverter _typeConverter;

        protected readonly CacheStorage<T> CacheStorage;

        public EntityInternalCache(IInternalEntityInfoStorage keyStorage, IInternalEntityCacheFactory cacheFactory,
            ITypeConverter typeConverter)
        {
            KeyStorage = keyStorage;
            CacheFactory = cacheFactory;
            CacheStorage = new CacheStorage<T>(keyStorage);
            _typeConverter = typeConverter;
        }

        public CacheGetResult TryGetEntity(EntityPrimaryKeySearchInfo searchInfo, out T entity)
        {
            CachedEntity<T> cached;
            var data = CacheStorage.GetCacheData(searchInfo.RelationInfo);
            if (data.EntityDictionary.TryGetValue(searchInfo.PrimaryKey, out cached))
            {
                entity = cached;
                return cached.NeedUpdate ? CacheGetResult.Update : CacheGetResult.Found;
            }
            entity = default(T);
            return CacheGetResult.NotFound;
        }

        public CacheGetResult TryGetEntities(EntityPrimaryKeySearchInfo[] searchInfos, out List<T> entities)
        {
            entities = new List<T>(searchInfos.Length);
            foreach (var key in searchInfos)
            {
                T cached;
                var getResult = TryGetEntity(key, out cached);
                if (getResult != CacheGetResult.Found)
                {
                    return getResult;
                }
                entities.Add(cached);
            }
            return CacheGetResult.Found;
        }

        public CacheGetResult TryGetEntities(EntityPrimaryKey[] primaryKeys, RelationInfo relations, out List<T> entities)
        {
            entities = new List<T>(primaryKeys.Length);
            var data = CacheStorage.GetCacheData(relations);
            foreach (var key in primaryKeys)
            {
                CachedEntity<T> cached;
                if (data.EntityDictionary.TryGetValue(key, out cached))
                {
                    if (cached.NeedUpdate)
                        return CacheGetResult.Update;
                }
                else
                {
                    return CacheGetResult.NotFound;
                }
                entities.Add(cached);
            }
            return CacheGetResult.Found;
        }

        public CacheGetResult TryGetEntity(EntityUniqueIndexSearchInfo searchInfo, out T entity)
        {
            CachedEntity<T> cached;
            var data = CacheStorage.GetCacheData(searchInfo.RelationInfo);
            if (data.IndexedDictionary[searchInfo.UniqueIndex.IndexInfo].TryGetValue(searchInfo.UniqueIndex, out cached))
            {
                entity = cached;
                return cached.NeedUpdate ? CacheGetResult.Update : CacheGetResult.Found;
            }
            entity = default(T);
            return CacheGetResult.NotFound;
        }

        public CacheGetResult TryGetEntities(EntityUniqueIndexSearchInfo[] searchInfos, out List<T> entities)
        {
            entities = new List<T>(searchInfos.Length);
            foreach (var key in searchInfos)
            {
                T cached;
                var getResult = TryGetEntity(key, out cached);
                if (getResult != CacheGetResult.Found)
                {
                    return getResult;
                }
                entities.Add(cached);
            }
            return CacheGetResult.Found;
        }

        public CacheGetResult TryGetEntities(EntityUniqueIndex[] indexes, RelationInfo relations, out List<T> entities)
        {
            entities = new List<T>(indexes.Length);
            var data = CacheStorage.GetCacheData(relations);
            foreach (var index in indexes)
            {
                CachedEntity<T> cached;
                if (data.IndexedDictionary[index.IndexInfo].TryGetValue(index, out cached))
                {
                    if (cached.NeedUpdate)
                        return CacheGetResult.Update;
                }
                else
                {
                    return CacheGetResult.NotFound;
                }
                entities.Add(cached);
            }
            return CacheGetResult.Found;
        }

        public IEnumerable<T> GetWhere(RelationInfo relations, Func<T, bool> whereFunc)
        {
            var data = CacheStorage.GetCacheData(relations);
            return data.EntityDictionary.Values.Where(cached => whereFunc(cached)).Select(cached => cached.Entity);
        }

        public IEnumerable<T> GetWhere(RelationInfo relations, Expression<Func<T, bool>> whereExpression)
        {
            var whereFunc = whereExpression.CacheCompile();
            return GetWhere(relations, whereFunc);
        }

        public IEnumerable<T> GetAll(RelationInfo relations)
        {
            var data = CacheStorage.GetCacheData(relations);
            return data.EntityDictionary.Values.Select(cached => cached.Entity);
        }

        public bool TryRemove(T entity, out List<T> removedList)
        {
            CachedEntity<T> removed;
            var pk = CacheStorage.GetPrimaryKeyValue(entity);
            var datas = CacheStorage.AllCacheDatas;
            var result = true;
            removedList = new List<T>(datas.Count);
            foreach (var data in datas)
            {
                result = result & data.EntityDictionary.TryRemove(pk, out removed);
                removedList.Add(removed);
                EntityUniqueIndex[] indexValues;
                if (result && data.PrimaryToIndexes.TryGetValue(pk, out indexValues))
                {
                    result = indexValues.Aggregate(true,
                        (current, indexValue) => current & data.IndexedDictionary[indexValue.IndexInfo].TryRemove(indexValue, out removed));
                }
            }
            return result;
        }

        public bool TryRemove(T entity)
        {
            CachedEntity<T> removed;
            var pk = CacheStorage.GetPrimaryKeyValue(entity);
            var datas = CacheStorage.AllCacheDatas;
            var result = true;
            foreach (var data in datas)
            {
                result = result & data.EntityDictionary.TryRemove(pk, out removed);
                EntityUniqueIndex[] indexValues;
                if (result && data.PrimaryToIndexes.TryGetValue(pk, out indexValues))
                {
                    result = indexValues.Aggregate(true,
                        (current, indexValue) => current & data.IndexedDictionary[indexValue.IndexInfo].TryRemove(indexValue, out removed));
                }
            }
            return result;
        }

        public void Update(IEnumerable<T> entities, RelationInfo relationInfo)
        {
            UpdateCache(entities, relationInfo);
        }

        public void Update(T entity, RelationInfo relationInfo)
        {
            UpdateCache(entity, relationInfo);
        }

        public void UpdateAll(IEnumerable<T> entities, RelationInfo relationInfo)
        {
            var data = CacheStorage.GetCacheData(relationInfo);
            lock (data)
            {
                data.EntityDictionary.Clear();
                data.PrimaryToIndexes.Clear();
                foreach (var indexed in data.IndexedDictionary.Values)
                {
                    indexed.Clear();
                }
                data.FullCollection = true;
                Update(entities, relationInfo);
            }
        }

        public void MarkForUpdate(T entity)
        {
            var pk = CacheStorage.GetPrimaryKeyValue(entity);
            foreach (var data in CacheStorage.AllCacheDatas)
            {
                CachedEntity<T> cached;
                if (data.EntityDictionary.TryGetValue(pk, out cached))
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

        public CacheGetResult TryGetEntity(EntityPrimaryKeySearchInfo searchInfo, out object entity)
        {
            T resultEntity;
            var result = TryGetEntity(searchInfo, out resultEntity);
            entity = resultEntity;
            return result;
        }

        public CacheGetResult TryGetEntities(EntityPrimaryKeySearchInfo[] searchInfos, out List<object> entities)
        {
            List<T> results;
            var result = TryGetEntities(searchInfos, out results);
            entities = results.Cast<object>().ToList();
            return result;
        }

        public CacheGetResult TryGetEntities(EntityPrimaryKey[] primaryKeys, RelationInfo relations, out List<object> entities)
        {
            List<T> results;
            var result = TryGetEntities(primaryKeys, relations, out results);
            entities = results.Cast<object>().ToList();
            return result;
        }

        public CacheGetResult TryGetEntity(EntityUniqueIndexSearchInfo uniqueIndex, out object entity)
        {
            T entityTyped;
            var result = TryGetEntity(uniqueIndex, out entityTyped);
            entity = entityTyped;
            return result;
        }

        public CacheGetResult TryGetEntities(EntityUniqueIndexSearchInfo[] uniqueIndexes, out List<object> entities)
        {
            List<T> results;
            var result = TryGetEntities(uniqueIndexes, out results);
            entities = results.Cast<object>().ToList();
            return result;
        }

        public CacheGetResult TryGetEntities(EntityUniqueIndex[] indexes, RelationInfo relations, out List<object> entities)
        {
            List<T> results;
            var result = TryGetEntities(indexes, relations, out results);
            entities = results.Cast<object>().ToList();
            return result;
        }

        public bool TryRemove(object entity, out List<object> removedList)
        {
            List<T> removed;
            var result = TryRemove((T) entity, out removed);
            removedList = removed.Cast<object>().ToList();
            return result;
        }

        public bool TryRemove(object entity)
        {
            return TryRemove((T) entity);
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
            var data = CacheStorage.GetCacheData(relationInfo);
            lock (data)
            {
                data.EntityDictionary.Clear();
                data.PrimaryToIndexes.Clear();
                foreach (var indexed in data.IndexedDictionary.Values)
                {
                    indexed.Clear();
                }
                Update(entities, relationInfo);
            }
        }

        public void MarkForUpdate(object entity)
        {
            MarkForUpdate((T) entity);
        }

        public void MarkForUpdate(IEnumerable<object> entities)
        {
            MarkForUpdate(entities.Cast<T>());
        }

        public ICollection<CachedEntity> UpdateCache(IEnumerable<object> entities, RelationInfo relations)
        {
            return UpdateCache(entities.Cast<T>(), relations).Cast<CachedEntity>().ToArray();
        }

        public bool GetCacheExist(RelationInfo relationInfo)
        {
            return CacheStorage.GetCacheExist(relationInfo);
        }

        public CachedEntity<T> UpdateCache(T entity, RelationInfo relations, CacheData<T> data = null)
        {
            if (data == null)
                data = CacheStorage.GetCacheData(relations);
            CachedEntity<T> result = null;
            var pk = CacheStorage.GetPrimaryKeyValue(entity);
            var indexValues = CacheStorage.GetIndexValues(entity).ToArray();
            data.EntityDictionary.AddOrUpdate(pk, key => result = new CachedEntity<T>(entity, GetRelations(entity, relations.Relations)),
                (key, _) => result = UpdateExist(_, result, relations.Relations));
            data.PrimaryToIndexes.AddOrUpdate(pk, indexValues, (key, _) => indexValues);

            foreach (var indexValue in indexValues)
            {
                data.IndexedDictionary[indexValue.IndexInfo].AddOrUpdate(indexValue, result, (index, _) => result);
            }
            return result;
        }

        public ICollection<CachedEntity<T>> UpdateCache(IEnumerable<T> entities, RelationInfo relations, CacheData<T> data = null)
        {
            List<CachedEntity<T>> results = new List<CachedEntity<T>>();
            if (data == null)
                data = CacheStorage.GetCacheData(relations);
            foreach (var entity in entities)
            {
                CachedEntity<T> result = null;
                var pk = CacheStorage.GetPrimaryKeyValue(entity);
                var indexValues = CacheStorage.GetIndexValues(entity).ToArray();
                data.EntityDictionary.AddOrUpdate(pk, key => result = new CachedEntity<T>(entity, GetRelations(entity, relations.Relations)),
                    (key, _) => result = UpdateExist(_, result, relations.Relations));
                data.PrimaryToIndexes.AddOrUpdate(pk, indexValues, (key, _) => indexValues);

                foreach (var indexValue in indexValues)
                {
                    data.IndexedDictionary[indexValue.IndexInfo].AddOrUpdate(indexValue, result, (index, _) => result);
                }
                results.Add(result);
            }
            return results;
        }

        public CachedEntity UpdateCache(object entity, RelationInfo relations)
        {
            return UpdateCache((T) entity, relations);
        }

        private void UpdateRelations(CachedEntity<T> exist, T newEntity, ICollection<RelationInfo> relationInfos)
        {
            if (newEntity == null)
                return;

            var keyedInstances = exist.Relations.ToDictionary(r => r.RelationInfo);

            foreach (var relation in relationInfos)
            {
                RelationInstance instance;

                if (!keyedInstances.TryGetValue(relation, out instance))
                    continue;

                var obj = RelationProcessor.GetRelatedObject(relation.OwnedType, relation.Name, newEntity);

                if (obj == null)
                    continue;

                var objType = obj.GetType();
                var elementType = objType.TryGetElementType(typeof (ICollection<>));
                if (elementType != null)
                {
                    // ReSharper disable once PossibleNullReferenceException
                    // ReSharper disable once LoopCanBeConvertedToQuery
                    foreach (var item in obj as IEnumerable)
                    {
                        instance.CacheContainer.Update(item, relation);
                    }
                }
                else
                {
                    instance.CacheContainer.Update(obj, relation);
                }
            }
        }

        private ICollection<RelationInstance> GetRelations(object entity, ICollection<RelationInfo> relationsInfo)
        {
            var result = new List<RelationInstance>();

            if (entity == null)
                return result;

            foreach (var relation in relationsInfo)
            {
                var obj = RelationProcessor.GetRelatedObject(relation.OwnedType, relation.Name, entity);

                if (obj == null)
                    continue;

                var objType = obj.GetType();
                var elementType = objType.TryGetElementType(typeof (ICollection<>));
                if (elementType != null)
                {
                    var cache = CacheFactory.GetCache(elementType);
                    // ReSharper disable once PossibleNullReferenceException
                    // ReSharper disable once LoopCanBeConvertedToQuery
                    var cachedItems = cache.UpdateCache((obj as IEnumerable).Cast<object>(), relation);
                    result.AddRange(
                        cachedItems.Select(
                            cached =>
                                new RelationInstance(cache.UpdateCache(cached, relation), cached.EntityUntyped.GetType(), relation, cache)));
                }
                else
                {
                    var cache = CacheFactory.GetCache(objType);
                    result.Add(new RelationInstance(cache.UpdateCache(obj, relation), objType, relation, cache));
                }
            }

            return result;
        }

        private CachedEntity<T> UpdateExist(CachedEntity<T> exist, T newEntity, ICollection<RelationInfo> newRelations)
        {
            lock (exist)
            {
                if (exist.NeedUpdate)
                {
                    exist.NeedUpdate = false;
                    _typeConverter.CopyInto(exist.Entity, newEntity, typeof (T));
                    UpdateRelations(exist, newEntity, newRelations);
                }
            }
            return exist;
        }
    }
}