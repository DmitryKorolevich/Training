using System;
using System.Data;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.Data.Entity;
using Microsoft.Data.Entity.Storage;
using VitalChoice.Data.Context;
using VitalChoice.Data.Transaction;
using VitalChoice.Infrastructure.Domain.Entities.Roles;
using VitalChoice.Infrastructure.Domain.Entities.Users;

namespace VitalChoice.Infrastructure.Context
{
    public abstract class IdentityDataContext : IdentityDbContext<ApplicationUser, ApplicationRole, int>, IDataContext, IDataContextAsync
    {
        private InnerEmbeddingTransaction _transaction;

        protected IdentityDataContext()
        {
            InstanceId = Guid.NewGuid();
        }

        protected IdentityDataContext(IServiceProvider serviceProvider) : base(serviceProvider)
        {
            InstanceId = Guid.NewGuid();
        }

        public Guid InstanceId { get; }

        public IRelationalTransaction BeginTransaction(IsolationLevel isolation = IsolationLevel.ReadUncommitted)
        {
            if (_transaction == null || _transaction.Closed)
            {
                _transaction = new InnerEmbeddingTransaction(Database.BeginTransaction(isolation));
                _transaction.TransactionCommit += OnTransactionCommit;
            }
            _transaction.IncReference();
            return _transaction;
        }

        public bool InTransaction => _transaction != null && !_transaction.Closed;

        public event Action TransactionCommit;

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