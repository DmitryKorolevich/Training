using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using VitalChoice.Caching.Extensions;
using VitalChoice.Caching.Interfaces;
using VitalChoice.Caching.Relational;
using VitalChoice.Caching.Services.Cache.Base;

namespace VitalChoice.Caching.Services.Cache
{
    public sealed class CacheStorage<T> : ICacheKeysStorage<T>, IDisposable
    {
        private readonly IInternalEntityCacheFactory _cacheFactory;
        private readonly EntityPrimaryKeyInfo _primaryKeyInfo;
        private readonly EntityCacheableIndexInfo _indexInfo;
        private readonly ICollection<EntityForeignKeyInfo> _foreignKeyInfos;
        private readonly ICollection<EntityCacheableIndexInfo> _nonUniqueIndexes;
        private readonly EntityInfo _entityInfo;

        public CacheStorage(IEntityInfoStorage keyStorage, IInternalEntityCacheFactory cacheFactory)
        {
            _cacheFactory = cacheFactory;
            if (keyStorage.GetEntityInfo<T>(out _entityInfo))
            {
                _primaryKeyInfo = _entityInfo.PrimaryKey;
                _indexInfo = _entityInfo.CacheableIndex;
                _foreignKeyInfos = _entityInfo.ForeignKeys;
                _nonUniqueIndexes = _entityInfo.NonUniqueIndexes;
                DependentTypes = _entityInfo.DependentTypes;
            }
        }

        private readonly ConcurrentDictionary<RelationInfo, ICacheData<T>> _cacheData =
            new ConcurrentDictionary<RelationInfo, ICacheData<T>>();



        public ICacheData<T> GetCacheData(RelationInfo relationInfo)
        {
            return _cacheData.GetOrAdd(relationInfo,
                r => new CacheData<T>(_cacheFactory, this, _entityInfo, relationInfo));
        }

        public ICollection<ICacheData<T>> AllCacheDatas => _cacheData.Values;

        public bool GetCacheExist(RelationInfo relationInfo)
        {
            ICacheData<T> data;
            if (_cacheData.TryGetValue(relationInfo, out data))
            {
                return !data.Empty;
            }
            return false;
        }

        public bool GetIsCacheFullCollection(RelationInfo relationInfo)
        {
            ICacheData<T> data;
            if (_cacheData.TryGetValue(relationInfo, out data))
            {
                return data.FullCollection;
            }
            return false;
        }

        public EntityForeignKey GetForeignKeyValue(T entity, EntityForeignKeyInfo foreignKeyInfo)
        {
            return foreignKeyInfo.GetForeignKeyValue(entity);
        }

        public ICollection<KeyValuePair<EntityForeignKeyInfo, EntityForeignKey>> GetForeignKeyValues(T entity)
        {
            return
                _foreignKeyInfos?.Select(f => new KeyValuePair<EntityForeignKeyInfo, EntityForeignKey>(f, f.GetForeignKeyValue(entity)))
                    .ToArray() ?? new KeyValuePair<EntityForeignKeyInfo, EntityForeignKey>[0];
        }

        public EntityIndex GetNonUniqueIndexValue(T entity, EntityCacheableIndexInfo indexInfo)
        {
            return indexInfo.GetIndexValue(entity);
        }

        public ICollection<KeyValuePair<EntityCacheableIndexInfo, EntityIndex>> GetNonUniqueIndexValues(T entity)
        {
            return
                _nonUniqueIndexes?.Select(f => new KeyValuePair<EntityCacheableIndexInfo, EntityIndex>(f, f.GetIndexValue(entity)))
                    .ToArray() ?? new KeyValuePair<EntityCacheableIndexInfo, EntityIndex>[0];
        }

        public EntityKey GetPrimaryKeyValue(T entity)
        {
            return _primaryKeyInfo.GetPrimaryKeyValue(entity);
        }

        public EntityIndex GetIndexValue(T entity)
        {
            return _indexInfo?.GetIndexValue(entity);
        }

        public EntityIndex GetConditionalIndexValue(T entity, EntityConditionalIndexInfo conditionalInfo)
        {
            return conditionalInfo.GetConditionalIndexValue(entity);
        }

        public EntityForeignKey GetForeignKeyValue(object entity, EntityForeignKeyInfo foreignKeyInfo)
        {
            return GetForeignKeyValue((T) entity, foreignKeyInfo);
        }

        public ICollection<KeyValuePair<EntityForeignKeyInfo, EntityForeignKey>> GetForeignKeyValues(object entity)
        {
            return GetForeignKeyValues((T) entity);
        }

        public EntityIndex GetNonUniqueIndexValue(object entity, EntityCacheableIndexInfo indexInfo)
        {
            return GetNonUniqueIndexValue((T) entity, indexInfo);
        }

        public ICollection<KeyValuePair<EntityCacheableIndexInfo, EntityIndex>> GetNonUniqueIndexValues(object entity)
        {
            return GetNonUniqueIndexValues((T) entity);
        }

        public EntityKey GetPrimaryKeyValue(object entity)
        {
            return GetPrimaryKeyValue((T) entity);
        }

        public EntityIndex GetIndexValue(object entity)
        {
            return GetIndexValue((T) entity);
        }

        public EntityIndex GetConditionalIndexValue(object entity, EntityConditionalIndexInfo conditionalInfo)
        {
            return GetConditionalIndexValue((T) entity, conditionalInfo);
        }

        public ICollection<KeyValuePair<Type, EntityCacheableIndexRelationInfo>> DependentTypes { get; }

        public void Dispose()
        {
            foreach (var data in _cacheData.Values)
            {
                data.Clear();
            }
            _cacheData.Clear();
        }
    }
}