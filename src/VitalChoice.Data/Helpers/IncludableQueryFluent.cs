using System;
using System.Linq.Expressions;
using VitalChoice.Data.Repositories;
using Microsoft.Data.Entity.Query;
using VitalChoice.Ecommerce.Domain;

namespace VitalChoice.Data.Helpers
{
    public sealed class IncludableQueryFluent<TEntity, TPreviousProperty>: QueryFluent<TEntity>, IIncludableQueryFluent<TEntity, TPreviousProperty>
        where TEntity : Entity
    {
        internal IIncludableQueryable<TEntity, TPreviousProperty> IncludableQuery { get; }

        public IncludableQueryFluent(ReadRepositoryAsync<TEntity> repository) : base(repository)
        {
            IncludableQuery = (IIncludableQueryable<TEntity, TPreviousProperty>)Query;
        }

        public IncludableQueryFluent(IQueryFluent<TEntity> queryFluent, IIncludableQueryable<TEntity, TPreviousProperty> query) : base((QueryFluent<TEntity>)queryFluent)
        {
            IncludableQuery = query;
            Query = query;
        }

        public IncludableQueryFluent(ReadRepositoryAsync<TEntity> repository, IQueryObject<TEntity> queryObject) : base(repository, queryObject)
        {
            IncludableQuery = (IIncludableQueryable<TEntity, TPreviousProperty>)Query;
        }

        public IncludableQueryFluent(ReadRepositoryAsync<TEntity> repository, Expression<Func<TEntity, bool>> expression) : base(repository, expression)
        {
            IncludableQuery = (IIncludableQueryable<TEntity, TPreviousProperty>)Query;
        }
    }
}