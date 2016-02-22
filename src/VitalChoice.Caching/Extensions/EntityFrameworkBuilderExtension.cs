using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Data.Entity;
using Microsoft.Data.Entity.ChangeTracking.Internal;
using Microsoft.Data.Entity.Infrastructure;
using Microsoft.Data.Entity.Query.Internal;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using VitalChoice.Caching.Interfaces;
using VitalChoice.Caching.Services;
using VitalChoice.Caching.Services.Cache;
using VitalChoice.Caching.Services.Cache.Base;

namespace VitalChoice.Caching.Extensions
{
    public static class EntityFrameworkBuilderExtension
    {
        public static EntityFrameworkServicesBuilder AddEntityFrameworkCache(this EntityFrameworkServicesBuilder builder,
            IEnumerable<Type> contextTypes)
        {
            return AddEntityFrameworkCache<CacheSyncProvider>(builder, contextTypes);
        }

        public static EntityFrameworkServicesBuilder AddEntityFrameworkCache<TSyncProvider>(this EntityFrameworkServicesBuilder builder,
            IEnumerable<Type> contextTypes)
            where TSyncProvider : class, ICacheSyncProvider
        {
            var services = builder.GetInfrastructure();
            services.Replace(ServiceDescriptor.Scoped(typeof (IStateManager), typeof (CacheStateManager)));
            services.Replace(ServiceDescriptor.Scoped(typeof (IAsyncQueryProvider), typeof (CacheEntityQueryProvider)));
            services.AddScoped<IQueryCacheFactory, QueryCacheFactory>();
            services.AddScoped<IInternalEntityCacheFactory, InternalEntityCacheFactory>();
            services.AddScoped<ICacheSyncProvider, TSyncProvider>();
            services.AddSingleton<IEntityInfoStorage, EntityInfoStorage>();
            services.AddInstance(typeof (IContextTypeContainer), new ContextTypeContainer(contextTypes.ToArray()));
            return builder;
        }

        public static EntityFrameworkServicesBuilder AddEntityFrameworkCache<TContext>(this EntityFrameworkServicesBuilder builder)
            where TContext: DbContext
        {
            return AddEntityFrameworkCache(builder, new[] {typeof (TContext)});
        }

        public static EntityFrameworkServicesBuilder AddEntityFrameworkCache<TContext1, TContext2>(this EntityFrameworkServicesBuilder builder)
            where TContext1 : DbContext
            where TContext2 : DbContext
        {
            return AddEntityFrameworkCache(builder, new[] { typeof(TContext1), typeof(TContext2) });
        }

        public static EntityFrameworkServicesBuilder AddEntityFrameworkCache<TContext1, TContext2, TContext3>(this EntityFrameworkServicesBuilder builder)
            where TContext1 : DbContext
            where TContext2 : DbContext
            where TContext3 : DbContext
        {
            return AddEntityFrameworkCache(builder, new[] { typeof(TContext1), typeof(TContext2), typeof(TContext3) });
        }

        public static EntityFrameworkServicesBuilder AddEntityFrameworkCache<TContext1, TContext2, TContext3, TContext4>(this EntityFrameworkServicesBuilder builder)
            where TContext1 : DbContext
            where TContext2 : DbContext
            where TContext3 : DbContext
            where TContext4 : DbContext
        {
            return AddEntityFrameworkCache(builder, new[] { typeof(TContext1), typeof(TContext2), typeof(TContext3), typeof(TContext4) });
        }
    }
}