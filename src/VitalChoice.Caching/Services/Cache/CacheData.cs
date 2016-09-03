using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking.Internal;
using VitalChoice.Caching.Extensions;
using VitalChoice.Caching.Interfaces;
using VitalChoice.Caching.Relational;
using VitalChoice.Caching.Relational.ChangeTracking;
using VitalChoice.Caching.Services.Cache.Base;
using VitalChoice.Data.Extensions;
using VitalChoice.Ecommerce.Domain.Helpers;

namespace VitalChoice.Caching.Services.Cache
{
    public sealed class CacheData<T> : ICacheData<T>
    {
        private readonly IInternalEntityCacheFactory _cacheFactory;
        private readonly EntityInfo _entityInfo;
        private readonly RelationInfo _relationInfo;

        private readonly CacheCluster<EntityKey, T> _mainCluster;

        private readonly CacheCluster<EntityIndex, T> _indexedCluster;

        private readonly Dictionary<EntityConditionalIndexInfo, CacheCluster<EntityIndex, T>> _conditionalIndexedDictionary;

        private readonly Dictionary<EntityCacheableIndexInfo, ConcurrentDictionary<EntityIndex, CacheCluster<EntityKey, T>>>
            _nonUniqueIndexedDictionary;

        private volatile bool _needUpdate;

        public CacheData(IInternalEntityCacheFactory cacheFactory, EntityInfo entityInfo, RelationInfo relationInfo)
        {
            _mainCluster = new CacheCluster<EntityKey, T>();
            _indexedCluster = new CacheCluster<EntityIndex, T>();
            _cacheFactory = cacheFactory;
            _entityInfo = entityInfo;
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
            return _mainCluster.Get(key);
        }

        public CachedEntity<T> Get(EntityIndex key)
        {
            return _indexedCluster.Get(key);
        }

        public CachedEntity<T> Get(EntityConditionalIndexInfo conditionalIndex, EntityIndex key)
        {
            return _conditionalIndexedDictionary[conditionalIndex].Get(key);
        }

        public ICollection<CachedEntity<T>> GetAll()
        {
            return _mainCluster.GetItems();
        }

        public void Clear()
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
            return _mainCluster.GetItems();
        }

        public CachedEntity TryRemoveUntyped(EntityKey key)
        {
            return TryRemove(key);
        }

        public CachedEntity Update(object entity, object dbContext)
        {
            return Update((T) entity, dbContext);
        }

        public CachedEntity UpdateExist(object entity, object dbContext)
        {
            return UpdateExist((T) entity, dbContext);
        }

        public bool ItemExist(EntityKey key)
        {
                return _mainCluster.Exist(key);
        }

        public bool GetHasRelation(string name)
        {
            if (string.IsNullOrEmpty(name))
                return true;
            return _relationInfo.HasRelation(name);
        }

