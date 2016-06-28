using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.EntityFrameworkCore.Query.Internal;
using Microsoft.Extensions.Logging;
using VitalChoice.Caching.Interfaces;
using VitalChoice.Caching.Iterators;
using VitalChoice.Caching.Services.Cache;
using VitalChoice.Caching.Services.Cache.Base;
using VitalChoice.Ecommerce.Domain;
using VitalChoice.Ecommerce.Domain.Helpers;

namespace VitalChoice.Caching.Services
{
    public class CacheEntityQueryProvider : SqlErrorTrapEntityQueryProvider
    {
        private readonly IQueryParserFactory _queryParserFactory;
        private readonly IInternalEntityCacheFactory _cacheFactory;
        private readonly DbContext _context;
        private readonly ILogger<CacheEntityQueryProvider> _logger;

        public CacheEntityQueryProvider(IQueryCompiler queryCompiler, IQueryParserFactory queryParserFactory,
            IInternalEntityCacheFactory cacheFactory, IEntityInfoStorage infoStorage,
            ILoggerFactory logger, ICurrentDbContext currentContext) : base(queryCompiler)
        {
            _queryParserFactory = queryParserFactory;
            _cacheFactory = cacheFactory;
            _context = currentContext.Context;
            _logger = logger.CreateLogger<CacheEntityQueryProvider>();
            infoStorage.Initialize(_context);
        }

        public override object Execute(Expression expression)
        {
            return Execute<object>(expression);
        }

        public override TResult Execute<TResult>(Expression expression)
        {
            var cacheObjectType = typeof(TResult);
            var elementType = typeof(TResult).TryGetElementType(typeof(IEnumerable<>));
            if (elementType != null)
            {
                cacheObjectType = elementType;
            }
            if (_cacheFactory.CanCache(cacheObjectType))
            {
                if (cacheObjectType == typeof(TResult))
                {
                    var cacheExecutor =
                        (ICacheExecutor)
                            Activator.CreateInstance(typeof(CacheExecutor<>).MakeGenericType(cacheObjectType), expression,
                                _context, _queryParserFactory, _logger);
                    if (cacheExecutor.ReparsedExpression != null)
                        return base.Execute<TResult>(cacheExecutor.ReparsedExpression);
                    CacheGetResult cacheGetResult;
                    var entity = (TResult) cacheExecutor.ExecuteFirst(out cacheGetResult);
                    switch (cacheGetResult)
                    {
                        case CacheGetResult.Found:
                            return entity;
                        case CacheGetResult.Update:
                            entity = base.Execute<TResult>(expression);
                            Task.Run(() =>
                            {
                                cacheExecutor.Update(entity);
                            }).ConfigureAwait(false);
                            return entity;
                    }
                }
                else
                {
                    var cacheExecutor =
                        (ICacheExecutor)
                            Activator.CreateInstance(typeof(CacheExecutor<>).MakeGenericType(cacheObjectType), expression,
                                _context, _queryParserFactory, _logger);
                    if (cacheExecutor.ReparsedExpression != null)
                        return base.Execute<TResult>(cacheExecutor.ReparsedExpression);

                    CacheGetResult cacheGetResult;
                    var entityList = cacheExecutor.Execute(out cacheGetResult);

                    switch (cacheGetResult)
                    {
                        case CacheGetResult.Found:
                            return (TResult) entityList;
                        case CacheGetResult.Update:
                            entityList = base.Execute<TResult>(expression);
                            return (TResult) cacheExecutor.UpdateList(entityList);
                    }
                }
            }
            return base.Execute<TResult>(expression);
        }

        public override IAsyncEnumerable<TResult> ExecuteAsync<TResult>(Expression expression)
        {
            if (_cacheFactory.CanCache(typeof(TResult)))
            {
                var cacheExecutor =
                    (ICacheExecutor)
                        Activator.CreateInstance(typeof(CacheExecutor<>).MakeGenericType(typeof(TResult)), expression,
                            _context, _queryParserFactory, _logger);
                if (cacheExecutor.ReparsedExpression != null)
                    return base.ExecuteAsync<TResult>(cacheExecutor.ReparsedExpression);
                CacheGetResult cacheGetResult;
                var result = (List<TResult>) cacheExecutor.Execute(out cacheGetResult);

                switch (cacheGetResult)
                {
                    case CacheGetResult.Found:
                        return result.ToAsyncEnumerable();
                    case CacheGetResult.Update:
                        var asyncResult = base.ExecuteAsync<TResult>(expression);
                        return cacheExecutor.UpdateListAsync(asyncResult);
                }
            }
            return base.ExecuteAsync<TResult>(expression);
        }

