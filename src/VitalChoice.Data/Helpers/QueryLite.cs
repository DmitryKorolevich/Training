using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using VitalChoice.Ecommerce.Domain;
using VitalChoice.Ecommerce.Domain.Entities.Base;

namespace VitalChoice.Data.Helpers
{
    public class QueryLite<TEntity> : IQueryLite<TEntity> 
        where TEntity : Entity
    {
        public QueryLite(IQueryFluent<TEntity> queryFluent)
        {
            Query = queryFluent;
        }

        public IQueryFluent<TEntity> Query { get; }

        public IIncludableQueryLite<TEntity, TProperty> Include<TProperty>(
            Expression<Func<TEntity, TProperty>> expression)
        {
            return new IncludableQueryLite<TEntity, TProperty>(Query.Include(expression));
        }
    }
}