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
    internal class EntityInternalCache<T> : IInternalEntityCache<T> where T : Entity
    {
        protected readonly IInternalEntityInfoStorage KeyStorage;
        protected readonly IInternalEntityCacheFactory CacheFactory;
        private readonly ITypeConverter _typeConverter;

        protected readonly ConcurrentDictionary<EntityPrimaryKey, CachedEntity> EntityDictionary = new ConcurrentDictionary<EntityPrimaryKey, CachedEntity>();
        protected readonly ConcurrentDictionary<EntityPrimaryKey, EntityUniqueIndex[]> PrimaryToIndexes = new ConcurrentDictionary<EntityPrimaryKey, EntityUniqueIndex[]>();
        protected readonly Dictionary<EntityUniqueIndexInfo, ConcurrentDictionary<EntityUniqueIndex, CachedEntity>> IndexedDictionary;

        public EntityInternalCache(IInternalEntityInfoStorage keyStorage, IInternalEntityCacheFactory cacheFactory, ITypeConverter typeConverter)
        {
            KeyStorage = keyStorage;
            CacheFactory = cacheFactory;
            _typeConverter = typeConverter;
            IndexedDictionary = new Dictionary<EntityUniqueIndexInfo, ConcurrentDictionary<EntityUniqueIndex, CachedEntity>>();
            foreach (var indexInfo in keyStorage.GetIndexInfos<T>())
            {
                IndexedDictionary.Add(indexInfo, new ConcurrentDictionary<EntityUniqueIndex, CachedEntity>());
            }
        }

        public bool TryGetEntity(EntityPrimaryKey primaryKey, out T entity)
        {
            CachedEntity cached;
            var result = EntityDictionary.TryGetValue(primaryKey, out cached);
            entity = cached;
            return result;
        }

        public bool TryGetEntities(EntityPrimaryKey[] primaryKeys, out List<T> entities)
        {
            entities = new List<T>(primaryKeys.Length);
            foreach (var key in primaryKeys)
            {
                CachedEntity cached;
                if (!EntityDictionary.TryGetValue(key, out cached))
                {
                    return false;
                }
                entities.Add(cached);
            }
            return true;
        }

        public bool TryGetEntity(EntityUniqueIndex uniqueIndex, out T entity)
        {
            CachedEntity cached;
            var result = IndexedDictionary[uniqueIndex.IndexInfo].TryGetValue(uniqueIndex, out cached);
            entity = cached;
            return result;
        }

        public bool TryGetEntities(EntityUniqueIndex[] uniqueIndexes, out List<T> entities)
        {
            entities = new List<T>(uniqueIndexes.Length);
            foreach (var index in uniqueIndexes)
            {
                CachedEntity cached;
                if (!IndexedDictionary[index.IndexInfo].TryGetValue(index, out cached))
                {
                    return false;
                }
                entities.Add(cached);
            }
            return true;
        }

        public List<T> GetWhere(Func<T, bool> whereFunc)
        {
            return EntityDictionary.Values.Where(cached => whereFunc(cached)).Select(cached => cached.Entity).ToList();
        }

        public List<T> GetWhere(Expression<Func<T, bool>> whereExpression)
        {
            var whereFunc = whereExpression.CacheCompile();
            return GetWhere(whereFunc);
        }

        public List<T> GetAll()
        {
            return EntityDictionary.Values.Select(cached => cached.Entity).ToList();
        }

        public bool TryRemove(T entity)
        {
            CachedEntity removed;
            EntityUniqueIndex[] indexValues;

            var pk = KeyStorage.GetPrimaryKeyValue(entity);
            var result = EntityDictionary.TryRemove(pk, out removed);
            if (result && PrimaryToIndexes.TryGetValue(pk, out indexValues))
            {
                result = indexValues.Aggregate(true,
                    (current, indexValue) => current & IndexedDictionary[indexValue.IndexInfo].TryRemove(indexValue, out removed));
            }
            return result;
        }

        public bool TryRemove(IEnumerable<T> entities)
        {
            return entities.Aggregate(true, (current, entity) => current && TryRemove(entity));
        }

        public bool TryRemoveTree(T entity)
        {
            throw new NotImplementedException();
        }

        public bool TryRemoveTree(IEnumerable<T> entities)
        {
            throw new NotImplementedException();
        }

        public void Update(IEnumerable<T> entities, ICollection<RelationInfo> relations)
        {
            foreach (var entity in entities)
            {
                Update(entity, relations);
            }
        }

        public void Update(T entity, ICollection<RelationInfo> relations)
        {
            var pk = KeyStorage.GetPrimaryKeyValue(entity);
            var indexValues = KeyStorage.GetIndexValues(entity).ToArray();
            var cached = new CachedEntity(entity, RelationProcessor.GetRelations(typeof(T), entity, relations), relations);

            EntityDictionary.AddOrUpdate(pk, cached, (key, _) => UpdateExist(_, cached));
            PrimaryToIndexes.AddOrUpdate(pk, indexValues, (key, _) => indexValues);
            foreach (var indexValue in indexValues)
            {
                IndexedDictionary[indexValue.IndexInfo].AddOrUpdate(indexValue, cached, (index, _) => UpdateExist(_, cached));
            }
        }

        private CachedEntity UpdateExist(CachedEntity exist, CachedEntity newEntity)
        {
            if (exist.RelationsInfo <= newEntity.RelationsInfo)
            {
                _typeConverter.CopyInto(exist.Entity, newEntity.Entity, typeof (T));
                exist.Relations = newEntity.Relations;
                exist.RelationsInfo = newEntity.RelationsInfo;
            }
            else
            {
                throw new InvalidOperationException("Cache update bucket is smaller than cached entity");
            }
            return exist;
        }

        public bool TryGetEntity(EntityPrimaryKey primaryKey, out object entity)
        {
            T resultEntity;
            var result = TryGetEntity(primaryKey, out resultEntity);
            entity = resultEntity;
            return result;
        }

        public bool TryGetEntities(EntityPrimaryKey[] primaryKeys, out List<object> entity)
        {
            throw new NotImplementedException();
        }

        public bool TryGetEntity(EntityUniqueIndex uniqueIndex, out object entity)
        {
            throw new NotImplementedException();
        }

        public bool TryGetEntities(EntityUniqueIndex[] uniqueIndexes, out List<object> entity)
        {
            throw new NotImplementedException();
        }

        public bool TryRemove(object entity)
        {
            return TryRemove((T) entity);
        }

        public bool TryRemove(IEnumerable<object> entities)
        {
            return TryRemove(entities.Cast<T>());
        }

        public bool TryRemoveTree(object entity)
        {
            return TryRemoveTree((T) entity);
        }

        public bool TryRemoveTree(IEnumerable<object> entities)
        {
            return TryRemoveTree(entities.Cast<T>());
        }

        public void Update(IEnumerable<object> entities, ICollection<RelationInfo> relations)
        {
            Update(entities.Cast<T>(), relations);
        }

        public void Update(object entity, ICollection<RelationInfo> relations)
        {
            Update((T) entity, relations);
        }

        public bool Empty => EntityDictionary.Count == 0;

        protected class CachedEntity
        {
            public CachedEntity(T entity, ICollection<RelationInstance> relations, ICollection<RelationInfo> relationsInfo)
            {
                Relations = relations;
                RelationsInfo = new RelationInfo(string.Empty, typeof (T), typeof (T))
                {
                    RelationsDict = relationsInfo.ToDictionary(r => new RelationCacheInfo(r.Name, r.RelationEntityType, r.ParentEntityType))
                };
                Entity = entity;
            }

            public T Entity { get; set; }
            public ICollection<RelationInstance> Relations { get; set; }
            public RelationInfo RelationsInfo { get; set; }

            public static implicit operator T(CachedEntity cached)
            {
                return cached.Entity;
            }
        }
    }
}