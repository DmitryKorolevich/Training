using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using VitalChoice.Caching.Relational;
using VitalChoice.Caching.Services.Cache;

namespace VitalChoice.Caching.Interfaces
{
    public interface IInternalEntityCache
    {
        CachedEntity Update(RelationInfo relations, object entity);
        IEnumerable<CachedEntity> Update(RelationInfo relations, IEnumerable<object> entity);
        void Update(IEnumerable<object> entities, RelationInfo relationInfo);
        void Update(object entity, RelationInfo relationInfo);
        void UpdateAll(IEnumerable<object> entities, RelationInfo relationInfo);
        void MarkForUpdate(object entity);
        void MarkForUpdate(IEnumerable<object> entities);
        bool TryRemove(object entity);
        bool GetCacheExist(RelationInfo relationInfo);
        bool GetIsCacheFullCollection(RelationInfo relationInfo);
    }

    public interface IInternalEntityCache<T> : IInternalEntityCache
    {
        CacheGetResult TryGetEntity(EntityKey key, RelationInfo relations, out T entity);
        CacheGetResult TryGetEntities(ICollection<EntityKey> primaryKeys, RelationInfo relations, Expression<Func<T, bool>> whereExpression, out List<T> entities);
        CacheGetResult TryGetEntity(EntityIndex index, RelationInfo relations, out T entity);
        CacheGetResult TryGetEntities(ICollection<EntityIndex> indexes, RelationInfo relations, Expression<Func<T, bool>> whereExpression, out List<T> entities);
        CacheGetResult TryGetEntity(EntityIndex key, EntityConditionalIndexInfo conditionalInfo, RelationInfo relations, out T entity);
        CacheGetResult TryGetEntities(ICollection<EntityIndex> indexes, EntityConditionalIndexInfo conditionalInfo,
            RelationInfo relations, Expression<Func<T, bool>> whereExpression, out List<T> entities);
        CacheGetResult GetWhere(RelationInfo relations, Func<T, bool> whereFunc, out List<T> entities);
        CacheGetResult GetWhere(RelationInfo relations, Expression<Func<T, bool>> whereExpression, out List<T> entities);
        CacheGetResult GetAll(RelationInfo relations, out List<T> entities);
        CacheGetResult GetFirstWhere(RelationInfo relations, Func<T, bool> whereFunc, out T entity);
        CacheGetResult GetFirstWhere(RelationInfo relations, Expression<Func<T, bool>> whereExpression, out T entity);
        CacheGetResult GetFirst(RelationInfo relations, out T entity);
        bool TryRemove(T entity, out List<T> removedList);
        bool TryRemove(T entity);
        void Update(IEnumerable<T> entities, RelationInfo relationInfo);
        void Update(T entity, RelationInfo relationInfo);
        CachedEntity<T> Update(RelationInfo relations, T entity);
        IEnumerable<CachedEntity<T>> Update(RelationInfo relations, IEnumerable<T> entities);
        void UpdateAll(IEnumerable<T> entities, RelationInfo relationInfo);
        void MarkForUpdate(T entity);
        void MarkForUpdate(IEnumerable<T> entities);
    }
}