using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Data.Entity;
using VitalChoice.Data.DataContext;
using VitalChoice.Domain;

namespace VitalChoice.Data.Repositories
{
	public class RepositoryAsync<TEntity> : ReadRepositoryAsync<TEntity>, IRepositoryAsync<TEntity> where TEntity : Entity
	{
		public RepositoryAsync(IDataContextAsync context) : base(context)
		{
		}

		public virtual TEntity Insert(TEntity entity)
        {
            TEntity toReturn = null;
            DbSet.Add(entity);
			Context.SaveChanges();
            toReturn = entity;
            return toReturn;
        }

        public virtual async Task<TEntity> InsertAsync(TEntity entity)
        {
            return await InsertAsync(CancellationToken.None, entity);
        }

        public virtual async Task<TEntity> InsertAsync(CancellationToken cancellationToken, TEntity entity)
        {
            TEntity toReturn = null;
            DbSet.Add(entity);
            await Context.SaveChangesAsync(cancellationToken);
            toReturn = entity;
            return toReturn;
        }

        public virtual bool InsertRange(IEnumerable<TEntity> entities)
		{
            foreach (var entity in entities)
            {
                DbSet.Add(entity);
            }
			Context.SaveChanges();
            return true;
		}

        public virtual async Task<bool> InsertRangeAsync(IEnumerable<TEntity> entities)
        {
            return await InsertRangeAsync(CancellationToken.None, entities);
        }

        public virtual async Task<bool> InsertRangeAsync(CancellationToken cancellationToken, IEnumerable<TEntity> entities)
        {
            foreach (var entity in entities)
            {
                DbSet.Add(entity);
            }
            await Context.SaveChangesAsync(cancellationToken);
            return true;
        }

        public virtual TEntity InsertGraph(TEntity entity)
        {
            TEntity toReturn = null;
            Context.TrackGraphForAdd(entity);
            Context.SaveChanges();
            toReturn = entity;
            return toReturn;
        }

        public virtual async Task<TEntity> InsertGraphAsync(TEntity entity)
        {
            return await InsertGraphAsync(CancellationToken.None, entity);
        }

        public virtual async Task<TEntity> InsertGraphAsync(CancellationToken cancellationToken, TEntity entity)
        {
            TEntity toReturn = null;
            Context.TrackGraphForAdd(entity);
            await Context.SaveChangesAsync(cancellationToken);
            toReturn = entity;
            return toReturn;
        }

        public virtual bool InsertGraphRange(params TEntity[] entities)
		{
            foreach(var entity in entities)
            {
                Context.TrackGraphForAdd(entity);
            }
			Context.SaveChanges();
            return true;
		}

        public virtual async Task<bool> InsertGraphRangeAsync(params TEntity[] entities)
        {
            return await InsertGraphRangeAsync(CancellationToken.None, entities);
        }

        public virtual async Task<bool> InsertGraphRangeAsync(CancellationToken cancellationToken, params TEntity[] entities)
        {
            foreach (var entity in entities)
            {
                Context.TrackGraphForAdd(entity);
            }
            await Context.SaveChangesAsync(cancellationToken);
            return true;
        }
        
        public virtual TEntity Update(TEntity entity)
        {
            TEntity toReturn;
            DbSet.Attach(entity);
			Context.SetState(entity, EntityState.Modified);
			Context.SaveChanges();
            toReturn = entity;
            return toReturn;
        }

        public virtual async Task<TEntity> UpdateAsync(TEntity entity) {
            return await UpdateAsync(CancellationToken.None, entity);
        }

	    public virtual async Task<TEntity> UpdateAsync(CancellationToken cancellationToken, TEntity entity)
	    {
            TEntity toReturn;
            DbSet.Attach(entity);
            Context.SetState(entity, EntityState.Modified);
            await Context.SaveChangesAsync(cancellationToken);
            toReturn = entity;
            return toReturn;
        }

	    public virtual void Delete(int id)
		{
			var entity = DbSet.FirstOrDefault(x=>x.Id == id);
			if (entity!= null)
			{
				Delete(entity);
			}
		}

		public virtual void Delete(TEntity entity)
		{
			DbSet.Attach(entity);
			Context.SetState(entity, EntityState.Deleted);
			Context.SaveChanges();
		}

		public virtual async Task<bool> DeleteAsync(int id)
		{
			return await DeleteAsync(CancellationToken.None, id);
		}

		public virtual async Task<bool> DeleteAsync(CancellationToken cancellationToken, int id)
		{
			var entity = await DbSet.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
			if (entity == null) return false;

			DbSet.Attach(entity);
			DbSet.Remove(entity);
			await Context.SaveChangesAsync(cancellationToken);
			return true;
		}
    }
}
