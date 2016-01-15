using Autofac;
using Microsoft.Data.Entity.ChangeTracking.Internal;
using Microsoft.Data.Entity.Infrastructure;
using Microsoft.Data.Entity.Query.Internal;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using VitalChoice.Caching.Interfaces;
using VitalChoice.Caching.Services;
using VitalChoice.Caching.Services.Cache;

namespace VitalChoice.Caching.Extensions
{
    public static class EntityFrameworkBuilderExtension
    {
        public static EntityFrameworkServicesBuilder AddEntityFrameworkCache(this EntityFrameworkServicesBuilder builder)
        {
            var services = builder.GetInfrastructure();
            services.Replace(ServiceDescriptor.Scoped(typeof(IStateManager), typeof(CacheStateManager)));
            services.Replace(ServiceDescriptor.Scoped(typeof(IAsyncQueryProvider), typeof(CacheEntityQueryProvider)));
            services.AddScoped<IQueryCacheFactory, QueryCacheFactory>();
            services.AddScoped<IInternalEntityCacheFactory, InternalEntityCacheFactory>();
            services.AddScoped<IInternalEntityInfoStorage, InternalEntityInfoStorage>();
            services.AddScoped<ICacheSyncProvider, CacheSyncProvider>();
            return builder;
        }

        public static EntityFrameworkServicesBuilder AddEntityFrameworkCache<TSyncProvider>(this EntityFrameworkServicesBuilder builder)
            where TSyncProvider: class, ICacheSyncProvider
        {
            var services = builder.GetInfrastructure();
            services.Replace(ServiceDescriptor.Scoped(typeof(IStateManager), typeof(CacheStateManager)));
            services.Replace(ServiceDescriptor.Scoped(typeof(IAsyncQueryProvider), typeof(CacheEntityQueryProvider)));
            services.AddScoped<IQueryCacheFactory, QueryCacheFactory>();
            services.AddScoped<IInternalEntityCacheFactory, InternalEntityCacheFactory>();
            services.AddScoped<IInternalEntityInfoStorage, InternalEntityInfoStorage>();
            services.AddScoped<ICacheSyncProvider, TSyncProvider>();
            return builder;
        }
    }
}