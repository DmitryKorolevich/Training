using System.Data;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using VitalChoice.Data.Context;
using VitalChoice.Data.Repositories;
using VitalChoice.Data.Transaction;
using VitalChoice.Ecommerce.Domain;
using VitalChoice.Ecommerce.Domain.Options;

namespace VitalChoice.Data.UnitOfWork
{
    public class UnitOfWorkBase : IUnitOfWorkAsync
	{
		private readonly IUnitOfWorkAsync _uow;

        protected static IOptions<AppOptionsBase> Options;

        public UnitOfWorkBase(IDataContextAsync context)
	    {
	        _uow = new UnitOfWork(context);
	    }

        public static void SetOptions(IOptions<AppOptionsBase> options)
        {
            Options = options;
        }

	    public void Dispose()
		{
			_uow.Dispose();
		}

		public void Dispose(bool disposing)
		{
			_uow.Dispose(disposing);
		}

	    public IScopedTransaction BeginTransaction(IsolationLevel isolation = IsolationLevel.ReadCommitted)
	    {
		    return _uow.BeginTransaction(isolation);
	    }

	    public IRepositoryAsync<TEntity> RepositoryAsync<TEntity>() where TEntity : Entity
		{
			return _uow.RepositoryAsync<TEntity>();
		}

        public IReadRepositoryAsync<TEntity> ReadRepositoryAsync<TEntity>() where TEntity : Entity
        {
            return _uow.ReadRepositoryAsync<TEntity>();
        }

        public int SaveChanges()
		{
			return _uow.SaveChanges();
		}

        public Task<int> SaveChangesAsync()
        {
            return _uow.SaveChangesAsync();
        }

        public Task<int> SaveChangesAsync(CancellationToken cancellationToken)
		{
			return _uow.SaveChangesAsync(cancellationToken);
		}
	}
}