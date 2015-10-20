#region

using System;
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
        IRepositoryAsync<TEntity> where TEntity : Entity
    {
        public UnitRepositoryAsync(IDataContextAsync context) : base(context)
        {
        }

        public virtual bool Insert(TEntity entity)
        {
            if (entity == null)
                return false;
            DbSet.Add(entity);
            return true;
        }

        public virtual Task<bool> InsertAsync(TEntity entity)
        {
            if (entity == null)
                return Task.FromResult(false);
            return InsertAsync(CancellationToken.None, entity);
        }

        public virtual Task<bool> InsertAsync(CancellationToken cancellationToken, TEntity entity)
        {
            if (entity == null)
                return Task.FromResult(false);
            DbSet.Add(entity);
            return Task.FromResult(true);
        }

        public virtual bool InsertRange(IEnumerable<TEntity> entities)
        {
            if (entities == null)
                return false;
            DbSet.AddRange(entities);
            return true;
        }

        public virtual Task<bool> InsertRangeAsync(IEnumerable<TEntity> entities)
        {
            return InsertRangeAsync(CancellationToken.None, entities);
        }

        public virtual Task<bool> InsertRangeAsync(CancellationToken cancellationToken, IEnumerable<TEntity> entities)
        {
            if (entities == null)
                return Task.FromResult(false);
            DbSet.AddRange(entities);
            return Task.FromResult(true);
        }

        public virtual bool InsertGraph(TEntity entity)
        {
            if (entity == null)
                return false;
            Context.TrackGraphForAdd(entity);
            return true;
        }

        public virtual Task<bool> InsertGraphAsync(TEntity entity)
        {
            return InsertGraphAsync(CancellationToken.None, entity);
        }

        public virtual Task<bool> InsertGraphAsync(CancellationToken cancellationToken, TEntity entity)
        {
            if (entity == null)
                return Task.FromResult(false);
            Context.TrackGraphForAdd(entity);
            return Task.FromResult(true);
        }

        public virtual bool InsertGraphRange(params TEntity[] entities)
        {
            if (entities == null)
                return false;
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
            if (entities == null)
                return Task.FromResult(false);
            foreach (var entity in entities)
            {
                Context.TrackGraphForAdd(entity);
            }
            return Task.FromResult(true);
        }

        public virtual async Task<bool> InsertGraphRangeAsync(IEnumerable<TEntity> entities)
        {
            return await InsertGraphRangeAsync(CancellationToken.None, entities);
        }

        public virtual Task<bool> InsertGraphRangeAsync(CancellationToken cancellationToken, IEnumerable<TEntity> entities)
        {
            if (entities == null)
                return Task.FromResult(false);
            foreach (var entity in entities)
            {
                Context.TrackGraphForAdd(entity);
            }
            return Task.FromResult(true);
        }

        public virtual bool Update(TEntity entity)
        {
            if (entity == null)
                return false;
            DbSet.Update(entity);
            return true;
        }

        public virtual Task<bool> UpdateAsync(TEntity entity)
        {
            return UpdateAsync(CancellationToken.None, entity);
        }

        public virtual Task<bool> UpdateAsync(CancellationToken cancellationToken, TEntity entity)
        {
            if (entity == null)
                return Task.FromResult(false);
            DbSet.Update(entity);
            return Task.FromResult(true);
        }

        public virtual bool UpdateRange(IEnumerable<TEntity> entities)
        {
            if (entities == null)
                return false;
            DbSet.UpdateRange(entities);
            return true;
        }

        public virtual Task<bool> UpdateRangeAsync(IEnumerable<TEntity> entities)
        {
            return UpdateRangeAsync(CancellationToken.None, entities);
        }

        public virtual Task<bool> UpdateRangeAsync(CancellationToken cancellationToken, IEnumerable<TEntity> entities)
        {
            if (entities == null)
                return Task.FromResult(false);
            DbSet.UpdateRange(entities);
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
            if (entity == null)
                return false;
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

        public virtual bool DeleteAll(IEnumerable<TEntity> entitySet)
        {
            if (entitySet == null)
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

        public virtual async Task<bool> DeleteAllAsync(IEnumerable<TEntity> entitySet)
        {
            return await DeleteAllAsync(CancellationToken.None, entitySet);
        }

        public virtual Task<bool> DeleteAllAsync(CancellationToken cancellationToken,
            IEnumerable<TEntity> entitySet)
        {
            if (entitySet == null)
                return Task.FromResult(false);
            DbSet.RemoveRange(entitySet);
            return Task.FromResult(true);
        }

        public void Detach(TEntity entity)
        {
            Context.SetState(entity, EntityState.Detached);
        }

        public void DetachAll(IEnumerable<TEntity> entities)
        {
            foreach (var entity in entities)
            {
                Context.SetState(entity, EntityState.Detached);
            }
        }
    }
}