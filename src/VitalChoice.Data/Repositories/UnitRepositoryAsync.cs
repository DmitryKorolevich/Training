#region

using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Data.Entity;
using VitalChoice.Data.DataContext;
using VitalChoice.Domain;

#endregion

namespace VitalChoice.Data.Repositories
{
    public class UnitRepositoryAsync<TEntity> : ReadRepositoryAsync<TEntity>,
        IUnitRepositoryAsync<TEntity> where TEntity : Entity
    {
        public UnitRepositoryAsync(IDataContextAsync context) : base(context)
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

        public virtual bool UpdateRange(IEnumerable<TEntity> entities)
        {
            foreach (var entity in entities)
            {
                DbSet.Attach(entity);
                Context.SetState(entity, EntityState.Modified);
            }
            return true;
        }

        public virtual Task<bool> UpdateRangeAsync(IEnumerable<TEntity> entities)
        {
            return UpdateRangeAsync(CancellationToken.None, entities);
        }

        public virtual Task<bool> UpdateRangeAsync(CancellationToken cancellationToken, IEnumerable<TEntity> entities)
        {
            foreach (var entity in entities)
            {
                DbSet.Attach(entity);
                Context.SetState(entity, EntityState.Modified);
            }
            return Task.FromResult(true);
        }

        public virtual bool Delete(int id)
        {
            var entity = DbSet.FirstOrDefault(x => x.Id == id);
            if (entity != null)
            {
                DbSet.Attach(entity);
                Context.SetState(entity, EntityState.Deleted);
                return true;
            }
            return false;
        }

        public virtual bool Delete(TEntity entity)
        {
            DbSet.Remove(entity);
            return true;
        }

        public virtual bool DeleteAll(ICollection<int> ids)
        {
            if (ids == null || ids.Count == 0)
                return false;
            var entitySet = DbSet.Where(e => ids.Any(id => id == e.Id));
            DbSet.AttachRange(entitySet);
            foreach (var entity in entitySet)
            {
                Context.SetState(entity, EntityState.Deleted);
            }
            return true;
        }

        public virtual bool DeleteAll(ICollection<TEntity> entitySet)
        {
            if (entitySet == null || !entitySet.Any())
                return false;
            DbSet.RemoveRange(entitySet);
            return true;
        }

        public virtual async Task<bool> DeleteAsync(int id)
        {
            return await DeleteAsync(CancellationToken.None, id);
        }

        public virtual async Task<bool> DeleteAsync(CancellationToken cancellationToken, int id)
        {
            var entity = await DbSet.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
            if (entity == null) return false;

            DbSet.Remove(entity);
            return true;
        }

        public virtual async Task<bool> DeleteAsync(TEntity entity)
        {
            return await DeleteAsync(CancellationToken.None, entity);
        }

        public virtual Task<bool> DeleteAsync(CancellationToken cancellationToken, TEntity entity)
        {
            if (entity == null)
                return Task.FromResult(false);
            DbSet.Remove(entity);
            return Task.FromResult(true);
        }

        public virtual async Task<bool> DeleteAllAsync(ICollection<int> ids)
        {
            return await DeleteAllAsync(CancellationToken.None, ids);
        }

        public virtual Task<bool> DeleteAllAsync(CancellationToken cancellationToken, ICollection<int> ids)
        {
            if (ids == null || ids.Count == 0)
                return Task.FromResult(false);
            var entitySet = DbSet.Where(e => ids.Any(id => id == e.Id));
            DbSet.AttachRange(entitySet);
            foreach (var entity in entitySet)
            {
                Context.SetState(entity, EntityState.Deleted);
            }
            return Task.FromResult(true);
        }

        public virtual async Task<bool> DeleteAllAsync(ICollection<TEntity> entitySet)
        {
            return await DeleteAllAsync(CancellationToken.None, entitySet);
        }

        public virtual Task<bool> DeleteAllAsync(CancellationToken cancellationToken,
            ICollection<TEntity> entitySet)
        {
            if (entitySet == null || !entitySet.Any())
                return Task.FromResult(false);
            DbSet.RemoveRange(entitySet);
            return Task.FromResult(true);
        }
    }
}