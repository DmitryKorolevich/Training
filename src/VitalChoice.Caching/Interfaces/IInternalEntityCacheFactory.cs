using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VitalChoice.Caching.GC;
using VitalChoice.Ecommerce.Domain;

namespace VitalChoice.Caching.Interfaces
{
    public interface IInternalEntityCacheFactory: IEntityCollectorInfo
    {
        bool CacheExist(Type entityType);
        IInternalEntityCache GetCache(Type entityType);
        IInternalEntityCache<T> GetCache<T>();
    }
}
