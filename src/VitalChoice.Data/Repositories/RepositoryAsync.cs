using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using VitalChoice.Data.DataContext;
using VitalChoice.Data.Infrastructure;

namespace VitalChoice.Data.Repositories
{
	public class RepositoryAsync<TEntity> : ReadRepositoryAsync<TEntity>, IRepositoryAsync<TEntity> where TEntity : Entity
	{
		public RepositoryAsync(IDataContextAsync context) : base(context)
		{
		}

		public virtual void Insert(TEntity entity)
		{
			((IObjectState)entity).ObjectState = ObjectState.Added;
			DbSet.Attach(entity);
			Context.SyncObjectState(entity);
		}

		public virtual void InsertRange(IEnumerable<TEntity> entities)
		{
			foreach (var entity in entities)
				Insert(entity);
			Context.SaveChanges();
		}

		public virtual void InsertGraph(TEntity entity)
		{
			DbSet.Add(entity);
			Context.SaveChanges();
		}

		public virtual void InsertGraphRange(IEnumerable<TEntity> entities)
		{
			DbSet.AddRange(entities);
			Context.SaveChanges();
		}

		public virtual void Update(TEntity entity)
		{
			((IObjectState)entity).ObjectState = ObjectState.Modified;
			DbSet.Attach(entity);
			Context.SyncObjectState(entity);
			Context.SaveChanges();
		}

		public virtual void Delete(object id)
		{
			var entity = DbSet.Find(id);
			Delete(entity);
		}

		public virtual void Delete(TEntity entity)
		{
			((IObjectState)entity).ObjectState = ObjectState.Deleted;
			DbSet.Attach(entity);
			Context.SyncObjectState(entity);
			Context.SaveChanges();
		}

		public virtual async Task<bool> DeleteAsync(params object[] keyValues)
		{
			return await DeleteAsync(CancellationToken.None, keyValues);
		}

		public virtual async Task<bool> DeleteAsync(CancellationToken cancellationToken, params object[] keyValues)
		{
			var entity = await FindAsync(cancellationToken, keyValues);
			if (entity == null) return false;

			DbSet.Attach(entity);
			DbSet.Remove(entity);
			Context.SaveChanges();
			return true;
		}
	}
}
