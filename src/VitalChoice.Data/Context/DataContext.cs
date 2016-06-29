﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using VitalChoice.Data.Transaction;

namespace VitalChoice.Data.Context
{
    public abstract class DataContext : DbContext, IDataContextAsync
    {
        private IScopedTransaction _transaction;

        protected DataContext(DbContextOptions contextOptions) :base(contextOptions)
        {
            InstanceId = Guid.NewGuid();
        }

        public object Tag { get; set; }
        public bool Disposed { get; private set; }
        public Guid InstanceId { get; }

	    public IScopedTransaction BeginTransaction(IsolationLevel isolation = IsolationLevel.ReadUncommitted)
	    {
	        if (_transaction == null || _transaction.Closed)
	        {
                _transaction = new ScopedTransaction(Database.BeginTransaction(isolation), this);
                _transaction.TransactionCommit += OnTransactionCommit;
	            _transaction.TransactionRollback += OnTransactionRollback;
	        }
            _transaction.IncReference();
            return _transaction;
        }

        public bool InTransaction => _transaction != null && !_transaction.Closed;

        public event Action TransactionCommit;
        public event Action TransactionRollback;

        public override void Dispose()
        {
            if (_transaction == null || _transaction.Closed)
                base.Dispose();
            Disposed = true;
        }

        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default(CancellationToken))
		{
			var changesAsync = await base.SaveChangesAsync(cancellationToken);
			return changesAsync;
		}

        public override int SaveChanges()
        {
            var changes = base.SaveChanges();
            return changes;
        }

        public void SetState(object entity, EntityState state)
        {
            base.Entry(entity).State = state;
        }

        public void TrackGraphForAdd(object entity)
        {
            this.ChangeTracker.TrackGraph(entity, e => e.Entry.State = EntityState.Added);
        }

        protected virtual void OnTransactionCommit()
        {
            TransactionCommit?.Invoke();
        }

        protected virtual void OnTransactionRollback()
        {
            TransactionRollback?.Invoke();
        }
    }
}