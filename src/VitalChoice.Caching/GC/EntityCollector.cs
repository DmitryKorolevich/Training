using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.OptionsModel;
using VitalChoice.Caching.Interfaces;
using VitalChoice.Ecommerce.Domain.Options;

namespace VitalChoice.Caching.GC
{
    public class EntityCollector : IDisposable
    {
        private readonly IInternalEntityInfoStorage _entityInfoStorage;
        private readonly IInternalEntityCacheFactory _cacheFactory;
        private readonly TimeSpan _timeToLeave;
        private readonly TimeSpan _scanPeriod;
        private readonly ulong _maxSize;

        public EntityCollector(IInternalEntityInfoStorage entityInfoStorage, IInternalEntityCacheFactory cacheFactory,
            IOptions<AppOptionsBase> options)
        {
            _entityInfoStorage = entityInfoStorage;
            _cacheFactory = cacheFactory;
        }

        private void ProcessObjects()
        {
            while (!_disposingEvent.WaitOne(_scanPeriod))
            {
                var now = DateTime.Now;
                foreach (var entityType in _entityInfoStorage.TrackedTypes)
                {
                    var internalCache = _cacheFactory.GetCache(entityType);
                    foreach (var cache in internalCache.GetAllCaches())
                    {
                        if (cache.FullCollection)
                        {
                            if (cache.GetAllUntyped().Any(e => !e.NeedUpdate && now - e.LastAccessTime > _timeToLeave))
                            {
                                cache.Clear();
                            }
                        }
                        else
                        {
                            foreach (var entity in cache.GetAllUntyped())
                            {
                                if (!entity.NeedUpdate && now - entity.LastAccessTime > _timeToLeave)
                                {
                                    cache.TryRemove(internalCache.GetPrimaryKeyValue(entity.EntityUntyped));
                                }
                            }
                        }
                    }
                }
            }
        }

        private readonly ManualResetEvent _disposingEvent = new ManualResetEvent(false);

        public void Dispose()
        {
            _disposingEvent.Set();
        }
    }
}