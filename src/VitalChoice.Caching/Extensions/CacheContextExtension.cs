using System;
using Microsoft.EntityFrameworkCore.ChangeTracking.Internal;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Query.Internal;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using VitalChoice.Caching.Interfaces;
using VitalChoice.Caching.Services;
using VitalChoice.Caching.Services.Cache;
using VitalChoice.Caching.Services.Cache.Base;

namespace VitalChoice.Caching.Extensions
{
    public class CacheContextExtension<TSyncProvider> : IDbContextOptionsExtension
        where TSyncProvider : class, ICacheSyncProvider
    {
        private readonly ICacheServiceScopeFactoryContainer _serviceScopeFactoryContainer;
        private readonly IServiceCollection _services;

        public CacheContextExtension(ICacheServiceScopeFactoryContainer serviceScopeFactoryContainer,
            Action<IServiceCollection> serviceBuilder = null)
        {
            _serviceScopeFactoryContainer = serviceScopeFactoryContainer;
            _services = new ServiceCollection();
            serviceBuilder?.Invoke(_services);
        }

        public void ApplyServices(IServiceCollection services)
        {
            services.TryAddEnumerable(_services);
            services.Replace(ServiceDescriptor.Scoped<IStateManager, CacheStateManager>());
            services.Replace(ServiceDescriptor.Scoped<IAsyncQueryProvider, CacheEntityQueryProvider>());
            services.AddSingleton<IQueryParserFactory, QueryParserFactory>();
            services.AddSingleton<IInternalEntityCacheFactory, InternalEntityCacheFactory>();
            services.AddSingleton<ICacheSyncProvider, TSyncProvider>();
            services.AddSingleton<IEntityInfoStorage, EntityInfoStorage>();
            services.AddSingleton<IContextTypeContainer>(sp => new ContextTypeContainer());
            services.AddSingleton(_serviceScopeFactoryContainer);
        }
    }
}