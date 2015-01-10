using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.Data.Entity;
using VitalChoice.Data.DataContext;
using VitalChoice.Data.Extensions;
using VitalChoice.Data.Helpers;
using VitalChoice.Domain;

namespace VitalChoice.Data.Repositories
{
	public class ReadRepositoryAsync<TEntity> : IReadRepositoryAsync<TEntity> where TEntity : Entity
	{
		protected IDataContextAsync Context { get; }
		protected DbSet<TEntity> DbSet { get; }

		public ReadRepositoryAsync(IDataContextAsync context)
		{
			this.Context = context;
			// Temporarily for FakeDbContext, Unit Test Fakes
			var dbContext = context as DbContext;
			if (dbContext != null)
				this.DbSet = dbContext.Set<TEntity>();
			/*else
            {
                var fakeContext = context as FakeDbContext;
                if (fakeContext != null) dbSet = fakeContext.Set<TEntity>();
            }*/
		}

		/*
				public virtual IQueryable<TEntity> SelectQuery(string query, params object[] parameters)
				{
					return dbSet.SqlQuery(query, parameters).AsQueryable();
				}*/

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

		internal IQueryable<TEntity> Select(
			Expression<Func<TEntity, bool>> filter = null,
			Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
			List<Expression<Func<TEntity, object>>> includes = null,
			int? page = null,
			int? pageSize = null)
		{
			IQueryable<TEntity> query = DbSet;

			if (includes != null)
				query = includes.Aggregate(query, (current, include) => current.Include(include));

			if (orderBy != null)
				query = orderBy(query);

			if (filter != null)
				query = query.AsExpandable().Where(filter);

			if (page != null && pageSize != null)
				query = query.Skip((page.Value - 1) * pageSize.Value).Take(pageSize.Value);//.AsNoTracking()

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
