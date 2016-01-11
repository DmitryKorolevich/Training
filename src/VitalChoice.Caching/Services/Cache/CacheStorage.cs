using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VitalChoice.Caching.Interfaces;
using VitalChoice.Caching.Relational;
using VitalChoice.Ecommerce.Domain;
using VitalChoice.ObjectMapping.Interfaces;

namespace VitalChoice.Caching.Services.Cache
{
    public sealed class CacheStorage<T> : ICacheKeysStorage<T>
    {
        private readonly IInternalEntityCacheFactory _cacheFactory;
        private readonly ITypeConverter _typeConverter;
        private readonly EntityPrimaryKeyInfo _primaryKeyInfo;
        private readonly EntityUniqueIndexInfo _indexInfo;
        private readonly ICollection<EntityConditionalIndexInfo> _conditionalIndexes;

        public CacheStorage(IInternalEntityInfoStorage keyStorage, IInternalEntityCacheFactory cacheFactory, ITypeConverter typeConverter)
        {
            _cacheFactory = cacheFactory;
            _typeConverter = typeConverter;
            _primaryKeyInfo = keyStorage.GetPrimaryKeyInfo<T>();
            _indexInfo = keyStorage.GetIndexInfo<T>();
            _conditionalIndexes = keyStorage.GetConditionalIndexInfos<T>();
        }

        private readonly ConcurrentDictionary<RelationInfo, CacheData<T>> _cacheData =
            new ConcurrentDictionary<RelationInfo, CacheData<T>>();



        public ICacheData<T> GetCacheData(RelationInfo relationInfo)
        {
            return _cacheData.GetOrAdd(relationInfo,
                r => new CacheData<T>(_cacheFactory, _typeConverter, this, _conditionalIndexes, relationInfo));
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
            var keyValues =
                _primaryKeyInfo.InfoCollection.Select(keyInfo => new EntityKeyValue(keyInfo, keyInfo.Property.GetClrValue(entity)));
            return new EntityKey(keyValues);
        }

        public EntityIndex GetIndexValue(T entity)
        {
            if (_indexInfo != null)
            {
                return
                    new EntityIndex(
                        _indexInfo.InfoCollection.Select(info => new EntityIndexValue(info, info.Property.GetClrValue(entity))));
            }
            return null;
        }

        public EntityIndex GetConditionalIndexValue(T entity, EntityConditionalIndexInfo conditionalInfo)
        {
            return
                new EntityIndex(
                    conditionalInfo.InfoCollection.Select(info => new EntityIndexValue(info, info.Property.GetClrValue(entity))));
        }
    }
}