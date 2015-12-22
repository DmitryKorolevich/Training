using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using VitalChoice.Caching.Data;
using VitalChoice.Ecommerce.Domain;

namespace VitalChoice.Caching.Cache
{
    internal interface IEntityInternalCache<T> where T : Entity
    {
        bool TryGetEntity(EntityPrimaryKey primaryKey, out T entity);
        ICollection<T> GetWhere(Func<T, bool> whereFunc);
        ICollection<T> GetWhere(Expression<Func<T, bool>> whereExpression);
        ICollection<T> GetAll();
        bool TryRemove(T entity);
        bool TryRemove(IEnumerable<T> entities);
        void Update(IEnumerable<T> entities);
        void Update(T entity);
    }
}