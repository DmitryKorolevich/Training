using System.Collections.Generic;
using System.Linq;
using VitalChoice.Caching.Extensions;
using VitalChoice.Caching.Relational;
using VitalChoice.ObjectMapping.Extensions;

namespace VitalChoice.Caching.Services.Cache.Base
{
    public struct CacheResult<T>
    {
        public CacheResult(T entity, CacheGetResult result)
        {
            Entity = entity;
            Result = result;
        }

        public T Entity;

        public CacheGetResult Result;

        public static implicit operator T(CacheResult<T> result)
        {
            return result.Entity;
        }

        public static implicit operator CacheResult<T>(CachedEntity<T> cached)
        {
            using (cached.Lock())
            {
                var needUpdate = cached.NeedUpdate;
                if (needUpdate)
                {
                    return new CacheResult<T>(default(T), CacheGetResult.Update);
                }
                return new CacheResult<T>(DeepCloneItem(cached.Entity, cached.Cache.Relations), CacheGetResult.Found);
            }
        }

        public static implicit operator CacheResult<T>(CacheGetResult result)
        {
            return new CacheResult<T>(default(T), result);
        }

        internal static T DeepCloneItem(T item, RelationInfo relations)
        {
            return (T) item.DeepCloneItem(relations);
        }

        internal static IEnumerable<T> DeepCloneList(IEnumerable<T> entities, RelationInfo relations)
        {
            return entities.Select(item => DeepCloneItem(item, relations));
        }
    }
}