#region

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
    public sealed class QueryFluent<TEntity> : IQueryFluent<TEntity> where TEntity : Entity
    {
        private readonly Expression<Func<TEntity, bool>> expression;
        private readonly List<Expression<Func<TEntity, object>>> includes;
        private readonly ReadRepositoryAsync<TEntity> repository;
        private Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy;

        public QueryFluent(ReadRepositoryAsync<TEntity> repository)
        {
            this.repository = repository;
            includes = new List<Expression<Func<TEntity, object>>>();
        }

        public QueryFluent(ReadRepositoryAsync<TEntity> repository, IQueryObject<TEntity> queryObject)
            : this(repository)
        {
            expression = queryObject.Query();
        }

        public QueryFluent(ReadRepositoryAsync<TEntity> repository, Expression<Func<TEntity, bool>> expression)
            : this(repository)
        {
            this.expression = expression;
        }

        public IQueryFluent<TEntity> OrderBy(Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy)
        {
            this.orderBy = orderBy;
            return this;
        }

        public IQueryFluent<TEntity> Include(Expression<Func<TEntity, object>> expression)
        {
            includes.Add(expression);
            return this;
        }

        public IEnumerable<TEntity> SelectPage(int page, int pageSize, out int totalCount, bool tracking = true)
        {
            totalCount = repository.Select(expression).Count();
            return repository.Select(expression, orderBy, includes, page, pageSize, tracking);
        }
        public IEnumerable<TEntity> Select(bool tracking = true)
        {
            return repository.Select(expression, orderBy, includes, tracking: tracking);
        }

        public IEnumerable<TResult> Select<TResult>(Expression<Func<TEntity, TResult>> selector, bool tracking = true)
        {
            return repository.Select(expression, orderBy, includes, tracking: tracking).Select(selector);
        }

        public async Task<IEnumerable<TEntity>> SelectAsync(bool tracking = true)
        {
            return await repository.SelectAsync(expression, orderBy, includes, tracking: tracking);
        }
    }
}