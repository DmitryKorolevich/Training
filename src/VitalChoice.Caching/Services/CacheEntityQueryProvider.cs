using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query.Internal;
using Microsoft.Extensions.Logging;
using VitalChoice.Caching.Interfaces;
using VitalChoice.Caching.Iterators;
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
            IInternalEntityCacheFactory cacheFactory, IEntityInfoStorage infoStorage, DbContext context,
            ILogger<CacheEntityQueryProvider> logger) : base(queryCompiler)
        {
            _queryParserFactory = queryParserFactory;
            _cacheFactory = cacheFactory;
            _infoStorage = infoStorage;
            _context = context;
            _logger = logger;
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
                    var cacheExecutor = new CacheExecutor<TResult>(expression, _context, _infoStorage, _queryParserFactory, _logger);
                    if (cacheExecutor.ReparsedExpression != null)
                        return base.Execute<TResult>(cacheExecutor.ReparsedExpression);
                    CacheGetResult cacheGetResult;
                    var entity = cacheExecutor.ExecuteTypedFirst(out cacheGetResult);
                    switch (cacheGetResult)
                    {
                        case CacheGetResult.Found:
                            return entity;
                        case CacheGetResult.Update:
                            var updateScope = new ProfilingScope(new ExpressionStringFormatter(expression));

                            updateScope.AddScopeData(CacheGetResult.Update);
                            entity = base.Execute<TResult>(expression);
                            Task.Factory.StartNew(() =>
                            {
                                try
                                {
                                    updateScope.AddScopeData(cacheExecutor.Update(entity));
                                }
                                finally
                                {
                                    updateScope.Dispose();
                                }
                            });
                            return entity;
                    }
                }
                else
                {
                    var cacheExecutor =
                        (ICacheExecutor)
                            Activator.CreateInstance(typeof(CacheExecutor<>).MakeGenericType(cacheObjectType), expression,
                                _context, _infoStorage, _queryParserFactory, _logger);
                    if (cacheExecutor.ReparsedExpression != null)
                        return base.Execute<TResult>(cacheExecutor.ReparsedExpression);

                    CacheGetResult cacheGetResult;
                    var entityList = cacheExecutor.Execute(out cacheGetResult);

                    switch (cacheGetResult)
                    {
                        case CacheGetResult.Found:
                            return (TResult) entityList;
                        case CacheGetResult.Update:
                            var updateScope = new ProfilingScope(new ExpressionStringFormatter(expression));
                            updateScope.AddScopeData(CacheGetResult.Update);
                            entityList = base.Execute<TResult>(expression);
                            return (TResult) cacheExecutor.UpdateList(entityList, updateScope);
                    }
                }
            }
            using (var scope = new ProfilingScope(new ExpressionStringFormatter(expression)))
            {
                scope.AddScopeData(CacheGetResult.NotFound);
                return base.Execute<TResult>(expression);
            }
        }

        public override IAsyncEnumerable<TResult> ExecuteAsync<TResult>(Expression expression)
        {
            if (_cacheFactory.CanCache(typeof(TResult)))
            {
                var cacheExecutor = new CacheExecutor<TResult>(expression, _context, _infoStorage, _queryParserFactory, _logger);
                if (cacheExecutor.ReparsedExpression != null)
                    return base.ExecuteAsync<TResult>(cacheExecutor.ReparsedExpression);
                CacheGetResult cacheGetResult;
                var result = cacheExecutor.ExecuteTyped(out cacheGetResult);

                switch (cacheGetResult)
                {
                    case CacheGetResult.Found:
                        return result.ToAsyncEnumerable();
                    case CacheGetResult.Update:
                        var updateScope = new ProfilingScope(new ExpressionStringFormatter(expression));
                        updateScope.AddScopeData(CacheGetResult.Update);
                        var asyncResult = base.ExecuteAsync<TResult>(expression);
                        return cacheExecutor.UpdateListAsync(asyncResult, updateScope);
                }
            }
            using (var scope = new ProfilingScope(new ExpressionStringFormatter(expression)))
            {
                scope.AddScopeData(CacheGetResult.NotFound);
                return base.ExecuteAsync<TResult>(expression);
            }
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
                    var cacheExecutor = new CacheExecutor<TResult>(expression, _context, _infoStorage, _queryParserFactory, _logger);
                    if (cacheExecutor.ReparsedExpression != null)
                        return base.Execute<TResult>(cacheExecutor.ReparsedExpression);
                    CacheGetResult cacheGetResult;
                    var entity = cacheExecutor.ExecuteTypedFirst(out cacheGetResult);
                    switch (cacheGetResult)
                    {
                        case CacheGetResult.Found:
                            return entity;
                        case CacheGetResult.Update:
                            var updateScope = new ProfilingScope(new ExpressionStringFormatter(expression));
                            updateScope.AddScopeData(CacheGetResult.Update);
                            entity = await base.ExecuteAsync<TResult>(expression, cancellationToken);
#pragma warning disable 4014
                            Task.Factory.StartNew(() =>
#pragma warning restore 4014
                            {
                                try
                                {
                                    updateScope.AddScopeData(cacheExecutor.Update(entity));
                                }
                                finally
                                {
                                    updateScope.Dispose();
                                }
                            }, cancellationToken);
                            return entity;
                    }
                }
                else
                {
                    var cacheExecutor =
                        (ICacheExecutor)
                            Activator.CreateInstance(typeof(CacheExecutor<>).MakeGenericType(cacheObjectType), expression,
                                _context, _infoStorage, _queryParserFactory, _logger);
                    if (cacheExecutor.ReparsedExpression != null)
                        return base.Execute<TResult>(cacheExecutor.ReparsedExpression);

                    CacheGetResult cacheGetResult;
                    var entityList = cacheExecutor.Execute(out cacheGetResult);

                    switch (cacheGetResult)
                    {
                        case CacheGetResult.Found:
                            return (TResult) entityList;
                        case CacheGetResult.Update:
                            var updateScope = new ProfilingScope(new ExpressionStringFormatter(expression));
                            try
                            {
                                updateScope.AddScopeData(CacheGetResult.Update);
                                entityList = await base.ExecuteAsync<TResult>(expression, cancellationToken);
                            }
                            catch
                            {
                                updateScope.Dispose();
                            }
                            return (TResult) cacheExecutor.UpdateList(entityList, updateScope);
                    }
                }
            }
            using (var scope = new ProfilingScope(new ExpressionStringFormatter(expression)))
            {
                scope.AddScopeData(CacheGetResult.NotFound);
                return await base.ExecuteAsync<TResult>(expression, cancellationToken);
            }
        }

        internal interface ICacheExecutor
        {
            Expression ReparsedExpression { get; }
            object Execute(out CacheGetResult cacheResult);
            object ExecuteFirst(out CacheGetResult cacheResult);
            bool Update(object entity);
            object UpdateList(object entities, ProfilingScope scope = null);
        }

        internal interface ICacheExecutor<T> : ICacheExecutor
        {
            List<T> ExecuteTyped(out CacheGetResult cacheResult);
            T ExecuteTypedFirst(out CacheGetResult cacheResult);
            bool Update(T entity);
            bool UpdateList(IEnumerable<T> entities);
        }

        internal class CacheExecutor<T> : ICacheExecutor<T>
        {
            private readonly ILogger _logger;
            private readonly QueryData<T> _queryData;
            private readonly IEntityCache<T> _cache;
            private readonly Expression _reparsedExpression;

            // ReSharper disable once UnusedMember.Local
            public CacheExecutor(Expression expression, DbContext context, IEntityInfoStorage infoStorage,
                IQueryParserFactory queryParserFactory, ILogger logger)
            {
                _logger = logger;
                var queryCache = queryParserFactory.GetQueryCache<T>();
                _cache = new EntityCache<T>(queryCache.InternalEntityCache, infoStorage, context, logger);
                _queryData = queryCache.ParseQuery(expression, out _reparsedExpression);
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

            public object UpdateList(object entities, ProfilingScope scope = null)
            {
                var list = entities as List<T>;
                if (list != null)
                {
                    try
                    {
                        var result = UpdateList(list);
                        scope?.AddScopeData(result);
                    }
                    finally
                    {
                        scope?.Dispose();
                    }
                    return entities;
                }
                return new CacheEnumerable<T>((IEnumerable<T>) entities, this, scope);
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
                    _logger.LogError(e.Message, e);
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
                    _logger.LogError(e.Message, e);
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
                    using (new ProfilingScope("Cache Update"))
                    {
                        return _cache.Update(_queryData, entity);
                    }
                }
                catch (Exception e)
                {
                    _logger.LogError(e.Message, e);
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
                    using (new ProfilingScope("Cache Update"))
                    {
                        return _cache.Update(_queryData, entities);
                    }
                }
                catch (Exception e)
                {
                    _logger.LogError(e.Message, e);
                }
                return false;
            }

            public IAsyncEnumerable<T> UpdateListAsync(IAsyncEnumerable<T> entities, ProfilingScope scope = null)
            {
                return new AsyncCacheEnumerable<T>(entities, this, scope);
            }
        }
    }
}