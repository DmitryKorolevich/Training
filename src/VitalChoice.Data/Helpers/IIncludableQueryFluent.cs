using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using VitalChoice.Domain;

namespace VitalChoice.Data.Helpers
{
    public interface IIncludableQueryFluent<TEntity, TPreviousProperty>: IQueryFluent<TEntity> where TEntity : Entity {
        IIncludableQueryFluent<TEntity, TProperty> ThenInclude<TProperty>(Expression<Func<TPreviousProperty, TProperty>> expression);

        IIncludableQueryFluent<TEntity, TProperty> ThenInclude<TProperty>(Expression<Func<TPreviousProperty, ICollection<TProperty>>> expression);
    }
}