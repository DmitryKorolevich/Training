using System;
using System.Linq.Expressions;
using VitalChoice.Data.Repositories;
using VitalChoice.Domain;

namespace VitalChoice.Data.Helpers
{
    public interface IQueryLite<TEntity>
        where TEntity : Entity
    {
        IIncludableQueryLite<TEntity, TProperty> Include<TProperty>(Expression<Func<TEntity, TProperty>> expression);
    }
}