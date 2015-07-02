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
		private readonly IUnitOfWorkAsync _uow;

	    protected static IOptions<AppOptions> Options { get; private set; }

	    protected UnitOfWorkBase()
	    {
		    this._uow = this.Init();
	    }

	    protected abstract IUnitOfWorkAsync Init();

	    public static void SetOptions(IOptions<AppOptions> options)
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

		public IUnitRepositoryAsync<TEntity> RepositoryAsync<TEntity>() where TEntity : Entity
		{
			return _uow.RepositoryAsync<TEntity>();
		}

		public int SaveChanges()
		{
			return _uow.SaveChanges();
		}

		public Task<int> SaveChangesAsync(CancellationToken cancellationToken)
		{
			return _uow.SaveChangesAsync(cancellationToken);
		}
	}
}