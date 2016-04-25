using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using Microsoft.Data.Entity;
using Microsoft.Data.Entity.ChangeTracking;
using VitalChoice.Caching.Extensions;
using VitalChoice.Caching.Interfaces;
using VitalChoice.Caching.Relational;
using VitalChoice.Caching.Relational.ChangeTracking;
using VitalChoice.Caching.Services.Cache.Base;
using VitalChoice.Data.Extensions;
using VitalChoice.Ecommerce.Domain.Helpers;
using VitalChoice.ObjectMapping.Base;
using VitalChoice.ObjectMapping.Extensions;
using VitalChoice.Profiling.Base;

namespace VitalChoice.Caching.Services.Cache
{
    public sealed class CacheData<T> : ICacheData<T>
    {
        private readonly IInternalEntityCacheFactory _cacheFactory;
        private readonly EntityInfo _entityInfo;
        private readonly IEntityInfoStorage _infoStorage;
        private readonly RelationInfo _relationInfo;

        private readonly object _lockObj = new object();

        private readonly CacheCluster<EntityKey, T> _mainCluster;

        private readonly CacheCluster<EntityIndex, T> _indexedCluster;

        private readonly Dictionary<EntityConditionalIndexInfo, CacheCluster<EntityIndex, T>> _conditionalIndexedDictionary;

        private readonly Dictionary<EntityCacheableIndexInfo, ConcurrentDictionary<EntityIndex, CacheCluster<EntityKey, T>>>
            _nonUniqueIndexedDictionary;

        private volatile bool _needUpdate;

        public CacheData(IInternalEntityCacheFactory cacheFactory, EntityInfo entityInfo, IEntityInfoStorage infoStorage, RelationInfo relationInfo)
        {
            _mainCluster = new CacheCluster<EntityKey, T>();
            _indexedCluster = new CacheCluster<EntityIndex, T>();
            _cacheFactory = cacheFactory;
            _entityInfo = entityInfo;
            _infoStorage = infoStorage;
            _relationInfo = relationInfo;
            _conditionalIndexedDictionary = new Dictionary<EntityConditionalIndexInfo, CacheCluster<EntityIndex, T>>();
            foreach (var conditionalIndex in entityInfo.ConditionalIndexes)
            {
                _conditionalIndexedDictionary.Add(conditionalIndex, new CacheCluster<EntityIndex, T>());
            }
            _nonUniqueIndexedDictionary =
                new Dictionary<EntityCacheableIndexInfo, ConcurrentDictionary<EntityIndex, CacheCluster<EntityKey, T>>>();
            foreach (var nonUniqueIndex in entityInfo.NonUniqueIndexes)
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
            lock (_lockObj)
            {
                return _mainCluster.Get(key);
            }
        }

        public CachedEntity<T> Get(EntityIndex key)
        {
            lock (_lockObj)
            {
                return _indexedCluster.Get(key);
            }
        }

        public CachedEntity<T> Get(EntityConditionalIndexInfo conditionalIndex, EntityIndex key)
        {
            lock (_lockObj)
            {
                return _conditionalIndexedDictionary[conditionalIndex].Get(key);
            }
        }

