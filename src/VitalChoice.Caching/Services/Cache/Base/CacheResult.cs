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

        public CacheResult(CachedEntity<T> cached, bool track)
        {
            using (cached.Lock())
            {
                var needUpdate = cached.NeedUpdate;
                if (needUpdate)
                {
                    Result = CacheGetResult.Update;
                    Entity = default(T);
                }
                Result = CacheGetResult.Found;
                Entity = track
                    ? DeepCloneItemForTrack(cached.Entity, cached.Cache.Relations)
                    : DeepCloneItem(cached.Entity, cached.Cache.Relations);
            }
        }

        public T Entity;

        public CacheGetResult Result;

        public static implicit operator T(CacheResult<T> result)
        {
            return result.Entity;
        }

        public static implicit operator CacheResult<T>(CacheGetResult result)
        {
            return new CacheResult<T>(default(T), result);
        }

        internal static T DeepCloneItemForTrack(T item, RelationInfo relations)
        {
            return (T)item.DeepCloneItemForTrack(relations);
        }

        internal static IEnumerable<T> DeepCloneListForTrack(IEnumerable<T> entities, RelationInfo relations)
        {
            return entities.Select(item => (T) item.DeepCloneItemForTrack(relations));
        }

        internal static T DeepCloneItem(T item, RelationInfo relations)
        {
            return (T) item.DeepCloneItem(relations);
        }

        internal static IEnumerable<T> DeepCloneList(IEnumerable<T> entities, RelationInfo relations)
        {
            return entities.Select(item => (T) item.DeepCloneItem(relations));
        }
    }
}