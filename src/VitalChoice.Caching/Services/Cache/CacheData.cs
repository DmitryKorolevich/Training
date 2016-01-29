using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using VitalChoice.Caching.Interfaces;
using VitalChoice.Caching.Relational;
using VitalChoice.Caching.Services.Cache.Base;
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
        private readonly RelationInfo _relationInfo;

        private readonly object _lockObj = new object();

        private readonly ConcurrentDictionary<EntityKey, CachedEntity<T>> _entityDictionary =
            new ConcurrentDictionary<EntityKey, CachedEntity<T>>();

        private readonly ConcurrentDictionary<EntityIndex, CachedEntity<T>> _indexedDictionary =
            new ConcurrentDictionary<EntityIndex, CachedEntity<T>>();

        private readonly Dictionary<EntityConditionalIndexInfo, ConcurrentDictionary<EntityIndex, CachedEntity<T>>>
            _conditionalIndexedDictionary;

        public CacheData(IInternalEntityCacheFactory cacheFactory, ITypeConverter typeConverter, CacheStorage<T> cacheStorage,
            ICollection<EntityConditionalIndexInfo> conditionalIndexes, RelationInfo relationInfo)
        {
            _cacheFactory = cacheFactory;
            _typeConverter = typeConverter;
            _cacheStorage = cacheStorage;
            _relationInfo = relationInfo;
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

        public void Clear()
        {
            lock (_lockObj)
            {
                NeedUpdate = true;
                _entityDictionary.Clear();
                _indexedDictionary.Clear();
                foreach (var conditionalIndex in _conditionalIndexedDictionary)
                {
                    conditionalIndex.Value.Clear();
                }
            }
        }

        public IEnumerable<CachedEntity> GetAllUntyped()
        {
            return _entityDictionary.Values;
        }

        public bool TryRemove(EntityKey key, out CachedEntity<T> removed)
        {
            lock (_lockObj)
            {
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

        public bool TryRemove(EntityKey key)
        {
            CachedEntity<T> removed;
            return TryRemove(key, out removed);
        }

        public CachedEntity<T> Update(T entity, bool ignoreState = false)
        {
            if (entity == null)
                return null;
            lock (_lockObj)
            {
                var pk = _cacheStorage.GetPrimaryKeyValue(entity);
                var indexValue = _cacheStorage.GetIndexValue(entity);
                var conditional =
                    _conditionalIndexedDictionary.Keys.Where(c => c.CheckCondition(entity))
                        .ToDictionary(c => c, c => _cacheStorage.GetConditionalIndexValue(entity, c));
                
                var result = _entityDictionary.AddOrUpdate(pk,
                    key => CreateNew(entity, conditional, indexValue),
                    (key, _) => UpdateExist(_, entity, _relationInfo.Relations, indexValue, conditional, ignoreState));

                return result;
            }
        }

        private CachedEntity<T> CreateNew(T entity, Dictionary<EntityConditionalIndexInfo, EntityIndex> conditional, EntityIndex indexValue)
        {
            CachedEntity<T> cached;
            if (entity == null)
            {
                cached = new CachedEntity<T>(default(T), null, null, this);
            }
            else
            {
                cached = new CachedEntity<T>(entity, GetRelations(entity, _relationInfo.Relations), conditional, this, indexValue);
                foreach (var conditionalIndex in conditional)
                {
                    _conditionalIndexedDictionary[conditionalIndex.Key].AddOrUpdate(conditionalIndex.Value, cached,
                        (index, cachedEntity) => cached);
                }
            }
            return cached;
        }

        public void Update(IEnumerable<T> entities)
        {
            foreach (var entity in entities)
            {
                Update(entity);
            }
        }

        public void UpdateAll(IEnumerable<T> entities)
        {
            Clear();
            Update(entities);
            FullCollection = true;
        }

        public void SetNull(EntityKey pk)
        {
            if (pk == null)
                return;
            lock (_lockObj)
            {
                _entityDictionary.AddOrUpdate(pk,
                    key => CreateNew(default(T), null, null),
                    (key, _) => UpdateExist(_, default(T), _relationInfo.Relations, null, null));
            }
        }

        public void SetNull(IEnumerable<EntityKey> keys)
        {
            foreach (var key in keys)
            {
                SetNull(key);
            }
        }

        public bool FullCollection { get; private set; }
        public bool NeedUpdate { get; set; }
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
                var elementType = objType.TryGetElementType(typeof (ICollection<>));
                if (elementType != null)
                {
                    var cache = _cacheFactory.GetCache(elementType);

                    // ReSharper disable once AssignNullToNotNullAttribute
                    var cachedItems = cache.Update(relation, (obj as IEnumerable).Cast<object>());
                    result.Add(new RelationInstance(cachedItems.ToArray(), elementType, relation, cache));
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
            EntityIndex indexValue, Dictionary<EntityConditionalIndexInfo, EntityIndex> conditional, bool ignoreState = false)
        {
            lock (exist)
            {
                if (exist.NeedUpdate || ignoreState)
                {
                    if (exist.UniqueIndex != null && indexValue != exist.UniqueIndex)
                    {
                        CachedEntity<T> temp;
                        _indexedDictionary.TryRemove(exist.UniqueIndex, out temp);
                    }
                    if (indexValue != null)
                        _indexedDictionary.AddOrUpdate(indexValue, exist, (index, _) => exist);
                    IEnumerable<EntityConditionalIndexInfo> removeList;
                    if (conditional == null)
                    {
                        removeList = _conditionalIndexedDictionary.Keys.ToArray();
                    }
                    else
                    {
                        foreach (var conditionalIndex in conditional)
                        {
                            _conditionalIndexedDictionary[conditionalIndex.Key].AddOrUpdate(conditionalIndex.Value, exist,
                                (index, _) => exist);
                        }
                        removeList = _conditionalIndexedDictionary.Keys.Where(key => !conditional.ContainsKey(key)).ToArray();
                    }
                    foreach (var indexInfo in removeList)
                    {
                        CachedEntity<T> temp;
                        _conditionalIndexedDictionary[indexInfo].TryRemove(_cacheStorage.GetConditionalIndexValue(exist, indexInfo),
                            out temp);
                    }
                    exist.UniqueIndex = indexValue;
                    exist.ConditionalIndexes = conditional;
                    exist.NeedUpdate = false;
                    if (exist.EntityUntyped != (object) newEntity)
                    {
                        if (newEntity != null)
                        {
                            _typeConverter.CopyInto(exist.Entity, newEntity, typeof (T));
                            UpdateRelations(exist, newEntity, newRelations);
                        }
                        else
                        {
                            exist.Entity = default(T);
                        }
                    }
                }
            }
            return exist;
        }

        #endregion
    }
}