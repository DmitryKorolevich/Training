using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Microsoft.Data.Entity;
using VitalChoice.Data.Repositories;
using VitalChoice.Domain;
using Microsoft.Data.Entity.Query;

namespace VitalChoice.Data.Helpers
{
    public sealed class IncludableQueryFluent<TEntity, TPreviousProperty>: QueryFluent<TEntity>, IIncludableQueryFluent<TEntity, TPreviousProperty>
        where TEntity : Entity 
    {
        private readonly bool _forICollection;

        public IIncludableQueryFluent<TEntity, TProperty> ThenInclude<TProperty>(Expression<Func<TPreviousProperty, ICollection<TProperty>>> expression)
        {
            if (_forICollection)
            {
                Query = ((IIncludableQueryable<TEntity, ICollection<TPreviousProperty>>)Query).ThenInclude(expression);
                return new IncludableQueryFluent<TEntity, TProperty>(this, true);
            }
            Query = ((IIncludableQueryable<TEntity, TPreviousProperty>)Query).ThenInclude(expression);
            return new IncludableQueryFluent<TEntity, TProperty>(this, true);
        }

        public IIncludableQueryFluent<TEntity, TProperty> ThenInclude<TProperty>(Expression<Func<TPreviousProperty, TProperty>> expression)
        {
            if (_forICollection)
            {
                Query = ((IIncludableQueryable<TEntity, ICollection<TPreviousProperty>>)Query).ThenInclude(expression);
                return new IncludableQueryFluent<TEntity, TProperty>(this);
            }
            Query = ((IIncludableQueryable<TEntity, TPreviousProperty>)Query).ThenInclude(expression);
            return new IncludableQueryFluent<TEntity, TProperty>(this);
        }

        internal IncludableQueryFluent(QueryFluent<TEntity> queryFluent,bool forICollection=false) : base(queryFluent)
        {
            this._forICollection = forICollection;
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