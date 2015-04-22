#region

using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using VitalChoice.Data.Helpers;
using VitalChoice.Data.Repositories;
using VitalChoice.Domain;

#endregion

namespace VitalChoice.Data.Services
{
    public abstract class GenericService<TEntity> : IGenericService<TEntity> where TEntity : Entity
    {
	    protected IRepositoryAsync<TEntity> Repository { get; }

		protected GenericService(IRepositoryAsync<TEntity> repository)
	    {
		    this.Repository = repository;
        }

        public virtual void Insert(TEntity entity)
        {
			Repository.Insert(entity);
        }

	    public async Task InsertAsync(TEntity entity)
	    {
		    await Repository.InsertAsync(entity);
	    }

	    public virtual bool InsertRange(IEnumerable<TEntity> entities)
        {
			return Repository.InsertRange(entities);
		}

	    public virtual async Task<bool> InsertRangeAsync(IEnumerable<TEntity> entities)
	    {
			return await Repository.InsertRangeAsync(entities);
		}

	    public virtual TEntity InsertGraph(TEntity entity)
        {
			return Repository.Insert(entity);
		}

	    public virtual async Task<TEntity> InsertGraphAsync(TEntity entity)
	    {
			return await Repository.InsertGraphAsync(entity);
		}

	    public virtual bool InsertGraphRange(params TEntity[] entities)
        {
			return Repository.InsertGraphRange(entities);
		}

	    public virtual async Task<bool> InsertGraphRangeAsync(params TEntity[] entities)
	    {
			return await Repository.InsertGraphRangeAsync(entities);
		}

	    public virtual TEntity Update(TEntity entity)
        {
			return Repository.Update(entity);
		}

	    public virtual async Task<TEntity> UpdateAsync(TEntity entity)
	    {
		    return await Repository.UpdateAsync(entity);
	    }

	    public virtual void Delete(int id)
        {
			Repository.Delete(id);
		}

        public virtual void Delete(TEntity entity)
        {
			Repository.Delete(entity);
		}

        public IEnumerable<TEntity> Query()
        {
            return Repository.Query().Select();
        }

        public virtual IEnumerable<TEntity> Query(IQueryObject<TEntity> queryObject)
        {
            return Repository.Query(queryObject).Select();
        }

		public virtual IEnumerable<TEntity> Query(Expression<Func<TEntity, bool>> query)
        {
            return Repository.Query(query).Select();
        }

		public virtual async Task<bool> DeleteAsync(int id)
        {
            return await DeleteAsync(CancellationToken.None, id);
        }

        public virtual async Task<bool> DeleteAsync(CancellationToken cancellationToken, int id)
        {
            return await DeleteAsync(cancellationToken, id);
        }
    }
}