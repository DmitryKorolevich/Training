#region

using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using VitalChoice.Data.Helpers;
using VitalChoice.Data.Infrastructure;
using VitalChoice.Data.Repositories;

#endregion

namespace VitalChoice.Data.Services
{
    public abstract class GenericService<TEntity> : IGenericService<TEntity> where TEntity : IObjectState
    {
	    protected IRepositoryAsync<TEntity> Repository { get; }

	    protected GenericService(IRepositoryAsync<TEntity> repository)
	    {
		    this.Repository = repository;
        }

        public virtual TEntity Find(params object[] keyValues)
        {
            return Repository.Find(keyValues);
        }

		//public virtual IQueryable<TEntity> SelectQuery(string query, params object[] parameters)
		//{
		//	return repository.SelectQuery(query, parameters).AsQueryable();
		//}

        public virtual void Insert(TEntity entity)
        {
			Repository.Insert(entity);
        }

        public virtual void InsertRange(IEnumerable<TEntity> entities)
        {
			Repository.InsertRange(entities);
		}

        public virtual void InsertGraph(TEntity entity)
        {
			Repository.Insert(entity);
		}

        public virtual void InsertGraphRange(IEnumerable<TEntity> entities)
        {
			Repository.InsertGraphRange(entities);
		}

        public virtual void Update(TEntity entity)
        {
			Repository.Update(entity);
		}

        public virtual void Delete(object id)
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

        public virtual async Task<TEntity> FindAsync(params object[] keyValues)
        {
            return await Repository.FindAsync(keyValues);
        }

        public virtual async Task<TEntity> FindAsync(CancellationToken cancellationToken, params object[] keyValues)
        {
            return await Repository.FindAsync(cancellationToken, keyValues);
        }

        public virtual async Task<bool> DeleteAsync(params object[] keyValues)
        {
            return await DeleteAsync(CancellationToken.None, keyValues);
        }

        public virtual async Task<bool> DeleteAsync(CancellationToken cancellationToken, params object[] keyValues)
        {
            return await DeleteAsync(cancellationToken, keyValues);
        }
    }
}