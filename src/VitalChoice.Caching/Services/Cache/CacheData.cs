using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using VitalChoice.Caching.Interfaces;
using VitalChoice.Caching.Relational;
using VitalChoice.Caching.Services.Cache.Base;
using VitalChoice.Data.Extensions;
using VitalChoice.Ecommerce.Domain;
using VitalChoice.Ecommerce.Domain.Helpers;
using VitalChoice.ObjectMapping.Base;
using VitalChoice.ObjectMapping.Interfaces;

namespace VitalChoice.Caching.Services.Cache
{
    public sealed class CacheData<T> : ICacheData<T>
    {
        private readonly IInternalEntityCacheFactory _cacheFactory;
        private readonly CacheStorage<T> _cacheStorage;
        private readonly RelationInfo _relationInfo;

        private readonly object _lockObj = new object();

        private readonly CacheCluster<EntityKey, T> _mainCluster;

        private readonly CacheCluster<EntityIndex, T> _indexedCluster;

        private readonly Dictionary<EntityConditionalIndexInfo, CacheCluster<EntityIndex, T>> _conditionalIndexedDictionary;

        private readonly Dictionary<EntityCacheableIndexInfo, ConcurrentDictionary<EntityIndex, CacheCluster<EntityKey, T>>> _nonUniqueIndexedDictionary;
        private volatile bool _needUpdate;

        public CacheData(IInternalEntityCacheFactory cacheFactory, CacheStorage<T> cacheStorage,
            ICollection<EntityConditionalIndexInfo> conditionalIndexes, ICollection<EntityCacheableIndexInfo> nonUniqueIndexes, RelationInfo relationInfo)
        {
            _mainCluster = new CacheCluster<EntityKey, T>();
            _indexedCluster = new CacheCluster<EntityIndex, T>();
            _cacheFactory = cacheFactory;
            _cacheStorage = cacheStorage;
            _relationInfo = relationInfo;
            _conditionalIndexedDictionary = new Dictionary<EntityConditionalIndexInfo, CacheCluster<EntityIndex, T>>();
            foreach (var conditionalIndex in conditionalIndexes)
            {
                _conditionalIndexedDictionary.Add(conditionalIndex, new CacheCluster<EntityIndex, T>());
            }
            _nonUniqueIndexedDictionary = new Dictionary<EntityCacheableIndexInfo, ConcurrentDictionary<EntityIndex, CacheCluster<EntityKey, T>>>();
            foreach (var nonUniqueIndex in nonUniqueIndexes)
            {
                _nonUniqueIndexedDictionary.Add(nonUniqueIndex, new ConcurrentDictionary<EntityIndex, CacheCluster<EntityKey, T>>());
            }
        }

        public CacheCluster<EntityKey, T> Get(EntityCacheableIndexInfo nonUniqueIndexInfo, EntityIndex index)
        {
            ConcurrentDictionary<EntityIndex, CacheCluster<EntityKey, T>> indexedPartition;
            if (_nonUniqueIndexedDictionary.TryGetValue(nonUniqueIndexInfo, out indexedPartition))
            {
                CacheCluster<EntityKey, T> cluster;
                if (indexedPartition.TryGetValue(index, out cluster))
                {
                    return cluster;
                }
            }
            return null;
        }

        public CachedEntity<T> Get(EntityKey key)
        {
            // ReSharper disable once InconsistentlySynchronizedField
            return _mainCluster.Get(key);
        }

        public CachedEntity<T> Get(EntityIndex key)
        {
            // ReSharper disable once InconsistentlySynchronizedField
            return _indexedCluster.Get(key);
        }

        public CachedEntity<T> Get(EntityConditionalIndexInfo conditionalIndex, EntityIndex key)
        {
            // ReSharper disable once InconsistentlySynchronizedField
            return _conditionalIndexedDictionary[conditionalIndex].Get(key);
        }

        public ICollection<CachedEntity<T>> GetAll()
        {
            // ReSharper disable once InconsistentlySynchronizedField
            return _mainCluster.GetItems();
        }

        public void Clear()
        {
            lock (_lockObj)
            {
                NeedUpdate = true;
                _mainCluster.Clear();
                _indexedCluster.Clear();
                foreach (var conditionalIndex in _conditionalIndexedDictionary)
                {
                    conditionalIndex.Value.Clear();
                }
            }
        }

