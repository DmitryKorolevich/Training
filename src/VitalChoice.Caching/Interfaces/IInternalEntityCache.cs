using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using VitalChoice.Caching.Relational;

namespace VitalChoice.Caching.Interfaces
{
    public interface IInternalEntityCache
    {
        bool TryGetEntity(EntityPrimaryKeySearchInfo searchInfo, out object entity);
        bool TryGetEntities(EntityPrimaryKeySearchInfo[] searchInfos, out List<object> entities);
        bool TryGetEntities(EntityPrimaryKey[] primaryKeys, RelationInfo relations, out List<object> entities);
        bool TryGetEntity(EntityUniqueIndexSearchInfo searchInfo, out object entity);
        bool TryGetEntities(EntityUniqueIndexSearchInfo[] searchInfos, out List<object> entities);
        bool TryGetEntities(EntityUniqueIndex[] indexes, RelationInfo relations, out List<object> entities);
        void Update(IEnumerable<object> entities, RelationInfo relationInfo);
        void Update(object entity, RelationInfo relationInfo);
        void UpdateAll(IEnumerable<object> entities, RelationInfo relationInfo);
        bool GetCacheExist(RelationInfo relationInfo);
    }

    public interface IInternalEntityCache<T> : IInternalEntityCache
    {
        bool TryGetEntity(EntityPrimaryKeySearchInfo searchInfo, out T entity);
        bool TryGetEntities(EntityPrimaryKeySearchInfo[] searchInfos, out List<T> entities);
        bool TryGetEntities(EntityPrimaryKey[] primaryKeys, RelationInfo relations, out List<T> entities);
        bool TryGetEntity(EntityUniqueIndexSearchInfo searchInfo, out T entity);
        bool TryGetEntities(EntityUniqueIndexSearchInfo[] searchInfos, out List<T> entities);
        bool TryGetEntities(EntityUniqueIndex[] indexes, RelationInfo relations, out List<T> entities);
        IEnumerable<T> GetWhere(RelationInfo relations, Func<T, bool> whereFunc);
        IEnumerable<T> GetWhere(RelationInfo relations, Expression<Func<T, bool>> whereExpression);
        IEnumerable<T> GetAll(RelationInfo relations);
        bool TryRemove(T entity, out List<T> removedList);
        void Update(IEnumerable<T> entities, RelationInfo relationInfo);
        void Update(T entity, RelationInfo relationInfo);
        void UpdateAll(IEnumerable<T> entities, RelationInfo relationInfo);
    }
}