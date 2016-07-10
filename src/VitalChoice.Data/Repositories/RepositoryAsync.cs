using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using VitalChoice.Data.Context;
using VitalChoice.Ecommerce.Domain;

namespace VitalChoice.Data.Repositories
{
    public class RepositoryAsync<TEntity> : ReadRepositoryAsync<TEntity>, IRepositoryAsync<TEntity>
        where TEntity : Entity
    {
        public RepositoryAsync(IDataContextAsync context) : base(context)
        {
        }

        public virtual bool Insert(TEntity entity)
        {
            DbSet.Add(entity);
            Context.SaveChanges();
            return true;
        }

        public virtual Task<bool> InsertAsync(TEntity entity)
        {
            return InsertAsync(CancellationToken.None, entity);
        }

        public virtual async Task<bool> InsertAsync(CancellationToken cancellationToken, TEntity entity)
        {
            if (entity != null)
            {
                DbSet.Add(entity);
                await Context.SaveChangesAsync(cancellationToken);
                return true;
            }
            return false;
        }

        public virtual bool InsertRange(IEnumerable<TEntity> entities)
        {
            if (entities != null)
            {
                DbSet.AddRange(entities);
                Context.SaveChanges();
                return true;
            }
            return false;
        }

        public virtual Task<bool> InsertRangeAsync(IEnumerable<TEntity> entities)
        {
            return InsertRangeAsync(CancellationToken.None, entities);
        }

        public virtual async Task<bool> InsertRangeAsync(CancellationToken cancellationToken,
            IEnumerable<TEntity> entities)
        {
            if (entities != null)
            {
                DbSet.AddRange(entities);
                await Context.SaveChangesAsync(cancellationToken);
                return true;
            }
            return false;
        }

        public virtual bool InsertGraph(TEntity entity)
        {
            if (entity != null)
            {
                Context.TrackGraphForAdd(entity);
                Context.SaveChanges();
                return true;
            }
            return false;
        }

        public virtual Task<bool> InsertGraphAsync(TEntity entity)
        {
            return InsertGraphAsync(CancellationToken.None, entity);
        }

        public virtual async Task<bool> InsertGraphAsync(CancellationToken cancellationToken, TEntity entity)
        {
            if (entity != null)
            {
                Context.TrackGraphForAdd(entity);
                await Context.SaveChangesAsync(cancellationToken);
                return true;
            }
            return false;
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

        public virtual Task<bool> InsertGraphRangeAsync(params TEntity[] entities)
        {
            return InsertGraphRangeAsync(CancellationToken.None, entities);
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

        public virtual Task<bool> InsertGraphRangeAsync(IEnumerable<TEntity> entities)
        {
            return InsertGraphRangeAsync(CancellationToken.None, entities);
        }

        public virtual async Task<bool> InsertGraphRangeAsync(CancellationToken cancellationToken,
            IEnumerable<TEntity> entities)
        {
            foreach (var entity in entities)
            {
                Context.TrackGraphForAdd(entity);
            }
            await Context.SaveChangesAsync(CancellationToken.None);
            return true;
        }

        public virtual bool Update(TEntity entity)
        {
            if (entity != null)
            {
                DbSet.Update(entity);
                Context.SaveChanges();
                return true;
            }
            return false;
        }

        public virtual Task<bool> UpdateAsync(TEntity entity)
        {
            return UpdateAsync(CancellationToken.None, entity);
        }

        public virtual async Task<bool> UpdateAsync(CancellationToken cancellationToken, TEntity entity)
        {
            if (entity != null)
            {
                DbSet.Update(entity);
                await Context.SaveChangesAsync(cancellationToken);
                return true;
            }
            return false;
        }

        public virtual bool UpdateRange(IEnumerable<TEntity> entities)
        {
            if (entities != null)
            {
                DbSet.UpdateRange(entities);
                Context.SaveChanges();
                return true;
            }
            return false;
        }

        public virtual Task<bool> UpdateRangeAsync(IEnumerable<TEntity> entities)
        {
            return UpdateRangeAsync(CancellationToken.None, entities);
        }

        public virtual async Task<bool> UpdateRangeAsync(CancellationToken cancellationToken,
            IEnumerable<TEntity> entities)
        {
            if (entities != null)
            {
                DbSet.UpdateRange(entities);
                await Context.SaveChangesAsync(cancellationToken);
                return true;
            }
            return false;
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
            if (entity != null)
            {
                DbSet.Remove(entity);
                Context.SaveChanges();
                return true;
            }
            return false;
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

        public virtual bool DeleteAll(IEnumerable<TEntity> entitySet)
        {
            if (entitySet == null)
                return false;
            DbSet.RemoveRange(entitySet);
            Context.SaveChanges();
            return true;
        }

        public virtual Task<bool> DeleteAsync(int id)
        {
            return DeleteAsync(CancellationToken.None, id);
        }

        public virtual async Task<bool> DeleteAsync(CancellationToken cancellationToken, int id)
        {
            var entity = await DbSet.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
            if (entity == null) return false;

            DbSet.Remove(entity);
            await Context.SaveChangesAsync(cancellationToken);
            return true;
        }

        public virtual Task<bool> DeleteAsync(TEntity entity)
        {
            return DeleteAsync(CancellationToken.None, entity);
        }

        public virtual async Task<bool> DeleteAsync(CancellationToken cancellationToken, TEntity entity)
        {
            if (entity == null)
                return false;
            DbSet.Remove(entity);
            await Context.SaveChangesAsync(cancellationToken);
            return true;
        }

        public virtual Task<bool> DeleteAllAsync(ICollection<int> ids)
        {
            return DeleteAllAsync(CancellationToken.None, ids);
        }

        public virtual async Task<bool> DeleteAllAsync(CancellationToken cancellationToken, ICollection<int> ids)
        {
            if (ids == null || ids.Count == 0)
                return false;
            var entitySet = DbSet.Where(e => ids.Contains(e.Id));
            foreach (var entity in entitySet)
            {
                Context.SetState(entity, EntityState.Deleted);
            }
            await Context.SaveChangesAsync(cancellationToken);
            return true;
        }

        public virtual Task<bool> DeleteAllAsync(IEnumerable<TEntity> entitySet)
        {
            return DeleteAllAsync(CancellationToken.None, entitySet);
        }

        public virtual async Task<bool> DeleteAllAsync(CancellationToken cancellationToken,
            IEnumerable<TEntity> entitySet)
        {
            if (entitySet == null)
                return false;
            DbSet.RemoveRange(entitySet);
            await Context.SaveChangesAsync(cancellationToken);
            return true;
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