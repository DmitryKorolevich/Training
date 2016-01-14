using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using VitalChoice.Caching.Relational;
using VitalChoice.Caching.Services.Cache;
using VitalChoice.Caching.Services.Cache.Base;

namespace VitalChoice.Caching.Interfaces
{
    public interface IInternalEntityCache: ICacheKeysStorage, IDisposable
    {
        CachedEntity Update(RelationInfo relations, object entity);
        IEnumerable<CachedEntity> Update(RelationInfo relations, IEnumerable<object> entity);
        void Update(IEnumerable<object> entities, RelationInfo relationInfo);
        void Update(object entity, RelationInfo relationInfo);
        void Update(object entity);
        void SetNull(IEnumerable<EntityKey> keys, RelationInfo relationInfo);
        void SetNull(EntityKey key, RelationInfo relationInfo);
        void UpdateAll(IEnumerable<object> entities, RelationInfo relationInfo);
        void MarkForUpdate(object entity);
        void MarkForUpdate(IEnumerable<object> entities);
        void MarkForUpdate(EntityKey pk);
        void MarkForUpdate(IEnumerable<EntityKey> pks);
        bool TryRemove(object entity);
        bool TryRemove(EntityKey pk);
        bool GetCacheExist(RelationInfo relationInfo);
        bool GetIsCacheFullCollection(RelationInfo relationInfo);
        IEnumerable<ICacheData> GetAllCaches();
    }

    public interface IInternalEntityCache<T> : IInternalEntityCache, ICacheKeysStorage<T>
    {
        CacheResult<T> TryGetEntity(EntityKey key, RelationInfo relations);

        IEnumerable<CacheResult<T>> TryGetEntities(ICollection<EntityKey> primaryKeys, RelationInfo relations);

        CacheResult<T> TryGetEntity(EntityIndex index, RelationInfo relations);

        IEnumerable<CacheResult<T>> TryGetEntities(ICollection<EntityIndex> indexes, RelationInfo relations);

        CacheResult<T> TryGetEntity(EntityIndex key, EntityConditionalIndexInfo conditionalInfo, RelationInfo relations);

        IEnumerable<CacheResult<T>> TryGetEntities(ICollection<EntityIndex> indexes,
            EntityConditionalIndexInfo conditionalInfo,
            RelationInfo relations);

        IEnumerable<CacheResult<T>> GetWhere(RelationInfo relations, Func<T, bool> whereFunc);
        IEnumerable<CacheResult<T>> GetAll(RelationInfo relations);
        bool TryRemove(T entity);
        IEnumerable<CacheResult<T>> TryRemoveWithResult(T entity);
        void Update(IEnumerable<T> entities, RelationInfo relationInfo);
        void Update(T entity, RelationInfo relationInfo);
        void Update(T entity);
        CachedEntity<T> Update(RelationInfo relations, T entity);
        IEnumerable<CachedEntity<T>> Update(RelationInfo relations, IEnumerable<T> entities);
        void UpdateAll(IEnumerable<T> entities, RelationInfo relationInfo);
        void MarkForUpdate(T entity);
        void MarkForUpdate(IEnumerable<T> entities);
    }
}