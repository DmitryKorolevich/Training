using System;
using System.Collections.Generic;
using System.Data;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Data.Entity;
using Microsoft.Data.Entity.ChangeTracking;
using Microsoft.Data.Entity.Infrastructure;
using VitalChoice.Data.Transaction;

namespace VitalChoice.Data.Context
{
    public class DataContext : DbContext, IDataContextAsync
    {
        private IInnerEmbeddingTransaction _transaction;

        public DataContext()
		{
			InstanceId = Guid.NewGuid();
        }

		public Guid InstanceId { get; }

	    public IInnerEmbeddingTransaction BeginTransaction(IsolationLevel isolation = IsolationLevel.ReadUncommitted)
	    {
	        if (_transaction == null || _transaction.Closed)
	        {
                _transaction = new InnerEmbeddingTransaction(Database.BeginTransaction(isolation), this);
            }
	        _transaction.IncReference();
	        return _transaction;
	    }

	    public override int SaveChanges()
		{
			var changes = base.SaveChanges();
			return changes;
		}

        public override void Dispose()
        {
            if (_transaction == null || _transaction.Closed)
                base.Dispose();
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
    }
}