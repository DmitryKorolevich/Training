#region

using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using VitalChoice.Data.Helpers;
using VitalChoice.Data.Repositories;
using VitalChoice.Domain.Infrastructure;

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

        public virtual void InsertGraphRange(params TEntity[] entities)
        {
			Repository.InsertGraphRange(entities);
		}

        public virtual void Update(TEntity entity)
        {
			Repository.Update(entity);
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