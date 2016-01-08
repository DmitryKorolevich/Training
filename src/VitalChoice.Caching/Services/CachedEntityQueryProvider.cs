using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Data.Entity.Query.Internal;
using VitalChoice.Caching.Interfaces;
using VitalChoice.Caching.Services.Cache;
using VitalChoice.ObjectMapping.Base;
using VitalChoice.ObjectMapping.Interfaces;

namespace VitalChoice.Caching.Services
{
    public class CachedEntityQueryProvider : EntityQueryProvider
    {
        private readonly IQueryCacheFactory _queryCacheFactory;
        private readonly IInternalEntityCacheFactory _cacheFactory;
        private readonly ITypeConverter _typeConverter;
        private readonly IModelConverterService _modelConverterService;

        public CachedEntityQueryProvider(IQueryCompiler queryCompiler, IQueryCacheFactory queryCacheFactory,
            IInternalEntityCacheFactory cacheFactory, ITypeConverter typeConverter, IModelConverterService modelConverterService) : base(queryCompiler)
        {
            _queryCacheFactory = queryCacheFactory;
            _cacheFactory = cacheFactory;
            _typeConverter = typeConverter;
            _modelConverterService = modelConverterService;
        }

        public override TResult Execute<TResult>(Expression expression)
        {
            var queryCache = _queryCacheFactory.GetQueryCache<TResult>();
            var queryData = queryCache.GerOrAdd(expression);
            var cache = new EntityCache<TResult>(_cacheFactory, new DirectMapper<TResult>(_typeConverter, _modelConverterService));
        }

        public override IAsyncEnumerable<TResult> ExecuteAsync<TResult>(Expression expression)
        {
            return base.ExecuteAsync<TResult>(expression);
        }

        public override Task<TResult> ExecuteAsync<TResult>(Expression expression, CancellationToken cancellationToken)
        {
            return base.ExecuteAsync<TResult>(expression, cancellationToken);
        }
    }
}
