﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Threading;
using System.Threading.Tasks;
using VitalChoice.Data.Context;
using VitalChoice.Data.Repositories;
using VitalChoice.Data.Transaction;
using VitalChoice.Ecommerce.Domain;

namespace VitalChoice.Data.UOW
{
    public class UnitOfWork : IUnitOfWorkAsync
    {
        #region Private Fields

        private readonly IDataContextAsync _dataContext;
        private readonly bool _contextOwned;
        private bool _disposed;
        private Dictionary<Type, object> _repositories;
        private Dictionary<Type, object> _readRepositories;

        #endregion Private Fields

        #region Constuctor/Dispose

        public UnitOfWork(IDataContextAsync dataContext, bool contextOwned)
        {
            _dataContext = dataContext;
            _contextOwned = contextOwned;
        }

        public IScopedTransaction BeginTransaction(IsolationLevel isolation = IsolationLevel.ReadCommitted)
        {
            return _dataContext.BeginTransaction(isolation);
        }

        #endregion Constuctor/Dispose

        public int SaveChanges()
        {
            return _dataContext.SaveChanges();
        }

        public Task<int> SaveChangesAsync()
        {
            return _dataContext.SaveChangesAsync(CancellationToken.None);
        }

        public Task<int> SaveChangesAsync(CancellationToken cancellationToken)
        {
            return _dataContext.SaveChangesAsync(cancellationToken);
        }

        public IRepositoryAsync<TEntity> RepositoryAsync<TEntity>() where TEntity : Entity
        {
            if (_repositories == null)
                _repositories = new Dictionary<Type, object>();
            object result;
            if (_repositories.TryGetValue(typeof(TEntity), out result))
                return (IRepositoryAsync<TEntity>) result;

            var repositoryType = typeof(UnitRepositoryAsync<>);
            result = Activator.CreateInstance(repositoryType.MakeGenericType(typeof(TEntity)), _dataContext);
            _repositories.Add(typeof(TEntity), result);

            return (IRepositoryAsync<TEntity>) result;
        }

        public IReadRepositoryAsync<TEntity> ReadRepositoryAsync<TEntity>() where TEntity : Entity
        {
            if (_readRepositories == null)
                _readRepositories = new Dictionary<Type, object>();
            object result;
            if (_readRepositories.TryGetValue(typeof(TEntity), out result))
                return (IReadRepositoryAsync<TEntity>) result;

            var repositoryType = typeof(ReadRepositoryAsync<>);
            result = Activator.CreateInstance(repositoryType.MakeGenericType(typeof(TEntity)), _dataContext);
            _readRepositories.Add(typeof(TEntity), result);

            return (IReadRepositoryAsync<TEntity>) result;
        }

        public void Dispose()
        {
            if (!_disposed)
            {
                _disposed = true;
                if (_contextOwned)
                {
                    _dataContext.Dispose();
                }
            }
        }
    }
}