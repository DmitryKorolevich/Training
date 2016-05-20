using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using VitalChoice.Caching.Interfaces;

namespace VitalChoice.Caching.Extensions
{
    public static class EntityFrameworkBuilderExtension
    {
        public static DbContextOptionsBuilder UseCache<TSyncProvider>(this DbContextOptionsBuilder optionsBuilder, ICacheServiceScopeFactoryContainer serviceScopeFactoryContainer,
            Action<IServiceCollection> serviceBuilder = null)
            where TSyncProvider : class, ICacheSyncProvider
        {
            ((IDbContextOptionsBuilderInfrastructure) optionsBuilder).AddOrUpdateExtension(
                new CacheContextExtension<TSyncProvider>(serviceScopeFactoryContainer, serviceBuilder));
            return optionsBuilder;
        }
    }
}