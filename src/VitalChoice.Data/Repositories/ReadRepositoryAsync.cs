using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.Data.Entity;
using VitalChoice.Data.Context;
using VitalChoice.Data.Helpers;
using VitalChoice.Ecommerce.Domain;

namespace VitalChoice.Data.Repositories
{
    public class ReadRepositoryAsync<TEntity> : IReadRepositoryAsync<TEntity> where TEntity : Entity
    {
        //public bool EarlyRead { get; set; } //added temporarly till ef 7 becomes stable

        protected IDataContextAsync Context { get; }
        protected internal DbSet<TEntity> DbSet { get; }

        public ReadRepositoryAsync(IDataContextAsync context)
        {
            this.Context = context;
            var dbContext = context as DbContext;
            if (dbContext != null)
                this.DbSet = dbContext.Set<TEntity>();
        }

        public IQueryFluent<TEntity> Query()
        {
            return new QueryFluent<TEntity>(this);
        }

        public virtual IQueryFluent<TEntity> Query(IQueryObject<TEntity> queryObject)
        {
            return new QueryFluent<TEntity>(this, queryObject);
        }

        public virtual IQueryFluent<TEntity> Query(Expression<Func<TEntity, bool>> query)
        {
            return new QueryFluent<TEntity>(this, query);
        }

        internal static IQueryable<TEntity> Select(
            IQueryable<TEntity> query,
            Expression<Func<TEntity, bool>> filter = null,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
            int? page = null,
            int? pageSize = null,
            bool tracking = true)
        {
            if (orderBy != null)
                query = orderBy(query);

            if (filter != null)
                query = query /*.AsExpandable()*/.Where(filter);

            if (!tracking)
                query = query.AsNoTracking();

            if (page != null && pageSize != null)
            {
                var pageNo = page.Value - 1;
                query = pageNo <= 0
                    ? query.Take(pageSize.Value)
                    : query.Skip(pageNo*pageSize.Value).Take(pageSize.Value);
            }

            return query;
        }

        internal async Task<List<TEntity>> SelectAsync(
            IQueryable<TEntity> query,
            Expression<Func<TEntity, bool>> filter = null,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
            int? page = null,
            int? pageSize = null,
            bool tracking = true)
        {
            //FIXED: added temporarly till ef 7 becomes stable, remove when it arrives
            //if (EarlyRead) 
            //{
            //	IEnumerable<TEntity> earlyRead = query.ToArray();
            //	if (orderBy != null)
            //		earlyRead = orderBy(earlyRead.AsQueryable());

            //	if (filter != null)
            //		earlyRead = earlyRead.Where(filter.CacheCompile());

            //	if (page != null && pageSize != null)
            //		earlyRead = earlyRead.Skip((page.Value - 1) * pageSize.Value).Take(pageSize.Value);

            //	return earlyRead.ToList();
            //}
            var queryable = Select(query, filter, orderBy, page, pageSize, tracking);
            return await queryable.ToListAsync();
        }
    }
}