        public ICollection<CachedEntity<T>> GetAll()
        {
            lock (_lockObj)
            {
                return _mainCluster.GetItems().ToArray();
            }
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
                foreach (var nonUniqueIndex in _nonUniqueIndexedDictionary)
                {
                    nonUniqueIndex.Value.Clear();
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
            lock (_lockObj)
            {
                return _mainCluster.GetItems().ToArray();
            }
        }

        public CachedEntity TryRemoveUntyped(EntityKey key)
        {
            return TryRemove(key);
        }

        public CachedEntity Update(object entity)
        {
            return Update((T) entity);
        }

        public CachedEntity UpdateExist(object entity)
        {
            return UpdateExist((T) entity);
        }

        public bool ItemExist(EntityKey key)
        {
            lock (_lockObj)
            {
                return _mainCluster.Exist(key);
            }
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
                                CacheCluster<EntityKey, T> temp;
                                _nonUniqueIndexedDictionary[nonUniquePartition.Key].TryRemove(nonUniquePartition.Value, out temp);
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
                return UpdateUnsafe(entity);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private CachedEntity<T> UpdateUnsafe(T entity)
        {
            var pk = _entityInfo.PrimaryKey.GetPrimaryKeyValue(entity);
            return _mainCluster.AddOrUpdate(pk, entity, e => CreateNew(pk, e), (e, exist) => UpdateExist(pk, e, exist));
        }

        public CachedEntity<T> UpdateExist(T entity)
        {
            if (entity == null)
                return null;
            lock (_lockObj)
            {
                return UpdateExistUnsafe(entity);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private CachedEntity<T> UpdateExistUnsafe(T entity)
        {
            var pk = _entityInfo.PrimaryKey.GetPrimaryKeyValue(entity);
            return _mainCluster.Update(pk, entity, (e, exist) => UpdateExist(pk, e, exist));
        }

        public CachedEntity<T> UpdateKeepRelations(T entity, Dictionary<TrackedEntityKey, EntityEntry> trackedEntities)
        {
            if (entity == null)
                return null;

            lock (_lockObj)
            {
                var pk = _entityInfo.PrimaryKey.GetPrimaryKeyValue(entity);
                var cached = Get(pk);
                if (cached == null)
                    return null;

                    if (!UpdateEntityWithRelations(entity, trackedEntities, cached))
                        return null;

                    SyncInternalCache(pk, entity, cached);
                return cached;
            }
        }

        private bool UpdateEntityWithRelations(T entity, Dictionary<TrackedEntityKey, EntityEntry> trackedEntities, CachedEntity<T> cached)
        {
            if (!GetAllNormalizedAndTracked(entity, _relationInfo, trackedEntities))
            {
                cached.NeedUpdate = true;
                return false;
            }
            cached.Entity = (T)entity.DeepCloneItem(_relationInfo);
            UpdateRelations(cached.Entity);
            cached.NeedUpdateRelated.Clear();
            return true;
            

            //var relatedObjects =
            //    _relationInfo.Relations.Select(
            //        relation =>
            //            new KeyValuePair<RelationInfo, object>(relation, relation.GetRelatedObject(oldEntity)))
            //        .ToArray();

            //foreach (var pair in relatedObjects)
            //{
            //    var newRelated = pair.Key.GetRelatedObject(entity);
            //    EntityRelationalReferenceInfo relationReference = null;
            //    _entityInfo.RelationReferences?.TryGetValue(pair.Key.Name, out relationReference);

            //    if (relationReference == null)
            //    {
            //        pair.Key.SetRelatedObject(oldEntity, (newRelated as IEnumerable<object>).DeepCloneCreateList(pair.Key));
            //    }
            //    else
            //    {
            //        var newRelatedKey = relationReference.GetPrimaryKeyValue(entity);
            //        pair.Key.SetRelatedObject(oldEntity, newRelatedKey.IsValid ? newRelated.DeepCloneItem(pair.Key) : null);
            //    }
            //}

            //UpdateExistsRelations(oldEntity);
            //cached.NeedUpdateRelated.Clear();
            //return true;
        }

        private bool GetAllNormalizedAndTracked(object entity, RelationInfo relations, Dictionary<TrackedEntityKey, EntityEntry> trackedEntities)
        {
            EntityInfo entityInfo;
            if (!_infoStorage.GetEntityInfo(relations.RelationType, out entityInfo))
            {
                return false;
            }
            foreach (var relation in relations.Relations)
            {
                var newRelated = relation.GetRelatedObject(entity);
                EntityRelationalReferenceInfo relationReference = null;
                entityInfo.RelationReferences?.TryGetValue(relation.Name, out relationReference);

                if (relation.IsCollection)
                {
                    if (!(newRelated is IEnumerable<object>))
                    {
                        return false;
                    }
                    var pkInfo = _infoStorage.GetPrimaryKeyInfo(relation.RelationType);
                    var newItems = newRelated as IEnumerable<object>;
                    if (pkInfo == null)
                    {
                        return false;
                    }
                    foreach (var newItem in newItems)
                    {
                        var pk = pkInfo.GetPrimaryKeyValue(newItem);
                        if (!pk.IsValid)
                        {
                            return false;
                        }
                        if (trackedEntities != null)
                        {
                            EntityEntry entry;
                            if (!trackedEntities.TryGetValue(new TrackedEntityKey(relation.RelationType, pk), out entry))
                            {
                                return false;
                            }
                            if (entry.State == EntityState.Detached || entry.Entity != newItem)
                            {
                                return false;
                            }
                        }
                        if (!GetAllNormalizedAndTracked(newItem, relation, trackedEntities))
                        {
                            return false;
                        }
                    }
                }
                else if (relationReference != null)
                {
                    var newRelatedKey = relationReference.GetPrimaryKeyValue(entity);
                    if (newRelatedKey.IsValid)
                    {
                        if (newRelated == null)
                        {
                            if (relationReference != entityInfo.PrimaryKey)
                            {
                                return false;
                            }
                            if (trackedEntities == null)
                            {
                                return false;
                            }
                            EntityEntry entry;
                            if (trackedEntities.TryGetValue(new TrackedEntityKey(relation.RelationType, newRelatedKey), out entry))
                            {
                                if (entry.Entity != null)
                                {
                                    return false;
                                }
                            }
                            if (_cacheFactory.CacheExist(relation.RelationType))
                            {
                                var cache = _cacheFactory.GetCache(relation.RelationType);
                                if (cache.ItemExistAndNotNull(newRelatedKey))
                                {
                                    return false;
                                }
                            }
                            continue;
                        }
                        if (trackedEntities != null)
                        {
                            EntityEntry entry;
                            if (!trackedEntities.TryGetValue(new TrackedEntityKey(relation.RelationType, newRelatedKey), out entry))
                            {
                                return false;
                            }
                            if (entry.State == EntityState.Detached || entry.Entity != newRelated)
                            {
                                return false;
                            }
                        }
                        if (!GetAllNormalizedAndTracked(newRelated, relation, trackedEntities))
                        {
                            return false;
                        }
                    }
                }
            }
            return true;
        }

        public bool Update(IEnumerable<T> entities)
        {
            lock (_lockObj)
            {
                foreach (var entity in entities)
                {
                    if (entity != null && UpdateUnsafe(entity) == null)
                    {
                        return false;
                    }
                }
                return true;
            }
        }

        public bool UpdateExist(IEnumerable<T> entities)
        {
            lock (_lockObj)
            {
                foreach (var entity in entities)
                {
                    if (entity != null && UpdateExistUnsafe(entity) == null)
                        return false;
                }
            }
            return true;
        }

        public bool UpdateAll(IEnumerable<T> entities)
        {
            lock (_lockObj)
            {
                Clear();
                var result = Update(entities);
                FullCollection = true;
                NeedUpdate = false;
                return result;
            }
        }

        public void SetNull(EntityKey pk)
        {
            if (pk == null)
                return;
            lock (_lockObj)
            {
                SetNullUnsafe(pk);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void SetNullUnsafe(EntityKey pk)
        {
            _mainCluster.AddOrUpdate(pk, new CachedEntity<T>(default(T), this));
        }

        public void SetNull(IEnumerable<EntityKey> keys)
        {
            lock (_lockObj)
            {
                foreach (var key in keys)
                {
                    SetNullUnsafe(key);
                }
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

        private void UpdateExistsRelations(T newEntity)
        {
            if (newEntity == null)
                return;

            foreach (var relation in _relationInfo.Relations)
            {
                var obj = relation.GetRelatedObject(newEntity);

                if (obj == null)
                    continue;

                var objType = obj.GetType();
                var elementType = objType.TryGetElementType(typeof(ICollection<>));
                if (elementType != null)
                {
                    if (!(obj is IEnumerable))
                        continue;

                    var cache = _cacheFactory.GetCache(elementType);
                    if (cache.GetCacheExist(relation))
                    {
                        var data = cache.GetCacheData(relation);
                        foreach (var item in obj as IEnumerable)
                        {
                            data.UpdateExist(item);
                        }
                    }
                }
                else
                {
                    var cache = _cacheFactory.GetCache(objType);
                    if (cache.GetCacheExist(relation))
                    {
                        var data = cache.GetCacheData(relation);
                        data.UpdateExist(obj);
                    }
                }
            }
        }

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
                    if (!(obj is IEnumerable))
                        continue;

                    var cache = _cacheFactory.GetCache(elementType);
                    var data = cache.GetCacheData(relation);
                    foreach (var item in obj as IEnumerable)
                    {
                        data.Update(item);
                    }
                }
                else
                {
                    var cache = _cacheFactory.GetCache(objType);
                    var data = cache.GetCacheData(relation);
                    data.Update(obj);
                }
            }
        }

        private CachedEntity<T> CreateNew(EntityKey pk, T entity)
        {
            var indexValue = _entityInfo.CacheableIndex.GetIndexValue(entity);
            var conditional =
                _conditionalIndexedDictionary.Keys.Where(c => c.CheckCondition(entity))
                    .Select(c => new KeyValuePair<EntityConditionalIndexInfo, EntityIndex>(c, c.GetConditionalIndexValue(entity)))
                    .ToArray();

            var nonUnique = _entityInfo.NonUniqueIndexes.GetNonUniqueIndexes(entity).ToArray();
            var foreignKeys = _entityInfo.ForeignKeys.GetForeignKeyValues(entity).ToArray();
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
                if (indexValue != null)
                    _indexedCluster.AddOrUpdate(indexValue, cached);
                foreach (var conditionalIndex in conditional)
                {
                    _conditionalIndexedDictionary[conditionalIndex.Key].AddOrUpdate(conditionalIndex.Value, cached);
                }
                foreach (var nonUniquePartition in nonUnique)
                {
                    _nonUniqueIndexedDictionary[nonUniquePartition.Key].AddOrUpdate(nonUniquePartition.Value, index =>
                    {
                        var cluster = new CacheCluster<EntityKey, T>();
                        cluster.AddOrUpdate(pk, cached);
                        return cluster;
                    }, (index, cluster) =>
                    {
                        cluster.AddOrUpdate(pk, cached);
                        return cluster;
                    });
                }
            }
            return cached;
        }

        private CachedEntity<T> UpdateExist(EntityKey pk, T entity, CachedEntity<T> exist)
        {
            if (exist.NeedUpdate && exist.EntityUntyped != (object) entity)
            {
                var updated = exist.Copy();
                SyncInternalCache(pk, entity, updated);
                if (entity != null)
                {
                    //if (!UpdateEntityWithRelations(entity, trackedEntities, exist))
                    //    return null;
                    updated.Entity = entity;
                    UpdateRelations(entity);
                }
                else
                {
                    updated.Entity = default(T);
                }
                return updated;
            }
            return exist;
        }

        private void SyncInternalCache(EntityKey pk, T entity, CachedEntity<T> exist)
        {
            var indexValue = _entityInfo.CacheableIndex.GetIndexValue(entity);
            var conditional =
                _conditionalIndexedDictionary.Keys.Where(c => c.CheckCondition(entity))
                    .ToDictionary(c => c, c => c.GetConditionalIndexValue(entity));

            var nonUnique = _entityInfo.NonUniqueIndexes.GetNonUniqueIndexes(entity).ToArray();
            var foreignKeys = _entityInfo.ForeignKeys.GetForeignKeyValues(entity).ToArray();

            if (exist.UniqueIndex != null && indexValue != exist.UniqueIndex)
            {
                _indexedCluster.Remove(exist.UniqueIndex);
            }
            if (indexValue != null)
                _indexedCluster.AddOrUpdate(indexValue, exist);
            IEnumerable<EntityConditionalIndexInfo> removeList;
            if (conditional.Any(c => c.Value == null))
            {
                removeList = _conditionalIndexedDictionary.Keys;
            }
            else
            {
                foreach (var conditionalIndex in conditional)
                {
                    _conditionalIndexedDictionary[conditionalIndex.Key].AddOrUpdate(conditionalIndex.Value, exist);
                }
                removeList = _conditionalIndexedDictionary.Keys.Where(key => !conditional.ContainsKey(key));
            }
            foreach (var indexInfo in removeList)
            {
                _conditionalIndexedDictionary[indexInfo].Remove(indexInfo.GetConditionalIndexValue(exist));
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
                    cluster.AddOrUpdate(pk, exist);
                    return cluster;
                }, (index, cluster) =>
                {
                    cluster.AddOrUpdate(pk, exist);
                    return cluster;
                });
            }
            exist.ForeignKeys = foreignKeys;
            exist.NonUniqueIndexes = nonUnique;
            exist.UniqueIndex = indexValue;
            exist.ConditionalIndexes = conditional;
            exist.NeedUpdate = false;
        }

        #endregion
    }
}