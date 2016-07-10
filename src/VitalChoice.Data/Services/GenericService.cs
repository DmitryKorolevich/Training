#region

using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using VitalChoice.Data.Helpers;
using VitalChoice.Data.Repositories;
using VitalChoice.Ecommerce.Domain;

#endregion

namespace VitalChoice.Data.Services
{
    public class GenericService<TEntity> : IGenericService<TEntity> where TEntity : Entity
    {
	    protected IRepositoryAsync<TEntity> Repository { get; }

		public GenericService(IRepositoryAsync<TEntity> repository)
	    {
		    this.Repository = repository;
        }

        public virtual void Insert(TEntity entity)
        {
			Repository.Insert(entity);
        }

	    public Task InsertAsync(TEntity entity)
	    {
		    return Repository.InsertAsync(entity);
	    }

	    public virtual bool InsertRange(ICollection<TEntity> entities)
        {
			return Repository.InsertRange(entities);
		}

	    public virtual Task<bool> InsertRangeAsync(ICollection<TEntity> entities)
	    {
			return Repository.InsertRangeAsync(entities);
		}

	    public virtual void InsertGraph(TEntity entity)
	    {
            Repository.Insert(entity);
		}

        public virtual Task InsertGraphAsync(TEntity entity)
        {
            return Repository.InsertGraphAsync(entity);
        }

        public virtual bool InsertGraphRange(params TEntity[] entities)
        {
			return Repository.InsertGraphRange(entities);
		}

	    public virtual async Task<bool> InsertGraphRangeAsync(params TEntity[] entities)
	    {
			return await Repository.InsertGraphRangeAsync(entities);
		}

	    public virtual void Update(TEntity entity)
	    {
	        Repository.Update(entity);
		}

	    public virtual Task UpdateAsync(TEntity entity)
	    {
	        return Repository.UpdateAsync(entity);
	    }

	    public virtual void Delete(int id)
        {
			Repository.Delete(id);
		}

        public virtual void Delete(TEntity entity)
        {
			Repository.Delete(entity);
		}

        public List<TEntity> Query()
        {
            return Repository.Query().Select(false);
        }

        public virtual List<TEntity> Query(IQueryObject<TEntity> queryObject)
        {
            return Repository.Query(queryObject).Select(false);
        }

	    public async Task<List<TEntity>> QueryAsync(IQueryObject<TEntity> queryObject)
	    {
			return await Repository.Query(queryObject).SelectAsync(false);
		}

	    public virtual List<TEntity> Query(Expression<Func<TEntity, bool>> query)
        {
            return Repository.Query(query).Select(false);
        }

        public async Task<TEntity> Query(int id)
        {
            var res = await Repository.Query(x => x.Id == id).SelectAsync(false);

            return res.FirstOrDefault();
        }

        public virtual async Task<bool> DeleteAsync(int id)
        {
            return await DeleteAsync(CancellationToken.None, id);
        }

        public virtual async Task<bool> DeleteAsync(CancellationToken cancellationToken, int id)
        {
            return await Repository.DeleteAsync(cancellationToken, id);
        }
    }
}