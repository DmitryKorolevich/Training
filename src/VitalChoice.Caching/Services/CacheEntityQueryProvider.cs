using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Data.Entity;
using Microsoft.Data.Entity.Query.Internal;
using Microsoft.Extensions.Logging;
using VitalChoice.Caching.Interfaces;
using VitalChoice.Caching.Services.Cache;
using VitalChoice.Caching.Services.Cache.Base;
using VitalChoice.Ecommerce.Domain;
using VitalChoice.Ecommerce.Domain.Helpers;

namespace VitalChoice.Caching.Services
{
    public class CacheEntityQueryProvider : EntityQueryProvider
    {
        private readonly IQueryCacheFactory _queryCacheFactory;
        private readonly IInternalEntityCacheFactory _cacheFactory;
        private readonly DbContext _context;
        private readonly ILogger<CacheEntityQueryProvider> _logger;

        public CacheEntityQueryProvider(IQueryCompiler queryCompiler, IQueryCacheFactory queryCacheFactory,
            IInternalEntityCacheFactory cacheFactory, DbContext context, ILogger<CacheEntityQueryProvider> logger) : base(queryCompiler)
        {
            _queryCacheFactory = queryCacheFactory;
            _cacheFactory = cacheFactory;
            _context = context;
            _logger = logger;
        }

        public override TResult Execute<TResult>(Expression expression)
        {
            var cacheObjectType = typeof (TResult);
            var elementType = typeof (TResult).TryGetElementType(typeof (IEnumerable<>));
            if (elementType != null)
            {
                cacheObjectType = elementType;
            }
            if (_cacheFactory.CanCache(cacheObjectType))
            {
                //if (cacheObjectType.GetTypeInfo().IsSubclassOf(typeof (Entity)))
                //{
                var cacheExecutor =
                    (ICacheExecutor)
                        Activator.CreateInstance(typeof (CacheExecutor<>).MakeGenericType(cacheObjectType), expression,
                            _context, _queryCacheFactory, _cacheFactory, _logger);
                CacheGetResult cacheGetResult;
                var result = elementType != null
                    ? cacheExecutor.Execute(out cacheGetResult)
                    : cacheExecutor.ExecuteFirst(out cacheGetResult);
                switch (cacheGetResult)
                {
                    case CacheGetResult.Found:
                        return (TResult) result;
                    case CacheGetResult.Update:
                        result = base.Execute<TResult>(expression);
                        if (elementType != null)
                        {
                            cacheExecutor.UpdateList(result);
                        }
                        else
                        {
                            cacheExecutor.Update(result);
                        }
                        return (TResult) result;
                }
                //}
            }
            return base.Execute<TResult>(expression);
        }

        public override IAsyncEnumerable<TResult> ExecuteAsync<TResult>(Expression expression)
        {
            var cacheObjectType = typeof (TResult);
            if (_cacheFactory.CanCache(cacheObjectType))
            {
                //if (cacheObjectType.GetTypeInfo().IsSubclassOf(typeof (Entity)))
                //{
                var cacheExecutor =
                    (ICacheExecutor)
                        Activator.CreateInstance(typeof (CacheExecutor<>).MakeGenericType(cacheObjectType), expression,
                            _context, _queryCacheFactory, _cacheFactory, _logger);
                CacheGetResult cacheGetResult;
                var result = (List<TResult>) cacheExecutor.Execute(out cacheGetResult);
                switch (cacheGetResult)
                {
                    case CacheGetResult.Found:
                        return result.ToAsyncEnumerable();
                    case CacheGetResult.Update:
                        var asyncResult = base.ExecuteAsync<TResult>(expression);
                        var results = asyncResult.ToList().GetAwaiter().GetResult();
                        cacheExecutor.UpdateList(results);
                        return results.ToAsyncEnumerable();
                }
                //}
            }
            return base.ExecuteAsync<TResult>(expression);
        }

