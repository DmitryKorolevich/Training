using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using VitalChoice.Caching.Relational;
using VitalChoice.Caching.Services.Cache.Base;

namespace VitalChoice.Caching.Interfaces
{
    public interface IInternalEntityCache : IEntityCollectorInfo, IDisposable
    {
        CachedEntity Update(RelationInfo relations, object entity, object dbContext);
        IEnumerable<CachedEntity> Update(RelationInfo relations, IEnumerable<object> entity, object dbContext);
        bool Update(IEnumerable<object> entities, RelationInfo relationInfo, object dbContext);
        bool Update(object entity, RelationInfo relationInfo, object dbContext);
        bool Update(object entity, DbContext context, object dbContext);
        bool UpdateAll(IEnumerable<object> entities, RelationInfo relationInfo, object dbContext);
        EntityKey MarkForUpdate(object entity, object dbContext);
        ICollection<EntityKey> MarkForUpdate(IEnumerable<object> entities, object dbContext);
        void SetNull(IEnumerable<EntityKey> keys, RelationInfo relationInfo);
        void SetNull(EntityKey key, RelationInfo relationInfo);
        void MarkForUpdate(EntityKey pk, object dbContext, string hasRelation = null);
        void MarkForUpdate(ICollection<EntityKey> pks, object dbContext, string hasRelation = null);
        EntityKey MarkForAdd(object entity, object dbContext);
        ICollection<EntityKey> MarkForAdd(ICollection<object> entities, object dbContext);
        bool TryRemove(object entity);
        bool TryRemove(EntityKey pk);
        bool ItemExistWithoutRelations(EntityKey pk);
        bool ItemExistAndNotNull(EntityKey pk);
        bool GetCacheExist(RelationInfo relationInfo);
        bool GetIsCacheFullCollection(RelationInfo relationInfo);
        IEnumerable<ICacheData> GetAllCaches();
        ICacheData GetCacheData(RelationInfo relationInfo);
        bool IsFullCollection(RelationInfo relations);
        EntityInfo EntityInfo { get; }
    }

    public interface IInternalCache<T> : IInternalEntityCache
    {
        CacheResult<T> TryGetEntity(EntityKey key, RelationInfo relations, ICacheStateManager stateManager, bool tracked);

        IEnumerable<CacheResult<T>> TryGetEntities(ICollection<EntityKey> primaryKeys, RelationInfo relations,
            ICacheStateManager stateManager, bool tracked);

        CacheResult<T> TryGetEntity(EntityIndex index, RelationInfo relations, ICacheStateManager stateManager, bool tracked);

        IEnumerable<CacheResult<T>> TryGetEntities(ICollection<EntityIndex> indexes, RelationInfo relations, ICacheStateManager stateManager,
            bool tracked);

        CacheResult<T> TryGetEntity(EntityIndex key, EntityConditionalIndexInfo conditionalInfo, RelationInfo relations,
            ICacheStateManager stateManager, bool tracked);

        IEnumerable<CacheResult<T>> TryGetEntities(ICollection<EntityIndex> indexes,
            EntityConditionalIndexInfo conditionalInfo,
            RelationInfo relations, ICacheStateManager stateManager, bool tracked);

        IEnumerable<CacheResult<T>> GetWhere(RelationInfo relations, Func<T, bool> whereFunc, ICacheStateManager stateManager, bool tracked);

        IEnumerable<CacheResult<T>> GetAll(RelationInfo relations, ICacheStateManager stateManager, bool tracked);

        bool TryRemove(T entity);

        IEnumerable<CacheResult<T>> TryRemoveWithResult(T entity, ICacheStateManager stateManager, bool tracked);

        bool Update(IEnumerable<T> entities, RelationInfo relationInfo, object dbContext);

        bool Update(T entity, RelationInfo relationInfo, object dbContext);

        bool Update(T entity, DbContext context, object dbContext);

        CachedEntity<T> Update(RelationInfo relations, T entity, object dbContext);

        IEnumerable<CachedEntity<T>> Update(RelationInfo relations, IEnumerable<T> entities, object dbContext);

        bool UpdateAll(IEnumerable<T> entities, RelationInfo relationInfo, object dbContext);

        EntityKey MarkForUpdate(T entity, object dbContext);

        ICollection<EntityKey> MarkForUpdate(IEnumerable<T> entities, object dbContext);

        EntityKey MarkForAdd(T entity, object dbContext);

        ICollection<EntityKey> MarkForAdd(ICollection<T> entities, object dbContext);
    }
}