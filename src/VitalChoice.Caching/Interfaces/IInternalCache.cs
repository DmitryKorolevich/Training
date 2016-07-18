using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using VitalChoice.Caching.Relational;
using VitalChoice.Caching.Services.Cache.Base;

namespace VitalChoice.Caching.Interfaces
{
    public interface IInternalEntityCache : IEntityCollectorInfo, IDisposable
    {
        CachedEntity Update(RelationInfo relations, object entity);
        IEnumerable<CachedEntity> Update(RelationInfo relations, IEnumerable<object> entity);
        bool Update(IEnumerable<object> entities, RelationInfo relationInfo);
        bool Update(object entity, RelationInfo relationInfo);
        bool Update(object entity, DbContext context);
        bool UpdateAll(IEnumerable<object> entities, RelationInfo relationInfo);
        EntityKey MarkForUpdate(object entity);
        ICollection<EntityKey> MarkForUpdate(IEnumerable<object> entities);
        void SetNull(IEnumerable<EntityKey> keys, RelationInfo relationInfo);
        void SetNull(EntityKey key, RelationInfo relationInfo);
        void MarkForUpdate(EntityKey pk, string hasRelation = null);
        void MarkForUpdate(ICollection<EntityKey> pks, string hasRelation = null);
        EntityKey MarkForAdd(object entity);
        ICollection<EntityKey> MarkForAdd(ICollection<object> entities);
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

        bool Update(IEnumerable<T> entities, RelationInfo relationInfo);

        bool Update(T entity, RelationInfo relationInfo);

        bool Update(T entity, DbContext context);

        CachedEntity<T> Update(RelationInfo relations, T entity);

        IEnumerable<CachedEntity<T>> Update(RelationInfo relations, IEnumerable<T> entities);

        bool UpdateAll(IEnumerable<T> entities, RelationInfo relationInfo);

        EntityKey MarkForUpdate(T entity);

        ICollection<EntityKey> MarkForUpdate(IEnumerable<T> entities);

        EntityKey MarkForAdd(T entity);

        ICollection<EntityKey> MarkForAdd(ICollection<T> entities);
    }
}