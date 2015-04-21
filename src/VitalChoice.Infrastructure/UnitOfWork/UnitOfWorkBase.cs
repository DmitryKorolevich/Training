using System.Threading;
using System.Threading.Tasks;
using VitalChoice.Data.Repositories;
using VitalChoice.Data.UnitOfWork;
using VitalChoice.Domain;

namespace VitalChoice.Infrastructure.UnitOfWork
{
    public abstract class UnitOfWorkBase : IUnitOfWorkAsync
	{
		private IUnitOfWorkAsync uow;

	    public UnitOfWorkBase()
	    {
		    this.uow = this.Init();
	    }

	    protected abstract IUnitOfWorkAsync Init();

		public void Dispose()
		{
			uow.Dispose();
		}

		public void Dispose(bool disposing)
		{
			uow.Dispose(disposing);
		}

		public IUnitRepositoryAsync<TEntity> RepositoryAsync<TEntity>() where TEntity : Entity
		{
			return uow.RepositoryAsync<TEntity>();
		}

		public int SaveChanges()
		{
			return uow.SaveChanges();
		}

		public Task<int> SaveChangesAsync(CancellationToken cancellationToken)
		{
			return uow.SaveChangesAsync(cancellationToken);
		}
	}
}