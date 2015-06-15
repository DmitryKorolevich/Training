using System.Threading;
using System.Threading.Tasks;
using Microsoft.Framework.OptionsModel;
using VitalChoice.Data.Repositories;
using VitalChoice.Data.UnitOfWork;
using VitalChoice.Domain;
using VitalChoice.Domain.Entities.Options;

namespace VitalChoice.Infrastructure.UnitOfWork
{
    public abstract class UnitOfWorkBase : IUnitOfWorkAsync
	{
		private IUnitOfWorkAsync uow;

	    protected static IOptions<AppOptions> Options { get; private set; }

	    public UnitOfWorkBase()
	    {
		    this.uow = this.Init();
	    }

	    protected abstract IUnitOfWorkAsync Init();

	    public static void SetOptions(IOptions<AppOptions> options)
	    {
		    Options = options;
	    }

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