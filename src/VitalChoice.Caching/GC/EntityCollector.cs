using System;
using System.Linq;
using System.Threading;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using VitalChoice.Caching.Extensions;
using VitalChoice.Caching.Interfaces;
using VitalChoice.Caching.Relational;
using VitalChoice.Ecommerce.Domain.Options;

namespace VitalChoice.Caching.GC
{
    public class EntityCollector : IEntityCollectorInfo, IDisposable
    {
        private readonly IEntityInfoStorage _entityInfoStorage;
        private readonly IInternalEntityCacheFactory _cacheFactory;
        private readonly ILogger _logger;
        private readonly TimeSpan _timeToLeave;
        private readonly TimeSpan _scanPeriod;
        private readonly long _maxSize;

        public EntityCollector(IEntityInfoStorage entityInfoStorage, IInternalEntityCacheFactory cacheFactory,
            IOptions<AppOptionsBase> options, ILogger logger)
        {
            _entityInfoStorage = entityInfoStorage;
            _cacheFactory = cacheFactory;
            _logger = logger;
            _maxSize = options.Value.CacheSettings.MaxProcessHeapsSizeBytes;
            _timeToLeave = TimeSpan.FromSeconds(options.Value.CacheSettings.CacheTimeToLeaveSeconds);
            _scanPeriod = TimeSpan.FromSeconds(options.Value.CacheSettings.CacheScanPeriodSeconds);
            new Thread(ProcessObjects).Start();
        }

        public bool CanAddUpCache()
        {
            return System.GC.GetTotalMemory(false) < _maxSize;
        }

        private readonly ManualResetEvent _disposingEvent = new ManualResetEvent(false);

        ~EntityCollector()
        {
            Dispose();
        }

        public void Dispose()
        {
            _disposingEvent.Set();
        }

        private void ProcessObjects()
        {
            while (!_disposingEvent.WaitOne(_scanPeriod))
            {
                try
                {
                    if (System.GC.GetTotalMemory(false) < _maxSize)
                    {
                        continue;
                    }
                    if (System.GC.GetTotalMemory(true) < _maxSize)
                    {
                        continue;
                    }
                    var now = DateTime.Now;
                    foreach (var entityType in _entityInfoStorage.TrackedTypes)
                    {
                        if (!_cacheFactory.CacheExist(entityType))
                            continue;

                        var internalCache = _cacheFactory.GetCache(entityType);
                        foreach (var cache in internalCache.GetAllCaches())
                        {
                            lock (cache)
                            {
                                if (cache.FullCollection)
                                {
                                    if (cache.GetAllUntyped().Any(e => now - e.LastUpdateTime < _timeToLeave))
                                    {
                                        foreach (
                                            var cached in cache.GetAllUntyped().Where(entity => now - entity.LastUpdateTime < _timeToLeave)
                                            )
                                        {
                                            using (cached.Lock())
                                            {
                                                var pk = internalCache.EntityInfo.PrimaryKey.GetPrimaryKeyValue(cached.EntityUntyped);
                                                internalCache.MarkForUpdate(pk);
                                            }
                                        }
                                        cache.Clear();
                                    }

                                }
                                else
                                {
                                    foreach (
                                        var cached in cache.GetAllUntyped().Where(entity => now - entity.LastUpdateTime < _timeToLeave)
                                        )
                                    {
                                        using (cached.Lock())
                                        {
                                            var pk = internalCache.EntityInfo.PrimaryKey.GetPrimaryKeyValue(cached.EntityUntyped);
                                            internalCache.MarkForUpdate(pk);
                                            cache.TryRemoveUntyped(
                                                internalCache.EntityInfo.PrimaryKey.GetPrimaryKeyValue(cached.EntityUntyped));
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                catch (Exception e)
                {
                    _logger.LogError(e.ToString());
                }
            }
        }
    }
}