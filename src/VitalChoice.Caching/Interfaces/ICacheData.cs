using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Data.Entity;
using Microsoft.Data.Entity.ChangeTracking;
using VitalChoice.Caching.Relational;
using VitalChoice.Caching.Relational.ChangeTracking;
using VitalChoice.Caching.Services.Cache;
using VitalChoice.Caching.Services.Cache.Base;

namespace VitalChoice.Caching.Interfaces
{
    public interface ICacheData
    {
        void Clear();
        IEnumerable<CachedEntity> GetUntyped(EntityCacheableIndexInfo nonUniqueIndexInfo, EntityIndex index);
        CachedEntity GetUntyped(EntityKey pk);
        IEnumerable<CachedEntity> GetAllUntyped();
        CachedEntity TryRemoveUntyped(EntityKey key);
        bool ItemExist(EntityKey key);
        bool GetHasRelation(string name);
        bool FullCollection { get; }
        bool NeedUpdate { get; set; }
        bool Empty { get; }
    }

    public interface ICacheData<T> : ICacheData
    {
        CacheCluster<EntityKey, T> Get(EntityCacheableIndexInfo nonUniqueIndexInfo, EntityIndex index);
        CachedEntity<T> Get(EntityKey key);
        CachedEntity<T> Get(EntityIndex key);
        CachedEntity<T> Get(EntityConditionalIndexInfo conditionalIndex, EntityIndex index);
        ICollection<CachedEntity<T>> GetAll();
        CachedEntity<T> TryRemove(EntityKey key);
        CachedEntity<T> Update(T entity);
        CachedEntity<T> UpdateKeepRelations(T entity, Dictionary<TrackedEntityKey, EntityEntry> trackedEntities);
        bool Update(IEnumerable<T> entity);
        bool UpdateAll(IEnumerable<T> entity);
        void SetNull(EntityKey key);
        void SetNull(IEnumerable<EntityKey> keys);
    }
}