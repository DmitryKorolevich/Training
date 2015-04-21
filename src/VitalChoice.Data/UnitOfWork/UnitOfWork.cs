#region

using System;
using System.Collections.Generic;
using System.Data;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Data.Entity;
using Microsoft.Data.Entity.Relational;
using VitalChoice.Data.DataContext;
using VitalChoice.Data.Repositories;
using VitalChoice.Domain;

#endregion

namespace VitalChoice.Data.UnitOfWork
{
	public class UnitOfWork : IUnitOfWork, IUnitOfWorkAsync
    {
        #region Private Fields

        private readonly IDataContextAsync dataContext;
        private bool disposed;
        private Dictionary<string, object> repositories;
        private RelationalTransaction transaction;

        #endregion Private Fields

        #region Constuctor/Dispose

        public UnitOfWork(IDataContextAsync dataContext)
        {
            this.dataContext = dataContext;
        }

        public void Dispose()
        {
	        transaction?.Dispose();

	        Dispose(true);
            GC.SuppressFinalize(this);
        }

        public virtual void Dispose(bool disposing)
        {
            if (!disposed && disposing)
                dataContext.Dispose();
            disposed = true;
        }

        #endregion Constuctor/Dispose

        public int SaveChanges()
        {
            return dataContext.SaveChanges();
        }

        public Task<int> SaveChangesAsync(CancellationToken cancellationToken)
        {
            return dataContext.SaveChangesAsync(cancellationToken);
        }

        public IUnitRepositoryAsync<TEntity> RepositoryAsync<TEntity>() where TEntity : Entity
        {
            if (repositories == null)
                repositories = new Dictionary<string, object>();

            var type = typeof (TEntity).Name;

            if (repositories.ContainsKey(type))
                return (IUnitRepositoryAsync<TEntity>) repositories[type];

            var repositoryType = typeof (UnitRepositoryAsync<>);
            repositories.Add(type, Activator.CreateInstance(repositoryType.MakeGenericType(typeof (TEntity)), dataContext));

            return (IUnitRepositoryAsync<TEntity>) repositories[type];
        }

        #region Unit of Work Transactions

        public void BeginTransaction()
        {
			transaction = ((DbContext) dataContext).Database.AsRelational().AsSqlServer().Connection.BeginTransaction(IsolationLevel.ReadCommitted);
        }

        public bool Commit()
        {
            transaction.Commit();
            return true;
        }

        public void Rollback()
        {
            transaction.Rollback();
        }

	    #endregion
    }
}