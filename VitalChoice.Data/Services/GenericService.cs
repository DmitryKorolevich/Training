#region

using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using VitalChoice.Data.Helpers;
using VitalChoice.Data.Infrastructure;
using VitalChoice.Data.Repositories;
using VitalChoice.Data.UnitOfWork;

#endregion

namespace VitalChoice.Data.Services
{
    public abstract class GenericService<TEntity> : IGenericService<TEntity> where TEntity : IObjectState
    {
        private readonly IRepositoryAsync<TEntity> repository;
	    private IUnitOfWorkAsync unitOfWork;

	    protected IRepositoryAsync<TEntity> Repository {
		    get { return repository; }
	    }

		protected IUnitOfWorkAsync UnitOfWork
		{
			get { return unitOfWork; }
		}

		protected GenericService(IUnitOfWorkAsync unitOfWork)
	    {
		    this.unitOfWork = unitOfWork;
            repository = this.unitOfWork.RepositoryAsync<TEntity>();
        }

        public virtual TEntity Find(params object[] keyValues)
        {
            return repository.Find(keyValues);
        }

		//public virtual IQueryable<TEntity> SelectQuery(string query, params object[] parameters)
		//{
		//	return repository.SelectQuery(query, parameters).AsQueryable();
		//}

        public virtual void Insert(TEntity entity)
        {
            repository.Insert(entity);

	        unitOfWork.SaveChanges();
        }

        public virtual void InsertRange(IEnumerable<TEntity> entities)
        {
            repository.InsertRange(entities);

			unitOfWork.SaveChanges();
		}

        public virtual void InsertGraph(TEntity entity)
        {
            repository.Insert(entity);

			unitOfWork.SaveChanges();
		}

        public virtual void InsertGraphRange(IEnumerable<TEntity> entities)
        {
            repository.InsertGraphRange(entities);

			unitOfWork.SaveChanges();
		}

        public virtual void Update(TEntity entity)
        {
            repository.Update(entity);

			unitOfWork.SaveChanges();
		}

        public virtual void Delete(object id)
        {
            repository.Delete(id);

			unitOfWork.SaveChanges();
		}

        public virtual void Delete(TEntity entity)
        {
            repository.Delete(entity);

			unitOfWork.SaveChanges();
		}

        public IEnumerable<TEntity> Query()
        {
            return repository.Query().Select();
        }

        public virtual IEnumerable<TEntity> Query(IQueryObject<TEntity> queryObject)
        {
            return repository.Query(queryObject).Select();
        }

		public virtual IEnumerable<TEntity> Query(Expression<Func<TEntity, bool>> query)
        {
            return repository.Query(query).Select();
        }

        public virtual async Task<TEntity> FindAsync(params object[] keyValues)
        {
            return await repository.FindAsync(keyValues);
        }

        public virtual async Task<TEntity> FindAsync(CancellationToken cancellationToken, params object[] keyValues)
        {
            return await repository.FindAsync(cancellationToken, keyValues);
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