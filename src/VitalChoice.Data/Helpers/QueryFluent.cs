#region

using Microsoft.Data.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using VitalChoice.Data.Repositories;
using VitalChoice.Domain;

#endregion

namespace VitalChoice.Data.Helpers
{
    public class QueryFluent<TEntity> : IQueryFluent<TEntity>
        where TEntity : Entity
    {
        private readonly Expression<Func<TEntity, bool>> _expression;
        protected readonly ReadRepositoryAsync<TEntity> Repository;
        private Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> _orderBy;
        protected IQueryable<TEntity> Query;

        protected QueryFluent(QueryFluent<TEntity> queryFluent)
        {
            Query = queryFluent.Query;
            Repository = queryFluent.Repository;
            _expression = queryFluent._expression;
            _orderBy = queryFluent._orderBy;
        }

        public QueryFluent(ReadRepositoryAsync<TEntity> repository)
        {
            Query = repository.DbSet;
            Repository = repository;
        }

        public QueryFluent(ReadRepositoryAsync<TEntity> repository, IQueryObject<TEntity> queryObject)
            : this(repository)
        {
            Query = repository.DbSet;
            _expression = queryObject.Query();
        }

        public QueryFluent(ReadRepositoryAsync<TEntity> repository, Expression<Func<TEntity, bool>> expression)
            : this(repository)
        {
            Query = repository.DbSet;
            _expression = expression;
        }

        public IQueryFluent<TEntity> OrderBy(Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy)
        {
            _orderBy = orderBy;
            return this;
        }

        public IIncludableQueryFluent<TEntity, TProperty> Include<TProperty>(Expression<Func<TEntity, TProperty>> expression)
        {
            Query = Query.Include(expression);
            return new IncludableQueryFluent<TEntity, TProperty>(this);
        }

        public IIncludableQueryFluent<TEntity, TProperty> Include<TProperty>(Expression<Func<TEntity, ICollection<TProperty>>> expression)
        {
            Query = Query.Include(expression);
            return new IncludableQueryFluent<TEntity, TProperty>(this,true);
        }
        public async Task<bool> SelectAnyAsync()
        {
            return await Repository.Select(Query, _expression, _orderBy).AnyAsync();
        }

        public async Task<int> SelectCountAsync()
        {
            return await Repository.Select(Query, _expression, _orderBy).CountAsync();
        }

        public IEnumerable<TEntity> SelectPage(int page, int pageSize, out int totalCount, bool tracking = true)
        {
            totalCount = Repository.Select(Query, _expression).Count();
            return Repository.Select(Query, _expression, _orderBy, page, pageSize, tracking);
        }
        public IEnumerable<TEntity> Select(bool tracking = true)
        {
            return Repository.Select(Query, _expression, _orderBy, tracking: tracking);
        }

        public IEnumerable<TResult> Select<TResult>(Expression<Func<TEntity, TResult>> selector, bool tracking = true)
        {
            return Repository.Select(Query, _expression, _orderBy, tracking: tracking).Select(selector);
        }

        public async Task<IEnumerable<TEntity>> SelectAsync(bool tracking = true)
        {
            return await Repository.SelectAsync(Query, _expression, _orderBy, tracking: tracking);
        }
    }
}