using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using VitalChoice.Ecommerce.Domain;
using VitalChoice.Ecommerce.Domain.Entities.Base;

namespace VitalChoice.Data.Helpers
{
    public interface IQueryLite<TEntity>
        where TEntity : Entity
    {
        IIncludableQueryLite<TEntity, TProperty> Include<TProperty>(Expression<Func<TEntity, TProperty>> expression);
    }
}