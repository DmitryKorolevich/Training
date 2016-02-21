using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VitalChoice.Caching.Interfaces;

namespace VitalChoice.Caching.Services.Cache
{
    public class QueryCacheFactory : IQueryCacheFactory
    {
        private static readonly ConcurrentDictionary<Type, object> QueryCaches = new ConcurrentDictionary<Type, object>();

        private readonly IEntityInfoStorage _entityInfo;

        public QueryCacheFactory(IEntityInfoStorage entityInfo)
        {
            _entityInfo = entityInfo;
        }

        public IQueryCache<T> GetQueryCache<T>()
        {
            return (IQueryCache<T>) QueryCaches.GetOrAdd(typeof (T), key => new QueryCache<T>(_entityInfo));
        }
    }
}