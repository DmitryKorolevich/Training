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

        public CacheGetResult TryGetEntities(ICollection<EntityPrimaryKeySearchInfo> searchInfos, Expression<Func<T, bool>> whereExpression,
            out List<T> entities)
        {
            entities = new List<T>(searchInfos.Count);
            var whereFunc = whereExpression?.CacheCompile();
            foreach (var key in searchInfos)
            {
                T cached;
                var getResult = TryGetEntity(key, out cached);
                if (getResult != CacheGetResult.Found)
                {
                    return getResult;
                }
                if (whereFunc?.Invoke(cached) ?? true)
                    entities.Add(cached);
            }
            return CacheGetResult.Found;
        }

        public CacheGetResult TryGetEntities(ICollection<EntityPrimaryKey> primaryKeys, RelationInfo relations, Expression<Func<T, bool>> whereExpression, out List<T> entities)
        {
            entities = new List<T>(primaryKeys.Count);
            var data = CacheStorage.GetCacheData(relations);
            var whereFunc = whereExpression?.CacheCompile();
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
                if (whereFunc?.Invoke(cached) ?? true)
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

        public CacheGetResult TryGetEntities(ICollection<EntityUniqueIndexSearchInfo> searchInfos, Expression<Func<T, bool>> whereExpression, out List<T> entities)
        {
            entities = new List<T>(searchInfos.Count);
            var whereFunc = whereExpression?.CacheCompile();
            foreach (var key in searchInfos)
            {
                T cached;
                var getResult = TryGetEntity(key, out cached);
                if (getResult != CacheGetResult.Found)
                {
                    return getResult;
                }
                if (whereFunc?.Invoke(cached) ?? true)
                    entities.Add(cached);
            }
            return CacheGetResult.Found;
        }

        public CacheGetResult TryGetEntities(ICollection<EntityUniqueIndex> indexes, RelationInfo relations, Expression<Func<T, bool>> whereExpression, out List<T> entities)
        {
            entities = new List<T>(indexes.Count);
            var data = CacheStorage.GetCacheData(relations);
            var whereFunc = whereExpression?.CacheCompile();
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
                if (whereFunc?.Invoke(cached) ?? true)
                    entities.Add(cached);
            }
            return CacheGetResult.Found;
        }

        public CacheGetResult GetWhere(RelationInfo relations, Func<T, bool> whereFunc, out List<T> entities)
        {
            var data = CacheStorage.GetCacheData(relations);
            if (!data.FullCollection)
            {
                entities = null;
                return CacheGetResult.NotFound;
            }
            if (data.EntityDictionary.Values.Any(cached => cached.NeedUpdate))
            {
                entities = null;
                return CacheGetResult.Update;
            }
            entities = data.EntityDictionary.Values.Where(cached => whereFunc(cached)).Select(cached => cached.Entity).ToList();
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
            if (!data.FullCollection)
            {
                entities = null;
                return CacheGetResult.NotFound;
            }
            if (data.EntityDictionary.Values.Any(cached => cached.NeedUpdate))
            {
                entities = null;
                return CacheGetResult.Update;
            }
            entities = data.EntityDictionary.Values.Select(cached => cached.Entity).ToList();
            return CacheGetResult.Found;
        }

        public CacheGetResult GetFirstWhere(RelationInfo relations, Func<T, bool> whereFunc, out T entity)
        {
            var data = CacheStorage.GetCacheData(relations);
            if (!data.FullCollection)
            {
                entity = default(T);
                return CacheGetResult.NotFound;
            }
            if (data.EntityDictionary.Values.Any(cached => cached.NeedUpdate))
            {
                entity = default(T);
                return CacheGetResult.Update;
            }
            entity = data.EntityDictionary.Values.FirstOrDefault(cached => whereFunc(cached));
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
            if (!data.FullCollection)
            {
                entity = default(T);
                return CacheGetResult.NotFound;
            }
            var first = data.EntityDictionary.Values.FirstOrDefault();
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
                removed.NeedUpdate = true;
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
                data.FullCollection = false;
                data.EntityDictionary.Clear();
                data.PrimaryToIndexes.Clear();
                foreach (var indexed in data.IndexedDictionary.Values)
                {
                    indexed.Clear();
                }
                Update(entities, relationInfo);
                data.FullCollection = true;
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

        //public CacheGetResult TryGetEntity(EntityPrimaryKeySearchInfo searchInfo, out object entity)
        //{
        //    T resultEntity;
        //    var result = TryGetEntity(searchInfo, out resultEntity);
        //    entity = resultEntity;
        //    return result;
        //}

        //public CacheGetResult TryGetEntities(ICollection<EntityPrimaryKeySearchInfo> searchInfos, out List<object> entities)
        //{
        //    List<T> results;
        //    var result = TryGetEntities(searchInfos, null, out results);
        //    entities = results.Cast<object>().ToList();
        //    return result;
        //}

        //public CacheGetResult TryGetEntities(ICollection<EntityPrimaryKey> primaryKeys, RelationInfo relations, out List<object> entities)
        //{
        //    List<T> results;
        //    var result = TryGetEntities(primaryKeys, relations, null, out results);
        //    entities = results.Cast<object>().ToList();
        //    return result;
        //}

        //public CacheGetResult TryGetEntity(EntityUniqueIndexSearchInfo uniqueIndex, out object entity)
        //{
        //    T entityTyped;
        //    var result = TryGetEntity(uniqueIndex, out entityTyped);
        //    entity = entityTyped;
        //    return result;
        //}

        //public CacheGetResult TryGetEntities(ICollection<EntityUniqueIndexSearchInfo> uniqueIndexes, out List<object> entities)
        //{
        //    List<T> results;
        //    var result = TryGetEntities(uniqueIndexes, null, out results);
        //    entities = results.Cast<object>().ToList();
        //    return result;
        //}

        //public CacheGetResult TryGetEntities(ICollection<EntityUniqueIndex> indexes, RelationInfo relations, out List<object> entities)
        //{
        //    List<T> results;
        //    var result = TryGetEntities(indexes, relations, null, out results);
        //    entities = results.Cast<object>().ToList();
        //    return result;
        //}

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
                data.FullCollection = false;
                data.EntityDictionary.Clear();
                data.PrimaryToIndexes.Clear();
                foreach (var indexed in data.IndexedDictionary.Values)
                {
                    indexed.Clear();
                }
                Update(entities, relationInfo);
                data.FullCollection = true;
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

        public bool GetIsCacheFullCollection(RelationInfo relationInfo)
        {
            return CacheStorage.GetIsCacheFullCollection(relationInfo);
        }

        public CachedEntity<T> UpdateCache(T entity, RelationInfo relations, CacheData<T> data = null)
        {
            if (data == null)
                data = CacheStorage.GetCacheData(relations);
            var pk = CacheStorage.GetPrimaryKeyValue(entity);
            var indexValues = CacheStorage.GetIndexValues(entity).ToArray();
            var result = data.EntityDictionary.AddOrUpdate(pk, key => new CachedEntity<T>(entity, GetRelations(entity, relations.Relations)),
                (key, _) => UpdateExist(_, entity, relations.Relations));
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
                var pk = CacheStorage.GetPrimaryKeyValue(entity);
                var indexValues = CacheStorage.GetIndexValues(entity).ToArray();
                var result = data.EntityDictionary.AddOrUpdate(pk, key => new CachedEntity<T>(entity, GetRelations(entity, relations.Relations)),
                    (key, _) => UpdateExist(_, entity, relations.Relations));
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

                    // ReSharper disable once AssignNullToNotNullAttribute
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