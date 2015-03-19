using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Microsoft.Data.Entity;
using VitalChoice.Data.Repositories;
using VitalChoice.Domain;

namespace VitalChoice.Data.Helpers
{
    public sealed class IncludableQueryFluent<TEntity, TPreviousProperty>: QueryFluent<TEntity>, IIncludableQueryFluent<TEntity, TPreviousProperty>
        where TEntity : Entity 
    {
        public IIncludableQueryFluent<TEntity, TProperty> ThenInclude<TProperty>(Expression<Func<TPreviousProperty, ICollection<TProperty>>> expression)
        {
            Query = ((IIncludableQueryable<TEntity, TPreviousProperty>)Query).ThenInclude(expression);
            return new IncludableQueryFluent<TEntity, TProperty>(this);
        }

        public IIncludableQueryFluent<TEntity, TProperty> ThenInclude<TProperty>(Expression<Func<TPreviousProperty, TProperty>> expression) {
            Query = ((IIncludableQueryable<TEntity, TPreviousProperty>)Query).ThenInclude(expression);
            return new IncludableQueryFluent<TEntity, TProperty>(this);
        }

        internal IncludableQueryFluent(QueryFluent<TEntity> queryFluent) : base(queryFluent)
        {
        }

        public IncludableQueryFluent(ReadRepositoryAsync<TEntity> repository) : base(repository)
        {
        }

        public IncludableQueryFluent(ReadRepositoryAsync<TEntity> repository, IQueryObject<TEntity> queryObject) : base(repository, queryObject)
        {
        }

        public IncludableQueryFluent(ReadRepositoryAsync<TEntity> repository, Expression<Func<TEntity, bool>> expression) : base(repository, expression)
        {
        }
    }
}