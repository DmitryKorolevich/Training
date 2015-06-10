using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Data.Entity;
using VitalChoice.Data.DataContext;
using VitalChoice.Domain;

namespace VitalChoice.Data.Repositories
{
    public class RepositoryAsync<TEntity> : ReadRepositoryAsync<TEntity>, IRepositoryAsync<TEntity>
        where TEntity : Entity
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

        public virtual async Task<bool> InsertRangeAsync(CancellationToken cancellationToken,
            IEnumerable<TEntity> entities)
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
            foreach (var entity in entities)
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

        public virtual async Task<bool> InsertGraphRangeAsync(CancellationToken cancellationToken,
            params TEntity[] entities)
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

        public virtual async Task<TEntity> UpdateAsync(TEntity entity)
        {
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

        public virtual bool UpdateRange(IEnumerable<TEntity> entities)
        {
            foreach (var entity in entities)
            {
                DbSet.Attach(entity);
                Context.SetState(entity, EntityState.Modified);
            }
            Context.SaveChanges();
            return true;
        }

        public virtual async Task<bool> UpdateRangeAsync(IEnumerable<TEntity> entities)
        {
            return await UpdateRangeAsync(CancellationToken.None, entities);
        }

        public virtual async Task<bool> UpdateRangeAsync(CancellationToken cancellationToken,
            IEnumerable<TEntity> entities)
        {
            foreach (var entity in entities)
            {
                DbSet.Attach(entity);
                Context.SetState(entity, EntityState.Modified);
            }
            await Context.SaveChangesAsync(cancellationToken);
            return true;
        }

        public virtual bool Delete(int id)
        {
            var entity = DbSet.FirstOrDefault(x => x.Id == id);
            if (entity != null)
            {
                DbSet.Attach(entity);
                Context.SetState(entity, EntityState.Deleted);
                Context.SaveChanges();
                return true;
            }
            return false;
        }

        public virtual bool Delete(TEntity entity)
        {
            DbSet.Remove(entity);
            Context.SaveChanges();
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
            Context.SaveChanges();
            return true;
        }

        public virtual bool DeleteAll(ICollection<TEntity> entitySet)
        {
            if (entitySet == null || !entitySet.Any())
                return false;
            DbSet.RemoveRange(entitySet);
            Context.SaveChanges();
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
            await Context.SaveChangesAsync(cancellationToken);
            return true;
        }

        public virtual async Task<bool> DeleteAsync(TEntity entity)
        {
            return await DeleteAsync(CancellationToken.None, entity);
        }

        public virtual async Task<bool> DeleteAsync(CancellationToken cancellationToken, TEntity entity)
        {
            if (entity == null)
                return false;
            DbSet.Remove(entity);
            await Context.SaveChangesAsync(cancellationToken);
            return true;
        }

        public virtual async Task<bool> DeleteAllAsync(ICollection<int> ids)
        {
            return await DeleteAllAsync(CancellationToken.None, ids);
        }

        public virtual async Task<bool> DeleteAllAsync(CancellationToken cancellationToken, ICollection<int> ids)
        {
            if (ids == null || ids.Count == 0)
                return false;
            var entitySet = DbSet.Where(e => ids.Any(id => id == e.Id));
            DbSet.AttachRange(entitySet);
            foreach (var entity in entitySet)
            {
                Context.SetState(entity, EntityState.Deleted);
            }
            await Context.SaveChangesAsync(cancellationToken);
            return true;
        }

        public virtual async Task<bool> DeleteAllAsync(ICollection<TEntity> entitySet)
        {
            return await DeleteAllAsync(CancellationToken.None, entitySet);
        }

        public virtual async Task<bool> DeleteAllAsync(CancellationToken cancellationToken,
            ICollection<TEntity> entitySet)
        {
            if (entitySet == null || !entitySet.Any())
                return false;
            DbSet.RemoveRange(entitySet);
            await Context.SaveChangesAsync(cancellationToken);
            return true;
        }
    }
}