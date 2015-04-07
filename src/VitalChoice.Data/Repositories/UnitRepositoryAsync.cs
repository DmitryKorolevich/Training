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

        public virtual TEntity Insert(TEntity entity)
        {
            TEntity toReturn = null;
            DbSet.Add(entity);
            toReturn = entity;
            return toReturn;
        }

        public virtual Task<TEntity> InsertAsync(TEntity entity)
        {
            return InsertAsync(CancellationToken.None, entity);
        }

        public virtual Task<TEntity> InsertAsync(CancellationToken cancellationToken, TEntity entity)
        {
            TEntity toReturn = null;
            DbSet.Add(entity);
            toReturn = entity;
            return Task.FromResult(toReturn);
        }

        public virtual bool InsertRange(IEnumerable<TEntity> entities)
        {
            foreach (var entity in entities)
            {
                DbSet.Add(entity);
            }
            return true;
        }

        public virtual Task<bool> InsertRangeAsync(IEnumerable<TEntity> entities)
        {
            return InsertRangeAsync(CancellationToken.None, entities);
        }

        public virtual Task<bool> InsertRangeAsync(CancellationToken cancellationToken, IEnumerable<TEntity> entities)
        {
            foreach (var entity in entities)
            {
                DbSet.Add(entity);
            }
            return Task.FromResult(true);
        }

        public virtual TEntity InsertGraph(TEntity entity)
        {
            TEntity toReturn = null;
            Context.TrackGraphForAdd(entity);
            toReturn = entity;
            return toReturn;
        }

        public virtual Task<TEntity> InsertGraphAsync(TEntity entity)
        {
            return InsertGraphAsync(CancellationToken.None, entity);
        }

        public virtual Task<TEntity> InsertGraphAsync(CancellationToken cancellationToken, TEntity entity)
        {
            TEntity toReturn = null;
            Context.TrackGraphForAdd(entity);
            toReturn = entity;
            return Task.FromResult(toReturn);
        }

        public virtual bool InsertGraphRange(params TEntity[] entities)
        {
            foreach (var entity in entities)
            {
                Context.TrackGraphForAdd(entity);
            }
            return true;
        }

        public virtual Task<bool> InsertGraphRangeAsync(params TEntity[] entities)
        {
            return InsertGraphRangeAsync(CancellationToken.None, entities);
        }

        public virtual Task<bool> InsertGraphRangeAsync(CancellationToken cancellationToken, params TEntity[] entities)
        {
            foreach (var entity in entities)
            {
                Context.TrackGraphForAdd(entity);
            }
            return Task.FromResult(true);
        }

        public virtual TEntity Update(TEntity entity)
        {
            TEntity toReturn;
            DbSet.Attach(entity);
            Context.SetState(entity, EntityState.Modified);
            toReturn = entity;
            return toReturn;
        }

        public virtual Task<TEntity> UpdateAsync(TEntity entity)
        {
            return UpdateAsync(CancellationToken.None, entity);
        }

        public virtual Task<TEntity> UpdateAsync(CancellationToken cancellationToken, TEntity entity)
        {
            TEntity toReturn;
            DbSet.Attach(entity);
            Context.SetState(entity, EntityState.Modified);
            toReturn = entity;
            return Task.FromResult(toReturn);
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

        public virtual Task<bool> DeleteAsync(CancellationToken cancellationToken, int id)
        {
            var entity = DbSet.FirstOrDefault(x => x.Id == id);
            if (entity == null) return Task.FromResult(false);

            DbSet.Attach(entity);
            DbSet.Remove(entity);
            return Task.FromResult(true);
        }
    }
}