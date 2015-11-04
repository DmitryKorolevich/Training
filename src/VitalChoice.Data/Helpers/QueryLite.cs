using System;
using System.Linq.Expressions;
using VitalChoice.Data.Repositories;
using VitalChoice.Domain;

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