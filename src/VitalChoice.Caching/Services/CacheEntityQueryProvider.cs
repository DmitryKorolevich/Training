using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Data.Entity;
using Microsoft.Data.Entity.Query.Internal;
using VitalChoice.Caching.Interfaces;
using VitalChoice.Caching.Services.Cache;
using VitalChoice.Caching.Services.Cache.Base;
using VitalChoice.Ecommerce.Domain;
using VitalChoice.Ecommerce.Domain.Helpers;
using VitalChoice.ObjectMapping.Base;
using VitalChoice.ObjectMapping.Interfaces;

namespace VitalChoice.Caching.Services
{
    public class CacheEntityQueryProvider : EntityQueryProvider
    {
        private readonly IQueryCacheFactory _queryCacheFactory;
        private readonly IInternalEntityCacheFactory _cacheFactory;
        private readonly ITypeConverter _typeConverter;
        private readonly IModelConverterService _modelConverterService;
        private readonly DbContext _context;

        public CacheEntityQueryProvider(IQueryCompiler queryCompiler, IQueryCacheFactory queryCacheFactory,
            IInternalEntityCacheFactory cacheFactory, ITypeConverter typeConverter,
            IModelConverterService modelConverterService, DbContext context) : base(queryCompiler)
        {
            _queryCacheFactory = queryCacheFactory;
            _cacheFactory = cacheFactory;
            _typeConverter = typeConverter;
            _modelConverterService = modelConverterService;
            _context = context;
        }

        public override TResult Execute<TResult>(Expression expression)
        {
            var cacheObjectType = typeof (TResult);
            var elementType = typeof (TResult).TryGetElementType(typeof (IEnumerable<>));
            if (elementType != null)
            {
                cacheObjectType = elementType;
            }
            if (cacheObjectType.GetTypeInfo().IsSubclassOf(typeof (Entity)))
            {
                var cacheExecutor =
                    (ICacheExecutor)
                        Activator.CreateInstance(typeof (CacheExecutor<>).MakeGenericType(cacheObjectType), expression,
                            _context, _queryCacheFactory, _cacheFactory, _typeConverter, _modelConverterService);
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
                            return (TResult) cacheExecutor.Update((IEnumerable<object>) result);
                        }
                        cacheExecutor.Update(result);
                        return (TResult) result;
                }
            }
            return base.Execute<TResult>(expression);
        }

        public override IAsyncEnumerable<TResult> ExecuteAsync<TResult>(Expression expression)
        {
            var cacheObjectType = typeof (TResult);
            if (cacheObjectType.GetTypeInfo().IsSubclassOf(typeof (Entity)))
            {
                var cacheExecutor =
                    (ICacheExecutor)
                        Activator.CreateInstance(typeof (CacheExecutor<>).MakeGenericType(cacheObjectType), expression,
                            _context, _queryCacheFactory, _cacheFactory, _typeConverter, _modelConverterService);
                CacheGetResult cacheGetResult;
                var result = cacheExecutor.Execute(out cacheGetResult);
                switch (cacheGetResult)
                {
                    case CacheGetResult.Found:
                        return result.Cast<TResult>().ToAsyncEnumerable();
                    case CacheGetResult.Update:
                        var asyncResult = base.ExecuteAsync<TResult>(expression);
                        var results = asyncResult.ToList().GetAwaiter().GetResult();
                        return ((List<TResult>) cacheExecutor.Update(results.Cast<object>())).ToAsyncEnumerable();
                }
            }
            return base.ExecuteAsync<TResult>(expression);
        }

        public override Task<TResult> ExecuteAsync<TResult>(Expression expression, CancellationToken cancellationToken)
        {
            var cacheObjectType = typeof (TResult);
            if (cacheObjectType.GetTypeInfo().IsSubclassOf(typeof (Entity)))
            {
                var cacheExecutor =
                    (ICacheExecutor)
                        Activator.CreateInstance(typeof (CacheExecutor<>).MakeGenericType(cacheObjectType), expression,
                            _context, _queryCacheFactory, _cacheFactory, _typeConverter, _modelConverterService);
                CacheGetResult cacheGetResult;
                var result = cacheExecutor.ExecuteFirst(out cacheGetResult);
                switch (cacheGetResult)
                {
                    case CacheGetResult.Found:
                        return Task.FromResult((TResult) result);
                    case CacheGetResult.Update:
                        var asyncResult = base.ExecuteAsync<TResult>(expression, cancellationToken);
                        var results = asyncResult.GetAwaiter().GetResult();
                        cacheExecutor.Update(results);
                        return Task.FromResult(results);
                }
            }
            return base.ExecuteAsync<TResult>(expression, cancellationToken);
        }

        private interface ICacheExecutor
        {
            IEnumerable<object> Execute(out CacheGetResult cacheResult);
            object ExecuteFirst(out CacheGetResult cacheResult);
            void Update(object entity);
            object Update(IEnumerable<object> entities);
        }

        private class CacheExecutor<T> : ICacheExecutor
            where T : Entity, new()
        {
            private readonly DbContext _context;
            private readonly QueryCacheData<T> _queryData;
            private readonly IEntityCache<T> _cache;

            public CacheExecutor(Expression expression, DbContext context, IQueryCacheFactory queryCacheFactory,
                IInternalEntityCacheFactory cacheFactory, ITypeConverter typeConverter,
                IModelConverterService modelConverterService)
            {
                _cache = new EntityCache<T>(cacheFactory,
                    new DirectMapper<T>(typeConverter, modelConverterService));
                _context = context;
                var queryCache = queryCacheFactory.GetQueryCache<T>();
                _queryData = queryCache.GerOrAdd(expression);
            }

            public IEnumerable<object> Execute(out CacheGetResult cacheResult)
            {
                if (_queryData.IsEmpty)
                {
                    cacheResult = CacheGetResult.NotFound;
                    return null;
                }
                List<T> entities;
                cacheResult = _cache.TryGetCached(_queryData, _context, out entities);
                return entities;
            }

            public object ExecuteFirst(out CacheGetResult cacheResult)
            {
                if (_queryData.IsEmpty)
                {
                    cacheResult = CacheGetResult.NotFound;
                    return null;
                }
                T entity;
                cacheResult = _cache.TryGetCachedFirstOrDefault(_queryData, _context, out entity);
                return entity;
            }

            public void Update(object entity)
            {
                if (_queryData.IsEmpty)
                {
                    return;
                }
                _cache.Update(_queryData, (T) entity);
            }

            public object Update(IEnumerable<object> entities)
            {
                if (_queryData.IsEmpty)
                {
                    return null;
                }
                var result = entities.Cast<T>().ToList();
                _cache.Update(_queryData, result);
                return result;
            }
        }
    }
}