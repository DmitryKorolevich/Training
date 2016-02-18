using System;
using System.Collections.Generic;
using System.Data;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Data.Entity;
using Microsoft.Data.Entity.Infrastructure;
using Microsoft.Data.Entity.Storage;
using VitalChoice.Data.Transaction;

namespace VitalChoice.Data.Context
{
    public abstract class DataContext : DbContext, IDataContextAsync
    {
        private IInnerEmbeddingTransaction _transaction;

        protected DataContext()
        {
            InstanceId = Guid.NewGuid();
        }

        protected DataContext(IServiceProvider serviceProvider) : base(serviceProvider)
        {
            InstanceId = Guid.NewGuid();
        }

	    
	    public Guid InstanceId { get; }

	    public IInnerEmbeddingTransaction BeginTransaction(IsolationLevel isolation = IsolationLevel.ReadUncommitted)
	    {
	        if (_transaction == null || _transaction.Closed)
	        {
                _transaction = new InnerEmbeddingTransaction(Database.BeginTransaction(isolation), this);
                _transaction.TransactionCommit += OnTransactionCommit;
            }
            _transaction.IncReference();
            return _transaction;
        }

        public bool InTransaction => _transaction != null && !_transaction.Closed;

        public override void Dispose()
        {
            if (_transaction == null)
                base.Dispose();
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

        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            var changesAsync = await base.SaveChangesAsync(cancellationToken);
            return changesAsync;
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
    }
}