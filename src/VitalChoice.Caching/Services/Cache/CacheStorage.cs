using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using VitalChoice.Caching.Interfaces;
using VitalChoice.Caching.Relational;
using VitalChoice.Caching.Services.Cache.Base;

namespace VitalChoice.Caching.Services.Cache
{
    public sealed class CacheStorage<T> : IDisposable
    {
        private readonly IInternalEntityCacheFactory _cacheFactory;
        private readonly ILoggerFactory _loggerFactory;
        private readonly EntityInfo _entityInfo;

        public CacheStorage(EntityInfo entityInfo, IInternalEntityCacheFactory cacheFactory, ILoggerFactory loggerFactory)
        {
            _cacheFactory = cacheFactory;
            _loggerFactory = loggerFactory;
            _entityInfo = entityInfo;
        }

        private readonly ConcurrentDictionary<RelationInfo, ICacheData<T>> _cacheData =
            new ConcurrentDictionary<RelationInfo, ICacheData<T>>();

        public ICacheData<T> GetCacheData(RelationInfo relationInfo)
        {
            return _cacheData.GetOrAdd(relationInfo,
                r => new CacheData<T>(_cacheFactory, _entityInfo, relationInfo, _loggerFactory));
        }

        public ICollection<ICacheData<T>> AllCacheDatas => _cacheData.Values;

        public bool GetCacheExist(RelationInfo relationInfo)
        {
            return _cacheData.ContainsKey(relationInfo);
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