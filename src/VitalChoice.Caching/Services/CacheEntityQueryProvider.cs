using System;
using System.Collections;
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
using VitalChoice.Profiling;
using VitalChoice.Profiling.Base;

namespace VitalChoice.Caching.Services
{
    public class CacheEntityQueryProvider : EntityQueryProvider
    {
        private readonly IQueryParserFactory _queryParserFactory;
        private readonly IInternalEntityCacheFactory _cacheFactory;
        private readonly IEntityInfoStorage _infoStorage;
        private readonly DbContext _context;
        private readonly ILogger<CacheEntityQueryProvider> _logger;

        public CacheEntityQueryProvider(IQueryCompiler queryCompiler, IQueryParserFactory queryParserFactory,
            IInternalEntityCacheFactory cacheFactory, IEntityInfoStorage infoStorage, DbContext context, ILogger<CacheEntityQueryProvider> logger) : base(queryCompiler)
        {
            _queryParserFactory = queryParserFactory;
            _cacheFactory = cacheFactory;
            _infoStorage = infoStorage;
            _context = context;
            _logger = logger;
        }

        public override TResult Execute<TResult>(Expression expression)
        {
            using (new ProfilingScope(new ExpressionStringFormatter(expression)))
            {
                var cacheObjectType = typeof (TResult);
                var elementType = typeof (TResult).TryGetElementType(typeof (IEnumerable<>));
                if (elementType != null)
                {
                    cacheObjectType = elementType;
                }
                if (_cacheFactory.CanCache(cacheObjectType))
                {
                    var cacheExecutor =
                        (ICacheExecutor)
                            Activator.CreateInstance(typeof (CacheExecutor<>).MakeGenericType(cacheObjectType), expression,
                                _context, _infoStorage, _queryParserFactory, _logger);
                    if (cacheExecutor.ReparsedExpression != null)
                        return base.Execute<TResult>(cacheExecutor.ReparsedExpression);
                    CacheGetResult cacheGetResult;
                    var results = elementType != null
                        ? cacheExecutor.Execute(out cacheGetResult)
                        : cacheExecutor.ExecuteFirst(out cacheGetResult);
                    if (cacheGetResult != CacheGetResult.NotFound)
                    {
                        using (var updateScope = new ProfilingScope(cacheGetResult))
                        {
                            switch (cacheGetResult)
                            {
                                case CacheGetResult.Found:
                                    return (TResult) results;
                                case CacheGetResult.Update:
                                    results = base.Execute<TResult>(expression);

                                    updateScope.AddScopeData(elementType != null
                                        ? cacheExecutor.UpdateList(results)
                                        : cacheExecutor.Update(results));

                                    return (TResult) results;
                            }
                        }
                    }
                }
                using (new ProfilingScope(CacheGetResult.NotFound))
                {
                    return base.Execute<TResult>(expression);
                }
            }
        }

        public override IAsyncEnumerable<TResult> ExecuteAsync<TResult>(Expression expression)
        {
            using (new ProfilingScope(new ExpressionStringFormatter(expression)))
            {
                var cacheObjectType = typeof (TResult);
                if (_cacheFactory.CanCache(cacheObjectType))
                {
                    var cacheExecutor =
                        (ICacheExecutor)
                            Activator.CreateInstance(typeof (CacheExecutor<>).MakeGenericType(cacheObjectType), expression,
                                _context, _infoStorage, _queryParserFactory, _logger);
                    if (cacheExecutor.ReparsedExpression != null)
                        return base.ExecuteAsync<TResult>(cacheExecutor.ReparsedExpression);
                    CacheGetResult cacheGetResult;
                    var result = (List<TResult>) cacheExecutor.Execute(out cacheGetResult);
                    if (cacheGetResult != CacheGetResult.NotFound)
                    {
                        using (var updateScope = new ProfilingScope(cacheGetResult))
                        {
                            switch (cacheGetResult)
                            {
                                case CacheGetResult.Found:
                                    return result.ToAsyncEnumerable();
                                case CacheGetResult.Update:
                                    var asyncResult = base.ExecuteAsync<TResult>(expression);
                                    var results = asyncResult.ToList().GetAwaiter().GetResult();
                                    updateScope.AddScopeData(cacheExecutor.UpdateList(results));
                                    return results.ToAsyncEnumerable();
                            }
                        }
                    }
                }
                using (new ProfilingScope(CacheGetResult.NotFound))
                {
                    return base.ExecuteAsync<TResult>(expression).ToList().GetAwaiter().GetResult().ToAsyncEnumerable();
                }
            }
        }

