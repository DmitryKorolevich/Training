using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VitalChoice.Caching.Interfaces;

namespace VitalChoice.Caching.Services.Cache
{
    public class QueryParserFactory : IQueryParserFactory
    {
        private readonly ConcurrentDictionary<Type, object> _queryCaches = new ConcurrentDictionary<Type, object>();

        private readonly IEntityInfoStorage _entityInfo;
        private readonly IInternalEntityCacheFactory _cacheFactory;

        public QueryParserFactory(IEntityInfoStorage entityInfo, IInternalEntityCacheFactory cacheFactory)
        {
            _entityInfo = entityInfo;
            _cacheFactory = cacheFactory;
        }

        public IQueryParser<T> GetQueryParser<T>()
        {
            return (IQueryParser<T>) _queryCaches.GetOrAdd(typeof (T), key => new QueryParser<T>(_entityInfo, _cacheFactory.GetCache<T>()));
        }
    }
}