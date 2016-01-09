using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using VitalChoice.Caching.Interfaces;
using VitalChoice.Caching.Relational;
using VitalChoice.Ecommerce.Domain;
using VitalChoice.Ecommerce.Domain.Helpers;
using VitalChoice.ObjectMapping.Interfaces;

namespace VitalChoice.Caching.Services.Cache
{
    public sealed class CacheData<T> : ICacheData<T>
    {
        private readonly IInternalEntityCacheFactory _cacheFactory;
        private readonly ITypeConverter _typeConverter;
        private readonly CacheStorage<T> _cacheStorage;

        private readonly object _lockObj = new object();

        private readonly ConcurrentDictionary<EntityKey, CachedEntity<T>> _entityDictionary =
            new ConcurrentDictionary<EntityKey, CachedEntity<T>>();

        private readonly ConcurrentDictionary<EntityIndex, CachedEntity<T>> _indexedDictionary = new ConcurrentDictionary<EntityIndex, CachedEntity<T>>();

        private readonly Dictionary<EntityConditionalIndexInfo, ConcurrentDictionary<EntityIndex, CachedEntity<T>>> _conditionalIndexedDictionary;

        public CacheData(IInternalEntityCacheFactory cacheFactory, ITypeConverter typeConverter, CacheStorage<T> cacheStorage, ICollection<EntityConditionalIndexInfo> conditionalIndexes)
        {
            _cacheFactory = cacheFactory;
            _typeConverter = typeConverter;
            _cacheStorage = cacheStorage;
            _conditionalIndexedDictionary = new Dictionary<EntityConditionalIndexInfo, ConcurrentDictionary<EntityIndex, CachedEntity<T>>>();
            foreach (var conditionalIndex in conditionalIndexes)
            {
                _conditionalIndexedDictionary.Add(conditionalIndex, new ConcurrentDictionary<EntityIndex, CachedEntity<T>>());
            }
        }

        public bool Get(EntityKey key, out CachedEntity<T> entity)
        {
            // ReSharper disable once InconsistentlySynchronizedField
            return _entityDictionary.TryGetValue(key, out entity);
        }

        public bool Get(EntityIndex key, out CachedEntity<T> entity)
        {
            // ReSharper disable once InconsistentlySynchronizedField
            return _indexedDictionary.TryGetValue(key, out entity);
        }

        public bool Get(EntityConditionalIndexInfo conditionalIndex, EntityIndex key, out CachedEntity<T> entity)
        {
            // ReSharper disable once InconsistentlySynchronizedField
            return _conditionalIndexedDictionary[conditionalIndex].TryGetValue(key, out entity);
        }

        public ICollection<CachedEntity<T>> GetAll()
        {
            return _entityDictionary.Values;
        }

        public bool TryRemove(EntityKey key, out CachedEntity<T> removed)
        {
            lock (_lockObj)
            {
                CachedEntity<T> temp;
                var result = _entityDictionary.TryRemove(key, out removed);
                if (result && removed.UniqueIndex != null)
                    result = _indexedDictionary.TryRemove(removed.UniqueIndex, out temp);
                if (result && removed.ConditionalIndexes != null)
                {
                    result = removed.ConditionalIndexes.Aggregate(true,
                        (current, conditionalIndex) =>
                            current && _conditionalIndexedDictionary[conditionalIndex.Key].TryRemove(conditionalIndex.Value, out temp));
                }
                return result;
            }
        }

        public bool TryRemove(EntityKey key)
        {
            lock (_lockObj)
            {
                CachedEntity<T> removed;
                CachedEntity<T> temp;
                var result = _entityDictionary.TryRemove(key, out removed);
                if (result)
                {
                    removed.NeedUpdate = true;
                }
                if (result && removed.UniqueIndex != null)
                    result = _indexedDictionary.TryRemove(removed.UniqueIndex, out temp);
                if (result && removed.ConditionalIndexes != null)
                {
                    result = removed.ConditionalIndexes.Aggregate(true,
                        (current, conditionalIndex) =>
                            current && _conditionalIndexedDictionary[conditionalIndex.Key].TryRemove(conditionalIndex.Value, out temp));
                }
                return result;
            }
        }

