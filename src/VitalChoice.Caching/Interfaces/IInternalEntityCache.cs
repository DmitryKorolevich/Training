using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using VitalChoice.Caching.Relational;
using VitalChoice.Ecommerce.Domain;

namespace VitalChoice.Caching.Interfaces
{
    public interface IInternalEntityCache
    {
        bool TryGetEntity(EntityPrimaryKey primaryKey, out object entity);
        bool TryRemove(object entity);
        bool TryRemove(IEnumerable<object> entities);
        bool TryRemoveTree(object entity);
        bool TryRemoveTree(IEnumerable<object> entities);
        void Update(IEnumerable<object> entities, ICollection<RelationInfo> relations);
        void Update(object entity, ICollection<RelationInfo> relations);
        bool Empty { get; }
    }

    public interface IInternalEntityCache<T> : IInternalEntityCache
        where T : Entity
    {
        bool TryGetEntity(EntityPrimaryKey primaryKey, out T entity);
        ICollection<T> GetWhere(Func<T, bool> whereFunc);
        ICollection<T> GetWhere(Expression<Func<T, bool>> whereExpression);
        ICollection<T> GetAll();
        bool TryRemove(T entity);
        bool TryRemove(IEnumerable<T> entities);
        bool TryRemoveTree(T entity);
        bool TryRemoveTree(IEnumerable<T> entities);
        void Update(IEnumerable<T> entities, ICollection<RelationInfo> relations);
        void Update(T entity, ICollection<RelationInfo> relations);
    }
}