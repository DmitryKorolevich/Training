using System;
using Microsoft.Extensions.DependencyInjection;
using VitalChoice.Caching.Interfaces;

namespace VitalChoice.Caching.Services.Cache.Base
{
    public class CacheServiceScopeFactoryContainer : ICacheServiceScopeFactoryContainer
    {
        public IServiceScopeFactory ScopeFactory { get; private set; }

        public void SetFactory(IServiceProvider serviceProvider)
        {
            ScopeFactory = serviceProvider.GetRequiredService<IServiceScopeFactory>();
        }
    }
}