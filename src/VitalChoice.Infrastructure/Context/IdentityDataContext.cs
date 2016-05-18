using System;
using System.Data;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using VitalChoice.Data.Context;
using VitalChoice.Data.Transaction;
using VitalChoice.Infrastructure.Domain.Entities.Roles;
using VitalChoice.Infrastructure.Domain.Entities.Users;

namespace VitalChoice.Infrastructure.Context
{
	public class IdentityDataContext : IdentityDbContext<ApplicationUser, ApplicationRole, int>, IDataContextAsync
    {
        private IInnerEmbeddingTransaction _transaction;

        protected IdentityDataContext()
        {
            InstanceId = Guid.NewGuid();
        }

	    public object Tag { get; set; }
	    public bool Disposed { get; private set; }
	    public Guid InstanceId { get; }
	    
	    public IInnerEmbeddingTransaction BeginTransaction(IsolationLevel isolation = IsolationLevel.ReadUncommitted)        {
            if (_transaction == null || _transaction.Closed)
            {
                _transaction = new InnerEmbeddingTransaction(Database.BeginTransaction(isolation), this);
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

	    protected virtual void OnTransactionRollback()
	    {
	        TransactionRollback?.Invoke();
	    }
    }
}