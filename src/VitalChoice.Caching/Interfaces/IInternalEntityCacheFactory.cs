using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VitalChoice.Ecommerce.Domain;

namespace VitalChoice.Caching.Interfaces
{
    public interface IInternalEntityCacheFactory
    {
        IInternalEntityCache GetCache(Type entityType);
        IInternalEntityCache<T> GetCache<T>();
    }
}