        public CachedEntity<T> TryRemove(EntityKey key)
        {
            lock (this)
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

        public CachedEntity<T> Update(T entity, object dbContext)
        {
            if (entity == null)
                return null;
            return UpdateUnsafe((T) entity.DeepCloneItem(_relationInfo), dbContext);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private CachedEntity<T> UpdateUnsafe(T entity, object dbContext)
        {
            var pk = _entityInfo.PrimaryKey.GetPrimaryKeyValue(entity);
            return _mainCluster.AddOrUpdate(pk, entity, e => CreateNew(pk, e, dbContext),
                (e, exist) => UpdateExist(pk, e, exist, dbContext));
        }

        public CachedEntity<T> UpdateExist(T entity, object dbContext)
        {
            if (entity == null)
                return null;
            return UpdateExistUnsafe(entity, dbContext);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private CachedEntity<T> UpdateExistUnsafe(T entity, object dbContext)
        {
            var pk = _entityInfo.PrimaryKey.GetPrimaryKeyValue(entity);
            return _mainCluster.Update(pk, entity, (e, exist) => UpdateExist(pk, e, exist, dbContext));
        }

        public CachedEntity<T> UpdateKeepRelations(T entity, ICacheStateManager stateManager, object dbContext)
        {
            if (entity == null)
                return null;

            var pk = _entityInfo.PrimaryKey.GetPrimaryKeyValue(entity);
            var cached = Get(pk);
            if (cached == null)
                return null;
            using (cached.Lock())
            {
                if (!UpdateEntityWithRelations(entity, stateManager, cached, dbContext))
                    return null;
                SyncInternalCache(pk, entity, cached, dbContext);
            }
            return cached;
        }

        private bool UpdateEntityWithRelations(T entity, ICacheStateManager stateManager,
            CachedEntity<T> cached, object dbContext)
        {
            ICollection<RelationInfo> relationsToClone;
            if (!GetAllNormalizedAndTracked(entity, stateManager, out relationsToClone, cached))
            {
                cached.SetNeedUpdate(true, dbContext);
                return false;
            }
            if (cached.Entity != null)
            {
                entity.UpdateCloneRelations(relationsToClone, cached.Entity);
                entity.UpdateNonRelatedObjects(cached.Entity);
                UpdateRelations(cached.Entity, dbContext, relationsToClone);
            }
            else
            {
                cached.Entity = (T) entity.DeepCloneItem(_relationInfo);
                UpdateRelations(cached.Entity, dbContext, _relationInfo.Relations);
            }
            cached.NeedUpdateRelated.Clear();
            return true;
        }

        private bool GetAllNormalizedAndTracked(object entity,
            ICacheStateManager stateManager, out ICollection<RelationInfo> relationsToClone,
            CachedEntity<T> cached)
        {

            relationsToClone = _relationInfo.Relations.Where(r => cached.NeedUpdateRelated.Contains(r.Name)).ToArray();
            return GetIsNormalized(entity, stateManager, relationsToClone, _entityInfo, false) &&
                   GetIsNormalized(cached.Entity, stateManager, relationsToClone, _entityInfo, true);
        }

        private bool GetIsNormalized(object entity, ICacheStateManager stateManager,
            ICollection<RelationInfo> relationsToClone, EntityInfo entityInfo, bool trackKeyOnly)
        {
            foreach (var relation in relationsToClone)
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
                    var pkInfo = relation.EntityInfo.PrimaryKey;
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
                        if (stateManager != null)
                        {
                            if (!stateManager.IsTracked(relation.EntityInfo, pk, newItem, trackKeyOnly))
                            {
                                return false;
                            }
                        }
                        if (!GetIsNormalized(newItem, stateManager, relation.Relations, relation.EntityInfo, trackKeyOnly))
                        {
                            return false;
                        }
                    }
                }
                else
                {
                    var newRelatedKey = relationReference?.GetPrimaryKeyValue(entity);
                    if (newRelatedKey?.IsValid ?? false)
                    {
                        if (newRelated == null)
                        {
                            if (relationReference != entityInfo.PrimaryKey)
                            {
                                return false;
                            }
                            if (stateManager?.IsTracked(relation.EntityInfo, newRelatedKey, null, trackKeyOnly) ?? true)
                            {
                                return false;
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
                        if (stateManager != null)
                        {
                            if (!stateManager.IsTracked(relation.EntityInfo, newRelatedKey, newRelated, trackKeyOnly))
                            {
                                return false;
                            }
                        }
                        if (!GetIsNormalized(newRelated, stateManager, relation.Relations, relation.EntityInfo, trackKeyOnly))
                        {
                            return false;
                        }
                    }
                }
            }
            return true;
        }

        public bool Update(IEnumerable<T> entities, object dbContext)
        {
            foreach (var entity in entities.Select(e => (T) e.DeepCloneItem(_relationInfo)))
            {
                if (entity != null && UpdateUnsafe(entity, dbContext) == null)
                {
                    return false;
                }
            }
            return true;
        }

        public bool UpdateExist(IEnumerable<T> entities, object dbContext)
        {
                foreach (var entity in entities)
                {
                    if (entity != null && UpdateExistUnsafe(entity, dbContext) == null)
                        return false;
                }
            return true;
        }

        public bool UpdateAll(IEnumerable<T> entities, object dbContext)
        {
            lock (this)
            {
                Clear();
                var result = Update(entities, dbContext);
                FullCollection = true;
                NeedUpdate = false;
                return result;
            }
        }

        public void SetNull(EntityKey pk)
        {
            if (pk == null)
                return;
            SetNullUnsafe(pk);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void SetNullUnsafe(EntityKey pk)
        {
            _mainCluster.AddOrUpdate(pk, new CachedEntity<T>(default(T), this));
        }

        public void SetNull(IEnumerable<EntityKey> keys)
        {
            foreach (var key in keys)
            {
                SetNullUnsafe(key);
            }
        }

        public bool FullCollection { get; private set; }

        public bool NeedUpdate
        {
            get { return _needUpdate; }
            set { _needUpdate = value; }
        }

        public bool Empty => _mainCluster.IsEmpty;
        public RelationInfo Relations => _relationInfo;
        public EntityInfo EntityInfo => _entityInfo;

        #region Helper Methods

        private void UpdateExistsRelations(T newEntity, object dbContext)
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
                            data.UpdateExist(item, dbContext);
                        }
                    }
                }
                else
                {
                    var cache = _cacheFactory.GetCache(objType);
                    if (cache.GetCacheExist(relation))
                    {
                        var data = cache.GetCacheData(relation);
                        data.UpdateExist(obj, dbContext);
                    }
                }
            }
        }

        private void UpdateRelations(T newEntity, object dbContext, IEnumerable<RelationInfo> relations = null)
        {
            if (newEntity == null)
                return;

            foreach (var relation in relations ?? _relationInfo.Relations)
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
                        data.Update(item, dbContext);
                    }
                }
                else
                {
                    var cache = _cacheFactory.GetCache(objType);
                    var data = cache.GetCacheData(relation);
                    data.Update(obj, dbContext);
                }
            }
        }

        private CachedEntity<T> CreateNew(EntityKey pk, T entity, object dbContext)
        {
            var indexValue = _entityInfo.CacheableIndex.GetIndexValue(entity);
            var conditional =
                _conditionalIndexedDictionary.Keys.Where(c => c.CheckCondition(entity))
                    .Select(c => new KeyValuePair<EntityConditionalIndexInfo, EntityIndex>(c, c.GetConditionalIndexValue(entity)))
                    .ToArray();

            var nonUnique = _entityInfo.NonUniqueIndexes.GetNonUniqueIndexes(entity);
            var foreignKeys = _entityInfo.ForeignKeys.GetForeignKeyValues(entity);
            CachedEntity<T> cachedEntity;
            if (entity == null)
            {
                cachedEntity = new CachedEntity<T>(default(T), this);
            }
            else
            {
                cachedEntity = new CachedEntity<T>(entity, this)
                {
                    ForeignKeys = foreignKeys,
                    ConditionalIndexes = conditional,
                    UniqueIndex = indexValue,
                    NonUniqueIndexes = nonUnique
                };
                UpdateRelations(entity, dbContext);
                if (indexValue != null)
                    _indexedCluster.AddOrUpdate(indexValue, cachedEntity);
                foreach (var conditionalIndex in conditional)
                {
                    _conditionalIndexedDictionary[conditionalIndex.Key].AddOrUpdate(conditionalIndex.Value, cachedEntity);
                }
                if (nonUnique != null)
                {
                    foreach (var nonUniquePartition in nonUnique)
                    {
                        _nonUniqueIndexedDictionary[nonUniquePartition.Key].AddOrUpdate(nonUniquePartition.Value, index =>
                        {
                            var cluster = new CacheCluster<EntityKey, T>();
                            cluster.AddOrUpdate(pk, cachedEntity);
                            return cluster;
                        }, (index, cluster) =>
                        {
                            cluster.AddOrUpdate(pk, cachedEntity);
                            return cluster;
                        });
                    }
                }
            }
            return cachedEntity;
        }

        private CachedEntity<T> UpdateExist(EntityKey pk, T entity, CachedEntity<T> exist, object dbContext)
        {
            using (exist.Lock())
            {
                if (exist.NeedUpdate && exist.EntityUntyped != (object) entity)
                {
                    SyncInternalCache(pk, entity, exist, dbContext);
                    if (entity != null)
                    {
                        //if (!UpdateEntityWithRelations(entity, trackedEntities, exist))
                        //    return null;
                        UpdateRelations(entity, dbContext);
                        exist.Entity = entity;
                    }
                    else
                    {
                        exist.Entity = default(T);
                    }
                    return exist;
                }
                return exist;
            }
        }

        private void SyncInternalCache(EntityKey pk, T entity, CachedEntity<T> cachedEntity, object dbContext)
        {
            var indexValue = _entityInfo.CacheableIndex.GetIndexValue(entity);
            var conditional =
                _conditionalIndexedDictionary.Keys.Where(c => c.CheckCondition(entity))
                    .ToDictionary(c => c, c => c.GetConditionalIndexValue(entity));

            var nonUnique = _entityInfo.NonUniqueIndexes.GetNonUniqueIndexes(entity);
            var foreignKeys = _entityInfo.ForeignKeys.GetForeignKeyValues(entity);

            using(cachedEntity.Lock())
            {
                if (indexValue != cachedEntity.UniqueIndex)
                {
                    if (cachedEntity.UniqueIndex != null)
                    {
                        _indexedCluster.Remove(cachedEntity.UniqueIndex);
                    }
                    if (indexValue != null)
                    {
                        _indexedCluster.AddOrUpdate(indexValue, cachedEntity);
                    }
                }
                IEnumerable<EntityConditionalIndexInfo> removeList;
                if (conditional.Any(c => c.Value == null))
                {
                    removeList = _conditionalIndexedDictionary.Keys;
                }
                else
                {
                    foreach (var conditionalIndex in conditional)
                    {
                        _conditionalIndexedDictionary[conditionalIndex.Key].AddOrUpdate(conditionalIndex.Value, cachedEntity);
                    }
                    removeList = _conditionalIndexedDictionary.Keys.Where(key => !conditional.ContainsKey(key));
                }
                foreach (var indexInfo in removeList)
                {
                    _conditionalIndexedDictionary[indexInfo].Remove(indexInfo.GetConditionalIndexValue(cachedEntity.EntityUntyped));
                }
                if (cachedEntity.NonUniqueIndexes != null)
                {
                    foreach (var existPair in cachedEntity.NonUniqueIndexes)
                    {
                        if (nonUnique != null)
                        {
                            var newIndex = nonUnique[existPair.Key];
                            if (newIndex == existPair.Value)
                            {
                                continue;
                            }
                            var existingPartition = _nonUniqueIndexedDictionary[existPair.Key];
                            RemoveNonUniqueIndex(pk, existingPartition, existPair.Value);
                            AddNonUniqueIndex(pk, cachedEntity, existingPartition, newIndex);
                        }
                        else
                        {
                            var existingPartition = _nonUniqueIndexedDictionary[existPair.Key];
                            RemoveNonUniqueIndex(pk, existingPartition, existPair.Value);
                        }
                    }
                }
                else if (nonUnique != null)
                {
                    foreach (var newPair in nonUnique)
                    {
                        var existingPartition = _nonUniqueIndexedDictionary[newPair.Key];
                        AddNonUniqueIndex(pk, cachedEntity, existingPartition, newPair.Value);
                    }
                }
                cachedEntity.ForeignKeys = foreignKeys;
                cachedEntity.NonUniqueIndexes = nonUnique;
                cachedEntity.UniqueIndex = indexValue;
                cachedEntity.ConditionalIndexes = conditional;
                cachedEntity.SetNeedUpdate(false, dbContext);
            }
        }

        private static void AddNonUniqueIndex(EntityKey pk, CachedEntity<T> exist,
            ConcurrentDictionary<EntityIndex, CacheCluster<EntityKey, T>> existingPartition, EntityIndex newIndex)
        {
            existingPartition.AddOrUpdate(newIndex, index =>
            {
                var newCluster = new CacheCluster<EntityKey, T>();
                newCluster.AddOrUpdate(pk, exist);
                return newCluster;
            }, (index, existingCluster) =>
            {
                existingCluster.AddOrUpdate(pk, exist);
                return existingCluster;
            });
        }

        private static void RemoveNonUniqueIndex(EntityKey pk,
            ConcurrentDictionary<EntityIndex, CacheCluster<EntityKey, T>> existingPartition,
            EntityIndex index)
        {
            CacheCluster<EntityKey, T> cluster;
            if (existingPartition.TryGetValue(index, out cluster))
            {
                cluster.Remove(pk);
                if (cluster.IsEmpty)
                {
                    existingPartition.TryRemove(index, out cluster);
                }
            }
        }

        #endregion
    }
}