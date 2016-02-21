using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VitalChoice.Caching.Extensions;
using VitalChoice.Caching.Interfaces;
using VitalChoice.Caching.Relational;
using VitalChoice.Ecommerce.Domain;
using VitalChoice.ObjectMapping.Interfaces;

namespace VitalChoice.Caching.Services.Cache
{
    public sealed class CacheStorage<T> : ICacheKeysStorage<T>, IDisposable
    {
        private readonly IInternalEntityCacheFactory _cacheFactory;
        private readonly EntityPrimaryKeyInfo _primaryKeyInfo;
        private readonly EntityUniqueIndexInfo _indexInfo;
        private readonly ICollection<EntityConditionalIndexInfo> _conditionalIndexes;

        public CacheStorage(IInternalEntityInfoStorage keyStorage, IInternalEntityCacheFactory cacheFactory)
        {
            _cacheFactory = cacheFactory;
            _primaryKeyInfo = keyStorage.GetPrimaryKeyInfo<T>();
            _indexInfo = keyStorage.GetIndexInfo<T>();
            _conditionalIndexes = keyStorage.GetConditionalIndexInfos<T>();
        }

        private readonly ConcurrentDictionary<RelationInfo, CacheData<T>> _cacheData =
            new ConcurrentDictionary<RelationInfo, CacheData<T>>();



        public ICacheData<T> GetCacheData(RelationInfo relationInfo)
        {
            return _cacheData.GetOrAdd(relationInfo,
                r => new CacheData<T>(_cacheFactory, this, _conditionalIndexes, relationInfo));
        }

        public ICollection<CacheData<T>> AllCacheDatas => _cacheData.Values;

        public bool GetCacheExist(RelationInfo relationInfo)
        {
            CacheData<T> data;
            if (_cacheData.TryGetValue(relationInfo, out data))
            {
                return !data.Empty;
            }
            return false;
        }

        public bool GetIsCacheFullCollection(RelationInfo relationInfo)
        {
            CacheData<T> data;
            if (_cacheData.TryGetValue(relationInfo, out data))
            {
                return data.FullCollection;
            }
            return false;
        }

        public EntityKey GetPrimaryKeyValue(T entity)
        {
            return entity.GetPrimaryKeyValue(_primaryKeyInfo);
        }

        public EntityIndex GetIndexValue(T entity)
        {
            if (_indexInfo != null)
            {
                return entity.GetIndexValue(_indexInfo);
            }
            return null;
        }

        public EntityIndex GetConditionalIndexValue(T entity, EntityConditionalIndexInfo conditionalInfo)
        {
            return entity.GetConditionalIndexValue(conditionalInfo);
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