        public override async Task<TResult> ExecuteAsync<TResult>(Expression expression, CancellationToken cancellationToken)
        {
            var cacheObjectType = typeof(TResult);
            var elementType = typeof(TResult).TryGetElementType(typeof(IEnumerable<>));
            if (elementType != null)
            {
                cacheObjectType = elementType;
            }
            if (_cacheFactory.CanCache(cacheObjectType))
            {
                if (cacheObjectType == typeof(TResult))
                {
                    var cacheExecutor =
                        (ICacheExecutor)
                            Activator.CreateInstance(typeof(CacheExecutor<>).MakeGenericType(cacheObjectType), expression,
                                _context, _queryParserFactory, _logger);
                    if (cacheExecutor.ReparsedExpression != null)
                        return base.Execute<TResult>(cacheExecutor.ReparsedExpression);
                    CacheGetResult cacheGetResult;
                    var entity = cacheExecutor.ExecuteFirst(out cacheGetResult);
                    switch (cacheGetResult)
                    {
                        case CacheGetResult.Found:
                            return (TResult) entity;
                        case CacheGetResult.Update:
                            entity = await base.ExecuteAsync<TResult>(expression, cancellationToken);
#pragma warning disable 4014
                            Task.Run(() =>
#pragma warning restore 4014
                            {
                                cacheExecutor.Update(entity);
                            }, cancellationToken).ConfigureAwait(false);
                            return (TResult) entity;
                    }
                }
                else
                {
                    var cacheExecutor =
                        (ICacheExecutor)
                            Activator.CreateInstance(typeof(CacheExecutor<>).MakeGenericType(cacheObjectType), expression,
                                _context, _queryParserFactory, _logger);
                    if (cacheExecutor.ReparsedExpression != null)
                        return base.Execute<TResult>(cacheExecutor.ReparsedExpression);

                    CacheGetResult cacheGetResult;
                    var entityList = cacheExecutor.Execute(out cacheGetResult);

                    switch (cacheGetResult)
                    {
                        case CacheGetResult.Found:
                            return (TResult) entityList;
                        case CacheGetResult.Update:
                            entityList = await base.ExecuteAsync<TResult>(expression, cancellationToken);
                            return (TResult) cacheExecutor.UpdateList(entityList);
                    }
                }
            }
            return await base.ExecuteAsync<TResult>(expression, cancellationToken);
        }

        internal interface ICacheExecutor
        {
            Expression ReparsedExpression { get; }
            object Execute(out CacheGetResult cacheResult);
            object ExecuteFirst(out CacheGetResult cacheResult);
            bool Update(object entity);
            object UpdateList(object entities);

            IAsyncEnumerable<T> UpdateListAsync<T>(IAsyncEnumerable<T> entities);
        }

        internal interface ICacheExecutor<T> : ICacheExecutor
        {
            List<T> ExecuteTyped(out CacheGetResult cacheResult);
            T ExecuteTypedFirst(out CacheGetResult cacheResult);
            bool Update(T entity);
            bool UpdateList(IEnumerable<T> entities);
        }

        internal class CacheExecutor<T> : ICacheExecutor<T>
            where T : class
        {
            private readonly ILogger _logger;
            private readonly QueryData<T> _queryData;
            private readonly IRelationalCache<T> _cache;
            private readonly Expression _reparsedExpression;

            public CacheExecutor(Expression expression, DbContext context, IQueryParserFactory queryParserFactory, ILogger logger)
            {
                _logger = logger;
                var queryCache = queryParserFactory.GetQueryCache<T>();
                _cache = new RelationalCache<T>(queryCache.InternalCache, context, logger);
                _queryData = queryCache.ParseQuery(expression, context.Model, out _reparsedExpression);
            }

            public Expression ReparsedExpression => _reparsedExpression;

            public object Execute(out CacheGetResult cacheResult)
            {
                return ExecuteTyped(out cacheResult);
            }

            public object ExecuteFirst(out CacheGetResult cacheResult)
            {
                return ExecuteTypedFirst(out cacheResult);
            }

            public bool Update(object entity)
            {
                return Update((T) entity);
            }

            public object UpdateList(object entities)
            {
                return new CacheEnumerable<T>((IEnumerable<T>) entities, this);
            }

            public List<T> ExecuteTyped(out CacheGetResult cacheResult)
            {
                try
                {
                    List<T> entities;
                    cacheResult = _cache.TryGetCached(_queryData, out entities);
                    return entities;
                }
                catch (Exception e)
                {
                    _logger.LogError(e.ToString());
                    cacheResult = CacheGetResult.NotFound;
                    return null;
                }
            }

            public T ExecuteTypedFirst(out CacheGetResult cacheResult)
            {
                try
                {
                    T entity;
                    cacheResult = _cache.TryGetCachedFirstOrDefault(_queryData, out entity);
                    return entity;
                }
                catch (Exception e)
                {
                    _logger.LogError(e.ToString());
                    cacheResult = CacheGetResult.NotFound;
                    return default(T);
                }
            }

            public bool Update(T entity)
            {
                if (!_queryData.CanCache)
                {
                    return false;
                }
                try
                {
                    return _cache.Update(_queryData, entity);
                }
                catch (Exception e)
                {
                    _logger.LogError(e.ToString());
                }
                return false;
            }

            public bool UpdateList(IEnumerable<T> entities)
            {
                if (!_queryData.CanCollectionCache)
                {
                    return false;
                }
                try
                {
                    return _cache.Update(_queryData, entities);
                }
                catch (Exception e)
                {
                    _logger.LogError(e.ToString());
                }
                return false;
            }

            public IAsyncEnumerable<TT> UpdateListAsync<TT>(IAsyncEnumerable<TT> entities)
            {
                return (IAsyncEnumerable<TT>) new AsyncCacheEnumerable<T>((IAsyncEnumerable<T>) entities, this);
            }

            public IAsyncEnumerable<T> UpdateListAsync(IAsyncEnumerable<T> entities)
            {
                return new AsyncCacheEnumerable<T>(entities, this);
            }
        }
    }
}