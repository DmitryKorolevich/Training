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
        private readonly EntityUniqueIndexInfo[] _indexes;

        public CacheStorage(IInternalEntityInfoStorage keyStorage)
        {
            _primaryKeyInfo = keyStorage.GetPrimaryKeyInfo<T>();
            _indexes = keyStorage.GetIndexInfos<T>();
        }

        private readonly ConcurrentDictionary<RelationInfo, CacheData<T>> _cacheData =
            new ConcurrentDictionary<RelationInfo, CacheData<T>>();

        public EntityPrimaryKey GetPrimaryKeyValue(T entity)
        {
            var keyValues =
                _primaryKeyInfo.KeyInfo.Select(keyInfo => new EntityKeyValue(keyInfo, keyInfo.Property.GetClrValue(entity)));
            return new EntityPrimaryKey(keyValues);
        }

        public IEnumerable<EntityUniqueIndex> GetIndexValues(T entity)
        {
            return
                _indexes.Select(u => u.IndexInfo)
                    .Select(indexInfo => indexInfo.Select(info => new EntityIndexValue(info, info.Property.GetClrValue(entity))))
                    .Select(indexValues => new EntityUniqueIndex(indexValues));
        }

        public CacheData<T> GetCacheData(RelationInfo relationInfo)
        {
            return _cacheData.GetOrAdd(relationInfo, r => new CacheData<T>(_indexes));
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