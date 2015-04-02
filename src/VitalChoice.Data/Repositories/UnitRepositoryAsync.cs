#region

using Microsoft.Data.Entity;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using VitalChoice.Data.DataContext;
using VitalChoice.Domain;
using VitalChoice.Domain.Infrastructure;

#endregion

namespace VitalChoice.Data.Repositories
{
    public class UnitRepositoryAsync<TEntity> : ReadRepositoryAsync<TEntity>,
    IUnitRepositoryAsync<TEntity> where TEntity : Entity
    {
        public UnitRepositoryAsync(IDataContextAsync context) :base(context)
        {
        }

		public virtual void Insert(TEntity entity)
		{
			DbSet.Attach(entity);
			Context.SetState(entity, EntityState.Added);
		}

		public virtual void InsertRange(IEnumerable<TEntity> entities)
		{
			foreach (var entity in entities)
				Insert(entity);
		}

		public virtual void InsertGraph(TEntity entity)
		{
			DbSet.Add(entity);
		}

		public virtual void InsertGraphRange(params TEntity[] entities)
		{
			DbSet.AddRange(entities);
		}

        public virtual void Update(TEntity entity)
        {
            DbSet.Attach(entity);
            Context.SetState(entity, EntityState.Modified);
        }

        public virtual async Task<bool> UpdateAsync(TEntity entity)
        {
            return await UpdateAsync(CancellationToken.None, entity);
        }

        public async Task<bool> UpdateAsync(CancellationToken cancellationToken, TEntity entity)
        {
            DbSet.Attach(entity);
            DbSet.Update(entity);
            Context.SetState(entity, EntityState.Modified);
            return true;
        }

        public virtual void Delete(int id)
		{
			var entity = DbSet.FirstOrDefault(x => x.Id == id);
			if (entity != null)
			{
				Delete(entity);
			}
		}

		public virtual void Delete(TEntity entity)
		{
			DbSet.Attach(entity);
			Context.SetState(entity, EntityState.Deleted);
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
			return true;
		}
	}
}