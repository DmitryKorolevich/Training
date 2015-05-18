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

        private readonly IDataContextAsync _dataContext;
        private bool _disposed;
        private Dictionary<string, object> _repositories;

        #endregion Private Fields

        #region Constuctor/Dispose

        public UnitOfWork(IDataContextAsync dataContext)
        {
            this._dataContext = dataContext;
        }

        public void Dispose()
        {
	        Dispose(true);
            GC.SuppressFinalize(this);
        }

        public virtual void Dispose(bool disposing)
        {
            if (!_disposed && disposing)
                _dataContext.Dispose();
            _disposed = true;
        }

        #endregion Constuctor/Dispose

        public int SaveChanges()
        {
            return _dataContext.SaveChanges();
        }

        public Task<int> SaveChangesAsync(CancellationToken cancellationToken)
        {
            return _dataContext.SaveChangesAsync(cancellationToken);
        }

        public IUnitRepositoryAsync<TEntity> RepositoryAsync<TEntity>() where TEntity : Entity
        {
            if (_repositories == null)
                _repositories = new Dictionary<string, object>();

            var type = typeof (TEntity).Name;

            if (_repositories.ContainsKey(type))
                return (IUnitRepositoryAsync<TEntity>) _repositories[type];

            var repositoryType = typeof (UnitRepositoryAsync<>);
            _repositories.Add(type, Activator.CreateInstance(repositoryType.MakeGenericType(typeof (TEntity)), _dataContext));

            return (IUnitRepositoryAsync<TEntity>) _repositories[type];
        }

        #region Unit of Work Transactions

	    #endregion
    }
}