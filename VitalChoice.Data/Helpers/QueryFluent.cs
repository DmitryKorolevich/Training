#region

using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using VitalChoice.Data.Repositories;

#endregion

namespace VitalChoice.Data.Helpers
{
	public sealed class QueryFluent<TEntity> : IQueryFluent<TEntity> where TEntity : Entity
    {
        private readonly Expression<Func<TEntity, bool>> expression;
        private readonly List<Expression<Func<TEntity, object>>> includes;
        private readonly Repository<TEntity> repository;
        private Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy;

        public QueryFluent(Repository<TEntity> repository)
        {
            this.repository = repository;
            includes = new List<Expression<Func<TEntity, object>>>();
        }

        public QueryFluent(Repository<TEntity> repository, IQueryObject<TEntity> queryObject)
            : this(repository)
        {
            expression = queryObject.Query();
        }

        public QueryFluent(Repository<TEntity> repository, Expression<Func<TEntity, bool>> expression) 
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

        public IEnumerable<TEntity> SelectPage(int page, int pageSize, out int totalCount)
        {
            totalCount = repository.Select(expression).Count();
            return repository.Select(expression, orderBy, includes, page, pageSize);
        }
        public IEnumerable<TEntity> Select()
        {
            return repository.Select(expression, orderBy, includes);
        }

        public IEnumerable<TResult> Select<TResult>(Expression<Func<TEntity, TResult>> selector)
        {
            return repository.Select(expression, orderBy, includes).Select(selector);
        }

        public async Task<IEnumerable<TEntity>> SingleAsync()
        {
            return await repository.SelectAsync(expression, orderBy, includes);
        }
    }
}