using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using VitalChoice.Caching.Relational;
using VitalChoice.Caching.Services.Cache;
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
        bool ItemExist(EntityKey pk);
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
        CacheResult<T> TryGetEntity(EntityKey key, RelationInfo relations, bool track);

        IEnumerable<CacheResult<T>> TryGetEntities(ICollection<EntityKey> primaryKeys, RelationInfo relations, bool track);

        CacheResult<T> TryGetEntity(EntityIndex index, RelationInfo relations, bool track);

        IEnumerable<CacheResult<T>> TryGetEntities(ICollection<EntityIndex> indexes, RelationInfo relations, bool track);

        CacheResult<T> TryGetEntity(EntityIndex key, EntityConditionalIndexInfo conditionalInfo, RelationInfo relations, bool track);

        IEnumerable<CacheResult<T>> TryGetEntities(ICollection<EntityIndex> indexes,
            EntityConditionalIndexInfo conditionalInfo,
            RelationInfo relations, bool track);

        IEnumerable<CacheResult<T>> GetWhere(RelationInfo relations, Func<T, bool> whereFunc, bool track);
        IEnumerable<CacheResult<T>> GetAll(RelationInfo relations, bool track);
        bool TryRemove(T entity);
        IEnumerable<CacheResult<T>> TryRemoveWithResult(T entity);
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