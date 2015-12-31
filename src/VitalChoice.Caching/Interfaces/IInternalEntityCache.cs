using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using VitalChoice.Caching.Relational;
using VitalChoice.Caching.Services.Cache;

namespace VitalChoice.Caching.Interfaces
{
    public interface IInternalEntityCache
    {
        CacheGetResult TryGetEntity(EntityPrimaryKeySearchInfo searchInfo, out object entity);
        CacheGetResult TryGetEntities(EntityPrimaryKeySearchInfo[] searchInfos, out List<object> entities);
        CacheGetResult TryGetEntities(EntityPrimaryKey[] primaryKeys, RelationInfo relations, out List<object> entities);
        CacheGetResult TryGetEntity(EntityUniqueIndexSearchInfo searchInfo, out object entity);
        CacheGetResult TryGetEntities(EntityUniqueIndexSearchInfo[] searchInfos, out List<object> entities);
        CacheGetResult TryGetEntities(EntityUniqueIndex[] indexes, RelationInfo relations, out List<object> entities);
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
    }

    public interface IInternalEntityCache<T> : IInternalEntityCache
    {
        CacheGetResult TryGetEntity(EntityPrimaryKeySearchInfo searchInfo, out T entity);
        CacheGetResult TryGetEntities(EntityPrimaryKeySearchInfo[] searchInfos, out List<T> entities);
        CacheGetResult TryGetEntities(EntityPrimaryKey[] primaryKeys, RelationInfo relations, out List<T> entities);
        CacheGetResult TryGetEntity(EntityUniqueIndexSearchInfo searchInfo, out T entity);
        CacheGetResult TryGetEntities(EntityUniqueIndexSearchInfo[] searchInfos, out List<T> entities);
        CacheGetResult TryGetEntities(EntityUniqueIndex[] indexes, RelationInfo relations, out List<T> entities);
        IEnumerable<T> GetWhere(RelationInfo relations, Func<T, bool> whereFunc);
        IEnumerable<T> GetWhere(RelationInfo relations, Expression<Func<T, bool>> whereExpression);
        IEnumerable<T> GetAll(RelationInfo relations);
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