        public IEnumerable<CachedEntity> GetUntyped(EntityCacheableIndexInfo nonUniqueIndexInfo, EntityIndex index)
        {
            var cluster = Get(nonUniqueIndexInfo, index);
            return cluster?.GetItems();
        }

        public CachedEntity GetUntyped(EntityKey pk)
        {
            return Get(pk);
        }

        public IEnumerable<CachedEntity> GetAllUntyped()
        {
            // ReSharper disable once InconsistentlySynchronizedField
            return _mainCluster.GetItems();
        }

        public CachedEntity TryRemoveUntyped(EntityKey key)
        {
            return TryRemove(key);
        }

        public bool GetHasRelation(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                return true;
            return _relationInfo?.RelationsDict.ContainsKey(name) ?? false;
        }

        public CachedEntity<T> TryRemove(EntityKey key)
        {
            lock (_lockObj)
            {
                var removed = _mainCluster.Remove(key);
                if (removed == null)
                    return null;
                if (removed.UniqueIndex != null)
                    _indexedCluster.Remove(removed.UniqueIndex);
                removed.ConditionalIndexes.ForEach(
                    conditionalIndex => _conditionalIndexedDictionary[conditionalIndex.Key].Remove(conditionalIndex.Value));
                if (removed.NonUniqueIndexes != null)
                {
                    foreach (var nonUniquePartition in removed.NonUniqueIndexes)
                    {
                        CacheCluster<EntityKey, T> cluster;
                        if (_nonUniqueIndexedDictionary[nonUniquePartition.Key].TryGetValue(nonUniquePartition.Value, out cluster))
                        {
                            cluster.Remove(key);
                            if (cluster.IsEmpty)
                            {
                                _nonUniqueIndexedDictionary[nonUniquePartition.Key].TryRemove(nonUniquePartition.Value, out cluster);
                            }
                        }
                    }
                }
                return removed;
            }
        }

        public CachedEntity<T> Update(T entity)
        {
            if (entity == null)
                return null;
            lock (_lockObj)
            {
                var pk = _cacheStorage.GetPrimaryKeyValue(entity);
                return _mainCluster.Update(pk, entity, e => CreateNew(pk, e), (e, exist) => UpdateExist(pk, e, exist));
            }
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
            NeedUpdate = false;
        }

