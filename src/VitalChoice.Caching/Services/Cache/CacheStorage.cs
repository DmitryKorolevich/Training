using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VitalChoice.Caching.Interfaces;
using VitalChoice.Caching.Relational;
using VitalChoice.Ecommerce.Domain;

namespace VitalChoice.Caching.Services.Cache
{
    public sealed class CacheStorage<T>
    {
        private readonly EntityPrimaryKeyInfo _primaryKeyInfo;
        private readonly EntityUniqueIndexInfo _indexInfo;

        public CacheStorage(IInternalEntityInfoStorage keyStorage)
        {
            _primaryKeyInfo = keyStorage.GetPrimaryKeyInfo<T>();
            _indexInfo = keyStorage.GetIndexInfos<T>();
        }

        private readonly ConcurrentDictionary<RelationInfo, CacheData<T>> _cacheData =
            new ConcurrentDictionary<RelationInfo, CacheData<T>>();

        public EntityPrimaryKey GetPrimaryKeyValue(T entity)
        {
            var keyValues =
                _primaryKeyInfo.KeyInfo.Select(keyInfo => new EntityKeyValue(keyInfo, keyInfo.Property.GetClrValue(entity)));
            return new EntityPrimaryKey(keyValues);
        }

        public EntityUniqueIndex GetIndexValue(T entity)
        {
            return
                new EntityUniqueIndex(
                    _indexInfo.IndexInfoInternal.Values.Select(info => new EntityIndexValue(info, info.Property.GetClrValue(entity))));
        }

        public CacheData<T> GetCacheData(RelationInfo relationInfo)
        {
            return _cacheData.GetOrAdd(relationInfo, r => new CacheData<T>());
        }

        public ICollection<CacheData<T>> AllCacheDatas => _cacheData.Values;

        public bool GetCacheExist(RelationInfo relationInfo)
        {
            CacheData<T> data;
            if (_cacheData.TryGetValue(relationInfo, out data))
            {
                return data.EntityDictionary.Count > 0;
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
    }
}