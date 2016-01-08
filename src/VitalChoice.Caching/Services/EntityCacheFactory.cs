using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VitalChoice.Caching.Services.Cache;
using VitalChoice.Ecommerce.Domain;

namespace VitalChoice.Caching.Services
{
    public interface IEntityCacheFactory
    {
        EntityCache<T> GetEntityCache<T>() 
            where T : Entity, new();
    }

    public class EntityCacheFactory : IEntityCacheFactory
    {
        public EntityCache<T> GetEntityCache<T>() where T : Entity, new()
        {
            
        }
    }
}