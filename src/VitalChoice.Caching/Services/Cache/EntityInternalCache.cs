using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
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

        public bool TryGetEntity(EntityPrimaryKeySearchInfo searchInfo, out T entity)
        {
            CachedEntity<T> cached;
            var data = CacheStorage.GetCacheData(searchInfo.RelationInfo);
            var result = data.EntityDictionary.TryGetValue(searchInfo.PrimaryKey, out cached);
            entity = cached;
            return result;
        }

        public bool TryGetEntities(EntityPrimaryKeySearchInfo[] searchInfos, out List<T> entities)
        {
            entities = new List<T>(searchInfos.Length);
            foreach (var key in searchInfos)
            {
                T cached;
                if (!TryGetEntity(key, out cached))
                {
                    return false;
                }
                entities.Add(cached);
            }
            return true;
        }

        public bool TryGetEntities(EntityPrimaryKey[] primaryKeys, RelationInfo relations, out List<T> entities)
        {
            entities = new List<T>(primaryKeys.Length);
            var data = CacheStorage.GetCacheData(relations);
            foreach (var key in primaryKeys)
            {
                CachedEntity<T> cached;
                if (!data.EntityDictionary.TryGetValue(key, out cached))
                {
                    return false;
                }
                entities.Add(cached);
            }
            return true;
        }

        public bool TryGetEntity(EntityUniqueIndexSearchInfo searchInfo, out T entity)
        {
            CachedEntity<T> cached;
            var data = CacheStorage.GetCacheData(searchInfo.RelationInfo);
            var result = data.IndexedDictionary[searchInfo.UniqueIndex.IndexInfo].TryGetValue(searchInfo.UniqueIndex, out cached);
            entity = cached;
            return result;
        }

        public bool TryGetEntities(EntityUniqueIndexSearchInfo[] searchInfos, out List<T> entities)
        {
            entities = new List<T>(searchInfos.Length);
            foreach (var key in searchInfos)
            {
                T cached;
                if (!TryGetEntity(key, out cached))
                {
                    return false;
                }
                entities.Add(cached);
            }
            return true;
        }

        public bool TryGetEntities(EntityUniqueIndex[] indexes, RelationInfo relations, out List<T> entities)
        {
            entities = new List<T>(indexes.Length);
            var data = CacheStorage.GetCacheData(relations);
            foreach (var index in indexes)
            {
                CachedEntity<T> cached;
                if (!data.IndexedDictionary[index.IndexInfo].TryGetValue(index, out cached))
                {
                    return false;
                }
                entities.Add(cached);
            }
            return true;
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

        public void Update(IEnumerable<T> entities, RelationInfo relationInfo)
        {
            var data = CacheStorage.GetCacheData(relationInfo);
            foreach (var entity in entities)
            {
                var pk = CacheStorage.GetPrimaryKeyValue(entity);
                var indexValues = CacheStorage.GetIndexValues(entity).ToArray();
                var cached = new CachedEntity<T>(entity, RelationProcessor.GetRelations(typeof (T), entity, relationInfo.Relations));
                data.EntityDictionary.AddOrUpdate(pk, cached, (key, _) => UpdateExist(_, cached));
                data.PrimaryToIndexes.AddOrUpdate(pk, indexValues, (key, _) => indexValues);

                foreach (var indexValue in indexValues)
                {
                    data.IndexedDictionary[indexValue.IndexInfo].AddOrUpdate(indexValue, cached, (index, _) => UpdateExist(_, cached, false));
                }
            }
        }

        public void Update(T entity, RelationInfo relationInfo)
        {
            var pk = CacheStorage.GetPrimaryKeyValue(entity);
            var indexValues = CacheStorage.GetIndexValues(entity).ToArray();
            var cached = new CachedEntity<T>(entity, RelationProcessor.GetRelations(typeof (T), entity, relationInfo.Relations));
            var data = CacheStorage.GetCacheData(relationInfo);
            data.EntityDictionary.AddOrUpdate(pk, cached, (key, _) => UpdateExist(_, cached));
            data.PrimaryToIndexes.AddOrUpdate(pk, indexValues, (key, _) => indexValues);

            foreach (var indexValue in indexValues)
            {
                data.IndexedDictionary[indexValue.IndexInfo].AddOrUpdate(indexValue, cached, (index, _) => UpdateExist(_, cached, false));
            }
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
                Update(entities, relationInfo);
            }
        }

        private CachedEntity<T> UpdateExist(CachedEntity<T> exist, CachedEntity<T> newEntity, bool updateEntireTree = true)
        {
            _typeConverter.CopyInto(exist.Entity, newEntity.Entity, typeof (T));
            exist.Relations = newEntity.Relations;

            if (!updateEntireTree)
                return exist;

            foreach (var relationInstance in exist.Relations)
            {
                var cache = CacheFactory.GetCache(relationInstance.RelationObjectType);
                cache.Update(relationInstance.RelatedObject, relationInstance.RelationInfo);
            }
            return exist;
        }

        public bool TryGetEntity(EntityPrimaryKeySearchInfo searchInfo, out object entity)
        {
            T resultEntity;
            var result = TryGetEntity(searchInfo, out resultEntity);
            entity = resultEntity;
            return result;
        }

        public bool TryGetEntities(EntityPrimaryKeySearchInfo[] searchInfos, out List<object> entities)
        {
            List<T> results;
            var result = TryGetEntities(searchInfos, out results);
            entities = results.Cast<object>().ToList();
            return result;
        }

        public bool TryGetEntities(EntityPrimaryKey[] primaryKeys, RelationInfo relations, out List<object> entities)
        {
            List<T> results;
            var result = TryGetEntities(primaryKeys, relations, out results);
            entities = results.Cast<object>().ToList();
            return result;
        }

        public bool TryGetEntity(EntityUniqueIndexSearchInfo uniqueIndex, out object entity)
        {
            T entityTyped;
            var result = TryGetEntity(uniqueIndex, out entityTyped);
            entity = entityTyped;
            return result;
        }

        public bool TryGetEntities(EntityUniqueIndexSearchInfo[] uniqueIndexes, out List<object> entities)
        {
            List<T> results;
            var result = TryGetEntities(uniqueIndexes, out results);
            entities = results.Cast<object>().ToList();
            return result;
        }

        public bool TryGetEntities(EntityUniqueIndex[] indexes, RelationInfo relations, out List<object> entities)
        {
            List<T> results;
            var result = TryGetEntities(indexes, relations, out results);
            entities = results.Cast<object>().ToList();
            return result;
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

        public bool GetCacheExist(RelationInfo relationInfo)
        {
            return CacheStorage.GetCacheExist(relationInfo);
        }
    }
}