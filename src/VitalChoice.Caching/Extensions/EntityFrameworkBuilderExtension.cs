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
        public static EntityFrameworkServicesBuilder AddEntityFrameworkCache<TInfoStorage>(this EntityFrameworkServicesBuilder builder)
            where TInfoStorage: class, IEntityInfoStorage
        {
            var services = builder.GetInfrastructure();
            services.Replace(ServiceDescriptor.Scoped(typeof(IStateManager), typeof(CacheStateManager)));
            services.Replace(ServiceDescriptor.Scoped(typeof(IAsyncQueryProvider), typeof(CacheEntityQueryProvider)));
            services.AddScoped<IQueryCacheFactory, QueryCacheFactory>();
            services.AddScoped<IInternalEntityCacheFactory, InternalEntityCacheFactory>();
            services.AddScoped<ICacheSyncProvider, CacheSyncProvider>();
            services.AddSingleton<IEntityInfoStorage, TInfoStorage>();
            return builder;
        }

        public static EntityFrameworkServicesBuilder AddEntityFrameworkCache<TSyncProvider, TInfoStorage>(this EntityFrameworkServicesBuilder builder)
            where TSyncProvider: class, ICacheSyncProvider
            where TInfoStorage : class, IEntityInfoStorage
        {
            var services = builder.GetInfrastructure();
            services.Replace(ServiceDescriptor.Scoped(typeof(IStateManager), typeof(CacheStateManager)));
            services.Replace(ServiceDescriptor.Scoped(typeof(IAsyncQueryProvider), typeof(CacheEntityQueryProvider)));
            services.AddScoped<IQueryCacheFactory, QueryCacheFactory>();
            services.AddScoped<IInternalEntityCacheFactory, InternalEntityCacheFactory>();
            services.AddScoped<ICacheSyncProvider, TSyncProvider>();
            services.AddSingleton<IEntityInfoStorage, TInfoStorage>();
            return builder;
        }
    }
}