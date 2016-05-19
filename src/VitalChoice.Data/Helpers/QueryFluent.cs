using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using VitalChoice.Data.Repositories;
using VitalChoice.Ecommerce.Domain;
using VitalChoice.Ecommerce.Domain.Exceptions;
using VitalChoice.Ecommerce.Domain.Transfer;

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

        public async Task<List<TResult>> SelectAsync<TResult>(Expression<Func<TEntity, TResult>> selector, bool tracking = true)
        {
            var query = RepositoryAsync<TEntity>.Select(Query, _expression, _orderBy, tracking: tracking).Select(selector);
            try
            {
                return await query.ToListAsync();
            }
            catch (Exception e)
            {
                throw new ApiException($"{query.Expression}", e);
            }
        }

        public async Task<TEntity> SelectFirstOrDefaultAsync(bool tracking = true)
        {
            var query = RepositoryAsync<TEntity>.Select(Query, _expression, _orderBy, tracking: tracking);
            try
            {
                return await query.FirstOrDefaultAsync();
            }
            catch (Exception e)
            {
                throw new ApiException($"{query.Expression}", e);
            }
        }

        public IQueryFluent<TEntity> Where(Expression<Func<TEntity, bool>> predicate)
        {
            Query = Query.Where(predicate);
            return this;
        }

        public IQueryFluent<TEntity> Distinct()
        {
            Query = Query.Distinct();
            return this;
        }

        public IQueryFluent<TEntity> OrderBy(Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy)
        {
            _orderBy = orderBy;
            return this;
        }

        public IIncludableQueryFluent<TEntity, TProperty> Include<TProperty>(Expression<Func<TEntity, TProperty>> expression)
        {
            return new IncludableQueryFluent<TEntity, TProperty>(this, Query.Include(expression));
        }

        public async Task<bool> SelectAnyAsync()
        {
            var query = RepositoryAsync<TEntity>.Select(Query, _expression, _orderBy);
            try
            {
                return await query.AnyAsync();
            }
            catch (Exception e)
            {
                throw new ApiException($"{query.Expression}", e);
            }
        }

        public async Task<int> SelectCountAsync()
        {
            var query = RepositoryAsync<TEntity>.Select(Query, _expression, _orderBy);
            try
            {
                return await query.CountAsync();
            }
            catch (Exception e)
            {
                throw new ApiException($"{query.Expression}", e);
            }
        }

        public async Task<decimal> SelectSumAsync(Expression<Func<TEntity,decimal>> func)
        {
            var query = RepositoryAsync<TEntity>.Select(Query, _expression, _orderBy);
            try
            {
                return await query.SumAsync(func);
            }
            catch (Exception e)
            {
                throw new ApiException($"{query.Expression}", e);
            }
        }

        public List<TEntity> SelectPage(int page, int pageSize, out int totalCount, bool tracking = false)
        {
            var countQuery = RepositoryAsync<TEntity>.Select(Query, _expression);
            try
            {
                totalCount = countQuery.Count();
            }
            catch (Exception e)
            {
                throw new ApiException($"{countQuery.Expression}", e);
            }
            var selectQuery = RepositoryAsync<TEntity>.Select(Query, _expression, _orderBy, page, pageSize, tracking);
            try
            {
                return selectQuery.ToList();
            }
            catch (Exception e)
            {
                throw new ApiException($"{selectQuery.Expression}", e);
            }
        }

        public async Task<PagedList<TEntity>> SelectPageAsync(int page, int pageSize, bool tracking = false)
        {
            var countQuery = RepositoryAsync<TEntity>.Select(Query, _expression);
            int count;
            try
            {
                count = await countQuery.CountAsync();
            }
            catch (Exception e)
            {
                throw new ApiException($"{countQuery.Expression}", e);
            }
            var items = await Repository.SelectAsync(Query, _expression, _orderBy, page, pageSize, tracking);
            return new PagedList<TEntity>(items, count);
        }

        public List<TEntity> Select(bool tracking = true)
        {
            var query = RepositoryAsync<TEntity>.Select(Query, _expression, _orderBy, tracking: tracking);
            try
            {
                return query.ToList();
            }
            catch (Exception e)
            {
                throw new ApiException($"{query.Expression}", e);
            }
        }

        public List<TResult> Select<TResult>(Expression<Func<TEntity, TResult>> selector, bool tracking = true)
        {
            var query = RepositoryAsync<TEntity>.Select(Query, _expression, _orderBy, tracking: tracking).Select(selector);
            try
            {
                return query.ToList();
            }
            catch (Exception e)
            {
                throw new ApiException($"{query.Expression}", e);
            }
        }

        public Task<List<TEntity>> SelectAsync(bool tracking = true)
        {
            return Repository.SelectAsync(Query, _expression, _orderBy, tracking: tracking);
        }
    }
}