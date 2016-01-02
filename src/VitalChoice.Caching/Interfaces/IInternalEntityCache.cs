using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using VitalChoice.Caching.Relational;
using VitalChoice.Caching.Services.Cache;

namespace VitalChoice.Caching.Interfaces
{
    public interface IInternalEntityCache
    {
        bool TryRemove(object entity, out List<object> removedList);
        bool TryRemove(object entity);
        void Update(IEnumerable<object> entities, RelationInfo relationInfo);
        void Update(object entity, RelationInfo relationInfo);
        void UpdateAll(IEnumerable<object> entities, RelationInfo relationInfo);
        void MarkForUpdate(object entity);
        void MarkForUpdate(IEnumerable<object> entities);
        CachedEntity UpdateCache(object entity, RelationInfo relations);
        ICollection<CachedEntity> UpdateCache(IEnumerable<object> entities, RelationInfo relations);
        bool GetCacheExist(RelationInfo relationInfo);
        bool GetIsCacheFullCollection(RelationInfo relationInfo);
    }

    public interface IInternalEntityCache<T> : IInternalEntityCache
    {
        CacheGetResult TryGetEntity(EntityPrimaryKeySearchInfo searchInfo, out T entity);
        CacheGetResult TryGetEntities(ICollection<EntityPrimaryKeySearchInfo> searchInfos, Expression<Func<T, bool>> whereExpression, out List<T> entities);
        CacheGetResult TryGetEntities(ICollection<EntityPrimaryKey> primaryKeys, RelationInfo relations, Expression<Func<T, bool>> whereExpression, out List<T> entities);
        CacheGetResult TryGetEntity(EntityUniqueIndexSearchInfo searchInfo, out T entity);
        CacheGetResult TryGetEntities(ICollection<EntityUniqueIndexSearchInfo> searchInfos, Expression<Func<T, bool>> whereExpression, out List<T> entities);
        CacheGetResult TryGetEntities(ICollection<EntityUniqueIndex> indexes, RelationInfo relations, Expression<Func<T, bool>> whereExpression, out List<T> entities);
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
        void UpdateAll(IEnumerable<T> entities, RelationInfo relationInfo);
        void MarkForUpdate(T entity);
        void MarkForUpdate(IEnumerable<T> entities);
        CachedEntity<T> UpdateCache(T entity, RelationInfo relations, CacheData<T> data = null);
        ICollection<CachedEntity<T>> UpdateCache(IEnumerable<T> entities, RelationInfo relations, CacheData<T> data = null);
    }
}