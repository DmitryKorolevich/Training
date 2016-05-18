using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
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
    public static class EntityFrameworkBuilderExtension
    {
        public static IServiceCollection AddEntityFrameworkCache(this IServiceCollection builder,
            IEnumerable<Type> contextTypes)
        {
            return AddEntityFrameworkCache<CacheSyncProvider>(builder, contextTypes);
        }

        public static IServiceCollection AddEntityFrameworkCache<TSyncProvider>(this IServiceCollection services,
            IEnumerable<Type> contextTypes)
            where TSyncProvider : class, ICacheSyncProvider
        {
            services.Replace(ServiceDescriptor.Scoped(typeof (IStateManager), typeof (CacheStateManager)));
            services.Replace(ServiceDescriptor.Scoped(typeof (IAsyncQueryProvider), typeof (CacheEntityQueryProvider)));
            services.AddSingleton<IQueryParserFactory, QueryParserFactory>();
            services.AddSingleton<IInternalEntityCacheFactory, InternalEntityCacheFactory>();
            services.AddSingleton<ICacheSyncProvider, TSyncProvider>();
            services.AddSingleton<IEntityInfoStorage, EntityInfoStorage>();
            services.AddSingleton<IContextTypeContainer>(sp => new ContextTypeContainer(contextTypes.ToArray()));
            return services;
        }

        public static IServiceCollection AddEntityFrameworkCache<TContext>(this IServiceCollection builder)
            where TContext: DbContext
        {
            return AddEntityFrameworkCache(builder, new[] {typeof (TContext)});
        }

        public static IServiceCollection AddEntityFrameworkCache<TContext1, TContext2>(this IServiceCollection builder)
            where TContext1 : DbContext
            where TContext2 : DbContext
        {
            return AddEntityFrameworkCache(builder, new[] { typeof(TContext1), typeof(TContext2) });
        }

        public static IServiceCollection AddEntityFrameworkCache<TContext1, TContext2, TContext3>(this IServiceCollection builder)
            where TContext1 : DbContext
            where TContext2 : DbContext
            where TContext3 : DbContext
        {
            return AddEntityFrameworkCache(builder, new[] { typeof(TContext1), typeof(TContext2), typeof(TContext3) });
        }

        public static IServiceCollection AddEntityFrameworkCache<TContext1, TContext2, TContext3, TContext4>(this IServiceCollection builder)
            where TContext1 : DbContext
            where TContext2 : DbContext
            where TContext3 : DbContext
            where TContext4 : DbContext
        {
            return AddEntityFrameworkCache(builder, new[] { typeof(TContext1), typeof(TContext2), typeof(TContext3), typeof(TContext4) });
        }
    }
}