        public CachedEntity<T> Update(T entity, RelationInfo relations)
        {
            lock (_lockObj)
            {
                var pk = _cacheStorage.GetPrimaryKeyValue(entity);
                var indexValue = _cacheStorage.GetIndexValue(entity);
                var conditional =
                    _conditionalIndexedDictionary.Keys.Where(c => c.CheckCondition(entity))
                        .ToDictionary(c => c, c => _cacheStorage.GetConditionalIndexValue(entity, c));
                var result = _entityDictionary.AddOrUpdate(pk,
                    key => new CachedEntity<T>(entity, GetRelations(entity, relations.Relations), conditional, indexValue),
                    (key, _) => UpdateExist(_, entity, relations.Relations, indexValue, conditional));
                
                return result;
            }
        }

        public void Update(IEnumerable<T> entities, RelationInfo relations)
        {
            foreach (var entity in entities)
            {
                Update(entity, relations);
            }
        }

        public void UpdateAll(IEnumerable<T> entities, RelationInfo relations)
        {
            lock (_lockObj)
            {
                FullCollection = false;
                _entityDictionary.Clear();
                _indexedDictionary.Clear();
                foreach (var conditionalIndex in _conditionalIndexedDictionary)
                {
                    conditionalIndex.Value.Clear();
                }
                FullCollection = true;
            }
            Update(entities, relations);
        }

        public bool FullCollection { get; private set; }
        public bool Empty => _entityDictionary.IsEmpty;

        #region Helper Methods

        private static void UpdateRelations(CachedEntity exist, T newEntity, IEnumerable<RelationInfo> relationInfos)
        {
            if (newEntity == null)
                return;

            var keyedInstances = exist.Relations.ToDictionary(r => r.RelationInfo);

            foreach (var relation in relationInfos)
            {
                RelationInstance instance;

                if (!keyedInstances.TryGetValue(relation, out instance))
                    continue;

                var obj = relation.GetRelatedObject(newEntity);

                if (obj == null)
                    continue;

                var objType = obj.GetType();
                var elementType = objType.TryGetElementType(typeof(ICollection<>));
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

        private ICollection<RelationInstance> GetRelations(object entity, IEnumerable<RelationInfo> relationsInfo)
        {
            var result = new List<RelationInstance>();

            if (entity == null)
                return result;

            foreach (var relation in relationsInfo)
            {
                var obj = relation.GetRelatedObject(entity);

                if (obj == null)
                    continue;

                var objType = obj.GetType();
                var elementType = objType.TryGetElementType(typeof(ICollection<>));
                if (elementType != null)
                {
                    var cache = _cacheFactory.GetCache(elementType);

                    // ReSharper disable once AssignNullToNotNullAttribute
                    var cachedItems = cache.Update(relation, (obj as IEnumerable).Cast<object>());
                    result.AddRange(
                        cachedItems.Select(
                            cached =>
                                new RelationInstance(cache.Update(relation, cached.EntityUntyped),
                                    cached.EntityUntyped.GetType(), relation, cache)));
                }
                else
                {
                    var cache = _cacheFactory.GetCache(objType);
                    result.Add(new RelationInstance(cache.Update(relation, obj), objType, relation, cache));
                }
            }

            return result;
        }

        private CachedEntity<T> UpdateExist(CachedEntity<T> exist, T newEntity, IEnumerable<RelationInfo> newRelations,
            EntityIndex indexValue, Dictionary<EntityConditionalIndexInfo, EntityIndex> conditional)
        {
            lock (exist)
            {
                if (exist.NeedUpdate)
                {
                    if (exist.UniqueIndex != null && indexValue != exist.UniqueIndex)
                    {
                        CachedEntity<T> temp;
                        _indexedDictionary.TryRemove(exist.UniqueIndex, out temp);
                    }
                    if (indexValue != null)
                        _indexedDictionary.AddOrUpdate(indexValue, exist, (index, _) => exist);

                    foreach (var conditionalIndex in conditional)
                    {
                        _conditionalIndexedDictionary[conditionalIndex.Key].AddOrUpdate(conditionalIndex.Value, exist, (index, _) => exist);
                    }
                    var removeList = _conditionalIndexedDictionary.Keys.Where(key => !conditional.ContainsKey(key)).ToArray();
                    foreach (var indexInfo in removeList)
                    {
                        CachedEntity<T> temp;
                        _conditionalIndexedDictionary[indexInfo].TryRemove(_cacheStorage.GetConditionalIndexValue(exist, indexInfo), out temp);
                    }
                    exist.UniqueIndex = indexValue;
                    exist.ConditionalIndexes = conditional;
                    exist.NeedUpdate = false;
                    _typeConverter.CopyInto(exist.ValueInternal, newEntity, typeof (T));
                    UpdateRelations(exist, newEntity, newRelations);
                }
            }
            return exist;
        }

        #endregion
    }
}