        public void SetNull(EntityKey pk)
        {
            if (pk == null)
                return;
            lock (_lockObj)
            {
                _mainCluster.Update(pk, new CachedEntity<T>(default(T), this));
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

        public bool NeedUpdate
        {
            get { return _needUpdate; }
            set { _needUpdate = value; }
        }

        public bool Empty => _mainCluster.IsEmpty;

        #region Helper Methods

        private void UpdateRelations(T newEntity)
        {
            if (newEntity == null)
                return;

            foreach (var relation in _relationInfo.Relations)
            {
                var obj = relation.GetRelatedObject(newEntity);

                if (obj == null)
                    continue;

                var objType = obj.GetType();
                var elementType = objType.TryGetElementType(typeof (ICollection<>));
                if (elementType != null)
                {
                    var cache = _cacheFactory.GetCache(elementType);
                    // ReSharper disable once PossibleNullReferenceException
                    // ReSharper disable once LoopCanBeConvertedToQuery
                    foreach (var item in obj as IEnumerable)
                    {
                        cache.Update(item, relation);
                    }
                }
                else
                {
                    var cache = _cacheFactory.GetCache(objType);
                    cache.Update(obj, relation);
                }
            }
        }

        private CachedEntity<T> CreateNew(EntityKey pk, T entity)
        {
            var indexValue = _cacheStorage.GetIndexValue(entity);
            var conditional =
                _conditionalIndexedDictionary.Keys.Where(c => c.CheckCondition(entity))
                    .ToDictionary(c => c, c => _cacheStorage.GetConditionalIndexValue(entity, c));

            var nonUnique = _cacheStorage.GetNonUniqueIndexValues(entity);
            var foreignKeys = _cacheStorage.GetForeignKeyValues(entity);
            CachedEntity<T> cached;
            if (entity == null)
            {
                cached = new CachedEntity<T>(default(T), this);
            }
            else
            {
                cached = new CachedEntity<T>(entity, this)
                {
                    ForeignKeys = foreignKeys,
                    ConditionalIndexes = conditional,
                    UniqueIndex = indexValue,
                    NonUniqueIndexes = nonUnique
                };
                UpdateRelations(entity);
                foreach (var conditionalIndex in conditional)
                {
                    _conditionalIndexedDictionary[conditionalIndex.Key].Update(conditionalIndex.Value, cached);
                }
                foreach (var nonUniquePartition in nonUnique)
                {
                    _nonUniqueIndexedDictionary[nonUniquePartition.Key].AddOrUpdate(nonUniquePartition.Value, index =>
                    {
                        var cluster = new CacheCluster<EntityKey, T>();
                        cluster.Update(pk, cached);
                        return cluster;
                    }, (index, cluster) =>
                    {
                        cluster.Update(pk, cached);
                        return cluster;
                    });
                }
            }
            return cached;
        }

        private CachedEntity<T> UpdateExist(EntityKey pk, T entity, CachedEntity<T> exist)
        {
            lock (exist)
            {
                if (exist.NeedUpdate && exist.EntityUntyped != (object) entity)
                {
                    var indexValue = _cacheStorage.GetIndexValue(entity);
                    var conditional =
                        _conditionalIndexedDictionary.Keys.Where(c => c.CheckCondition(entity))
                            .ToDictionary(c => c, c => _cacheStorage.GetConditionalIndexValue(entity, c));

                    var nonUnique = _cacheStorage.GetNonUniqueIndexValues(entity);
                    var foreignKeys = _cacheStorage.GetForeignKeyValues(entity);

                    if (exist.UniqueIndex != null && indexValue != exist.UniqueIndex)
                    {
                        _indexedCluster.Remove(exist.UniqueIndex);
                    }
                    if (indexValue != null)
                        _indexedCluster.Update(indexValue, exist);
                    IEnumerable<EntityConditionalIndexInfo> removeList;
                    if (conditional.Any(c => c.Value == null))
                    {
                        removeList = _conditionalIndexedDictionary.Keys.ToArray();
                    }
                    else
                    {
                        foreach (var conditionalIndex in conditional)
                        {
                            _conditionalIndexedDictionary[conditionalIndex.Key].Update(conditionalIndex.Value, exist);
                        }
                        removeList = _conditionalIndexedDictionary.Keys.Where(key => !conditional.ContainsKey(key)).ToArray();
                    }
                    foreach (var indexInfo in removeList)
                    {
                        _conditionalIndexedDictionary[indexInfo].Remove(_cacheStorage.GetConditionalIndexValue(exist, indexInfo));
                    }
                    if (exist.NonUniqueIndexes != null)
                    {
                        foreach (var nonUniquePartition in exist.NonUniqueIndexes)
                        {
                            CacheCluster<EntityKey, T> cluster;
                            if (_nonUniqueIndexedDictionary[nonUniquePartition.Key].TryGetValue(nonUniquePartition.Value, out cluster))
                            {
                                cluster.Remove(pk);
                                if (cluster.IsEmpty)
                                {
                                    _nonUniqueIndexedDictionary[nonUniquePartition.Key].TryRemove(nonUniquePartition.Value, out cluster);
                                }
                            }
                        }
                    }
                    foreach (var nonUniquePartition in nonUnique)
                    {
                        _nonUniqueIndexedDictionary[nonUniquePartition.Key].AddOrUpdate(nonUniquePartition.Value, index =>
                        {
                            var cluster = new CacheCluster<EntityKey, T>();
                            cluster.Update(pk, exist);
                            return cluster;
                        }, (index, cluster) =>
                        {
                            cluster.Update(pk, exist);
                            return cluster;
                        });
                    }
                    exist.ForeignKeys = foreignKeys;
                    exist.NonUniqueIndexes = nonUnique;
                    exist.UniqueIndex = indexValue;
                    exist.ConditionalIndexes = conditional;
                    exist.NeedUpdate = false;

                    if (entity != null)
                    {
                        TypeConverter.CopyInto(exist.Entity, entity, typeof (T));
                        UpdateRelations(entity);
                    }
                    else
                    {
                        exist.Entity = default(T);
                    }
                }
            }
            return exist;
        }

        #endregion
    }
}