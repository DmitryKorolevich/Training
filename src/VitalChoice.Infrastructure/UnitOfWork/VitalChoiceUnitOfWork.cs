using System;
using System.Threading;
using System.Threading.Tasks;
using VitalChoice.Data.Repositories;
using VitalChoice.Data.UnitOfWork;
using VitalChoice.Domain;
using VitalChoice.Domain.Infrastructure;
using VitalChoice.Infrastructure.Context;

namespace VitalChoice.Infrastructure.UnitOfWork
{
    public class VitalChoiceUnitOfWork : IUnitOfWorkAsync
	{
		private IUnitOfWorkAsync uow;

		public VitalChoiceUnitOfWork() {
			var context = new VitalChoiceContext();
			//context.Database.Connection = new DataStoreConnection();
            uow = new Data.UnitOfWork.UnitOfWork(context);
		}

		public void BeginTransaction()
		{
			uow.BeginTransaction();
		}

		public bool Commit()
		{
			return uow.Commit();
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

		public void Rollback()
		{
			uow.Rollback();
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