using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VitalChoice.Ecommerce.Domain;

namespace VitalChoice.Caching.Interfaces
{
    internal interface IInternalEntityCacheFactory
    {
        IInternalEntityCache GetCache(Type entityType);
        IInternalEntityCollectionCache GetCollectionCache(Type entityType);
        IInternalEntityCache<T> GetCache<T>() 
            where T : Entity;
        IInternalEntityCollectionCache<T> GetCollectionCache<T>() 
            where T : Entity;
    }
}