        public override async Task<TResult> ExecuteAsync<TResult>(Expression expression, CancellationToken cancellationToken)
        {
            var cacheObjectType = typeof (TResult);
            var elementType = typeof(TResult).TryGetElementType(typeof(IEnumerable<>));
            if (elementType != null)
            {
                cacheObjectType = elementType;
            }
            if (_cacheFactory.CanCache(cacheObjectType))
            {
                //if (cacheObjectType.GetTypeInfo().IsSubclassOf(typeof (Entity)))
                //{
                var cacheExecutor =
                    (ICacheExecutor)
                        Activator.CreateInstance(typeof (CacheExecutor<>).MakeGenericType(cacheObjectType), expression,
                            _context, _queryCacheFactory, _cacheFactory, _logger);
                CacheGetResult cacheGetResult;
                var result = elementType != null
                    ? cacheExecutor.Execute(out cacheGetResult)
                    : cacheExecutor.ExecuteFirst(out cacheGetResult);
                switch (cacheGetResult)
                {
                    case CacheGetResult.Found:
                        return (TResult) result;
                    case CacheGetResult.Update:
                        var results = await base.ExecuteAsync<TResult>(expression, cancellationToken);
                        if (elementType != null)
                        {
                            cacheExecutor.UpdateList(results);
                        }
                        else
                        {
                            cacheExecutor.Update(results);
                        }
                        return results;
                }
                //}
            }
            return await base.ExecuteAsync<TResult>(expression, cancellationToken);
        }

        private interface ICacheExecutor
        {
            object Execute(out CacheGetResult cacheResult);
            object ExecuteFirst(out CacheGetResult cacheResult);
            void Update(object entity);
            void UpdateList(object entities);
        }

        private struct CacheExecutor<T> : ICacheExecutor
            where T : class, new()
        {
            private readonly ILogger _logger;
            private readonly QueryData<T> _queryData;
            private readonly IEntityCache<T> _cache;

            public CacheExecutor(Expression expression, DbContext context, IQueryCacheFactory queryCacheFactory,
                IInternalEntityCacheFactory cacheFactory, ILogger logger)
            {
                _cache = new EntityCache<T>(cacheFactory, context);
                _logger = logger;
                var queryCache = queryCacheFactory.GetQueryCache<T>();
                _queryData = queryCache.GerOrAdd(expression);
            }

            public object Execute(out CacheGetResult cacheResult)
            {
                if (!_queryData.CanCollectionCache)
                {
                    cacheResult = CacheGetResult.NotFound;
                    return null;
                }
                try
                {
                    List<T> entities;
                    cacheResult = _cache.TryGetCached(_queryData, out entities);
                    return entities;
                }
                catch (Exception e)
                {
                    _logger.LogError(e.Message, e);
                    cacheResult = CacheGetResult.NotFound;
                    return null;
                }
            }

            public object ExecuteFirst(out CacheGetResult cacheResult)
            {
                if (!_queryData.CanCache)
                {
                    cacheResult = CacheGetResult.NotFound;
                    return null;
                }
                try
                {
                    T entity;
                    cacheResult = _cache.TryGetCachedFirstOrDefault(_queryData, out entity);
                    return entity;
                }
                catch (Exception e)
                {
                    _logger.LogError(e.Message, e);
                    cacheResult = CacheGetResult.NotFound;
                    return null;
                }
            }

            public void Update(object entity)
            {
                if (!_queryData.CanCache)
                {
                    return;
                }
                try
                {
                    _cache.Update(_queryData, (T) entity);
                }
                catch (Exception e)
                {
                    _logger.LogError(e.Message, e);
                }
            }

            public void UpdateList(object entities)
            {
                if (!_queryData.CanCollectionCache)
                {
                    return;
                }
                try
                {
                    _cache.Update(_queryData, (IEnumerable<T>)entities);
                }
                catch (Exception e)
                {
                    _logger.LogError(e.Message, e);
                }
            }
        }
    }
}