        public override async Task<TResult> ExecuteAsync<TResult>(Expression expression, CancellationToken cancellationToken)
        {
            using (new ProfilingScope(new ExpressionStringFormatter(expression)))
            {
                var cacheObjectType = typeof (TResult);
                var elementType = typeof (TResult).TryGetElementType(typeof (IEnumerable<>));
                if (elementType != null)
                {
                    cacheObjectType = elementType;
                }
                if (_cacheFactory.CanCache(cacheObjectType))
                {
                    var cacheExecutor =
                        (ICacheExecutor)
                            Activator.CreateInstance(typeof (CacheExecutor<>).MakeGenericType(cacheObjectType), expression,
                                _context, _infoStorage, _queryParserFactory, _logger);
                    if (cacheExecutor.ReparsedExpression != null)
                        return await base.ExecuteAsync<TResult>(cacheExecutor.ReparsedExpression, cancellationToken);
                    CacheGetResult cacheGetResult;
                    var result = elementType != null
                        ? cacheExecutor.Execute(out cacheGetResult)
                        : cacheExecutor.ExecuteFirst(out cacheGetResult);
                    if (cacheGetResult != CacheGetResult.NotFound)
                    {
                        using (var updateScope = new ProfilingScope(cacheGetResult))
                        {
                            switch (cacheGetResult)
                            {
                                case CacheGetResult.Found:
                                    return (TResult) result;
                                case CacheGetResult.Update:
                                    var results = (object) await base.ExecuteAsync<TResult>(expression, cancellationToken);

                                    updateScope.AddScopeData(elementType != null
                                        ? cacheExecutor.UpdateList(results)
                                        : cacheExecutor.Update(results));
                                    return (TResult) results;
                            }
                        }
                    }
                }
                using (new ProfilingScope(CacheGetResult.NotFound))
                {
                    return await base.ExecuteAsync<TResult>(expression, cancellationToken);
                }
            }
        }

        private interface ICacheExecutor
        {
            Expression ReparsedExpression { get; }
            object Execute(out CacheGetResult cacheResult);
            object ExecuteFirst(out CacheGetResult cacheResult);
            bool Update(object entity);
            bool UpdateList(object entities);
        }

        private struct CacheExecutor<T> : ICacheExecutor
            where T : class, new()
        {
            private readonly ILogger _logger;
            private readonly QueryData<T> _queryData;
            private readonly IEntityCache<T> _cache;
            private readonly Expression _reparsedExpression;

            // ReSharper disable once UnusedMember.Local
            public CacheExecutor(Expression expression, DbContext context, IEntityInfoStorage infoStorage, IQueryParserFactory queryParserFactory, ILogger logger)
            {
                _logger = logger;
                var queryCache = queryParserFactory.GetQueryCache<T>();
                _cache = new EntityCache<T>(queryCache.InternalEntityCache, infoStorage, context, logger);
                _queryData = queryCache.ParseQuery(expression, out _reparsedExpression);
            }

            public Expression ReparsedExpression => _reparsedExpression;

            public object Execute(out CacheGetResult cacheResult)
            {
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

            public bool Update(object entity)
            {
                if (!_queryData.CanCache)
                {
                    return false;
                }
                try
                {
                    using (new ProfilingScope("Cache Update"))
                    {
                        return _cache.Update(_queryData, (T) entity);
                    }
                }
                catch (Exception e)
                {
                    _logger.LogError(e.Message, e);
                }
                return false;
            }

            public bool UpdateList(object entities)
            {
                if (!_queryData.CanCollectionCache)
                {
                    return false;
                }
                try
                {
                    using (new ProfilingScope("Cache Update"))
                    {
                        return _cache.Update(_queryData, (IEnumerable<T>) entities);
                    }
                }
                catch (Exception e)
                {
                    _logger.LogError(e.Message, e);
                }
                return false;
            }
        }
    }
}