using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VitalChoice.Caching.GC;
using VitalChoice.Ecommerce.Domain;

namespace VitalChoice.Caching.Interfaces
{
    public interface IInternalEntityCacheFactory
    {
        bool CanCache(Type entityType);
        bool CacheExist(Type entityType);
        IInternalEntityCache GetCache(Type entityType);
        bool CanCache(string entityType);
        IInternalEntityCache GetCache(string entityType);
        IInternalCache<T> GetCache<T>();
    }
}
