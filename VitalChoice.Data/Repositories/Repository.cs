#region

using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using LinqKit;
using VitalChoice.Data.DataContext;
using VitalChoice.Data.Helpers;
using VitalChoice.Data.Infrastructure;

#endregion

namespace VitalChoice.Data.Repositories
{
    public class Repository<TEntity> : 
        IRepository<TEntity>, 
        IRepositoryAsync<TEntity> where TEntity : Entity
    {
        private readonly IDataContextAsync context;
        private readonly DbSet<TEntity> dbSet;

        public Repository(IDataContextAsync context)
        {
            this.context = context;
            // Temporarily for FakeDbContext, Unit Test Fakes
            var dbContext = context as DbContext;
            if (dbContext != null) 
                dbSet = dbContext.Set<TEntity>();
            /*else
            {
                var fakeContext = context as FakeDbContext;
                if (fakeContext != null) dbSet = fakeContext.Set<TEntity>();
            }*/
        }

        public virtual TEntity Find(params object[] keyValues)
        {
            return dbSet.Find(keyValues);
        }
/*
        public virtual IQueryable<TEntity> SelectQuery(string query, params object[] parameters)
        {
            return dbSet.SqlQuery(query, parameters).AsQueryable();
        }*/

        public virtual void Insert(TEntity entity)
        {
            ((IObjectState) entity).ObjectState = ObjectState.Added;
            dbSet.Attach(entity);
            context.SyncObjectState(entity);
        }

        public virtual void InsertRange(IEnumerable<TEntity> entities)
        {
            foreach (var entity in entities)
                Insert(entity);
        }

        public virtual void InsertGraph(TEntity entity)
        {
            dbSet.Add(entity);
        }

        public virtual void InsertGraphRange(IEnumerable<TEntity> entities)
        {
            dbSet.AddRange(entities);
        }

        public virtual void Update(TEntity entity)
        {
            ((IObjectState) entity).ObjectState = ObjectState.Modified;
            dbSet.Attach(entity);
            context.SyncObjectState(entity);
        }

        public virtual void Delete(object id)
        {
            var entity = dbSet.Find(id);
            Delete(entity);
        }

        public virtual void Delete(TEntity entity)
        {
            ((IObjectState) entity).ObjectState = ObjectState.Deleted;
            dbSet.Attach(entity);
            context.SyncObjectState(entity);
        }

        public IQueryFluent<TEntity> Query()
        {
            return new QueryFluent<TEntity>(this);
        }

		//public int Update(IQueryObject<TEntity> queryObject)
		//{
		//	return dbSet.Update(queryObject.Query());
		//}

		//public int Delete(IQueryObject<TEntity> queryObject)
		//{
		//	IQueryable<Entity> query = dbSet.AsQueryable();
		//	return context.Delete(queryObject.Query());
		//}

	    public virtual IQueryFluent<TEntity> Query(IQueryObject<TEntity> queryObject)
        {
            return new QueryFluent<TEntity>(this, queryObject);
        }

        public virtual IQueryFluent<TEntity> Query(Expression<Func<TEntity, bool>> query)
        {
            return new QueryFluent<TEntity>(this, query);
        }

        public virtual async Task<TEntity> FindAsync(params object[] keyValues)
        {
            return await dbSet.FindAsync(keyValues);
        }

        public virtual async Task<TEntity> FindAsync(CancellationToken cancellationToken, params object[] keyValues)
        {
            return await dbSet.FindAsync(cancellationToken, keyValues);
        }

        public virtual async Task<bool> DeleteAsync(params object[] keyValues)
        {
            return await DeleteAsync(CancellationToken.None, keyValues);
        }

        public virtual async Task<bool> DeleteAsync(CancellationToken cancellationToken, params object[] keyValues)
        {
            var entity = await FindAsync(cancellationToken, keyValues);
            if (entity == null) return false;

            dbSet.Attach(entity);
            dbSet.Remove(entity);
            return true;
        }

        internal IQueryable<TEntity> Select(
            Expression<Func<TEntity, bool>> filter = null,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
            List<Expression<Func<TEntity, object>>> includes = null,
            int? page = null,
            int? pageSize = null)
        {
            IQueryable<TEntity> query = dbSet;

            if (includes != null)
	            query = includes.Aggregate(query, (current, include) => current.Include(include));

	        if (orderBy != null)
                query = orderBy(query);

            if (filter != null)
                query = query.AsExpandable().Where(filter);

            if (page != null && pageSize != null)
                query = query.Skip((page.Value - 1)*pageSize.Value).Take(pageSize.Value);

            return query;
        }

        internal async Task<IEnumerable<TEntity>> SelectAsync(
            Expression<Func<TEntity, bool>> query = null,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
            List<Expression<Func<TEntity, object>>> includes = null,
            int? page = null,
            int? pageSize = null)
        {
            return Select(query, orderBy, includes, page, pageSize).AsEnumerable();
        }

		//public IQueryable ODataQueryable(ODataQueryOptions<TEntity> oDataQueryOptions)
		//{
		//	return oDataQueryOptions.ApplyTo(dbSet);
		//}

		//public IQueryable<TEntity> ODataQueryable()
		//{
		//	return dbSet;
		//}
    }
}