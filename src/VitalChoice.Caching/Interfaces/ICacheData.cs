using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.ChangeTracking.Internal;
using VitalChoice.Caching.Relational;
using VitalChoice.Caching.Relational.ChangeTracking;
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
        CachedEntity Update(object entity, object dbContext);
        CachedEntity UpdateExist(object entity, object dbContext);
        bool ItemExist(EntityKey key);
        bool GetHasRelation(string name);
        bool FullCollection { get; }
        bool NeedUpdate { get; set; }
        bool Empty { get; }
        RelationInfo Relations { get; }
        EntityInfo EntityInfo { get; }
    }

    public interface ICacheData<T> : ICacheData
    {
        CacheCluster<EntityKey, T> Get(EntityCacheableIndexInfo nonUniqueIndexInfo, EntityIndex index);
        CachedEntity<T> Get(EntityKey key);
        CachedEntity<T> Get(EntityIndex key);
        CachedEntity<T> Get(EntityConditionalIndexInfo conditionalIndex, EntityIndex index);
        ICollection<CachedEntity<T>> GetAll();
        CachedEntity<T> TryRemove(EntityKey key);
        CachedEntity<T> Update(T entity, object dbContext);
        CachedEntity<T> UpdateExist(T entity, object dbContext);
        CachedEntity<T> UpdateKeepRelations(T entity, ICacheStateManager stateManager, object dbContext);
        bool Update(IEnumerable<T> entity, object dbContext);
        bool UpdateExist(IEnumerable<T> entities, object dbContext);
        bool UpdateAll(IEnumerable<T> entity, object dbContext);
        void SetNull(EntityKey key);
        void SetNull(IEnumerable<EntityKey> keys);
    }
}