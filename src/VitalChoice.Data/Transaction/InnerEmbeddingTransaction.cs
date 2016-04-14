using System;
using System.Data.Common;
using Microsoft.Data.Entity.Storage;
using Microsoft.Extensions.Logging;
using VitalChoice.Data.Context;

namespace VitalChoice.Data.Transaction
{
    public class InnerEmbeddingTransaction : IInnerEmbeddingTransaction
    {
        private readonly IRelationalTransaction _transaction;
        public IDataContextAsync DbContext { get; }
        private int _referenceCount;

        public InnerEmbeddingTransaction(IRelationalTransaction transaction, IDataContextAsync dbContext)
        {
            _transaction = transaction;
            DbContext = dbContext;
        }

        public void Dispose()
        {
            if (_referenceCount <= 1)
            {
                Closed = true;
                _referenceCount--;
                _transaction.Dispose();
            }
            else
            {
                _referenceCount--;
            }
        }

        public DbTransaction Instance => _transaction.Instance;

        public void Commit()
        {
            if (_referenceCount == 1)
            {
                _transaction.Commit();
                OnTransactionCommit();
            }
            if (_referenceCount <= 0)
            {
                throw new Exception("Some of the inner transactions was rolled back");
            }
        }

        public void Rollback()
        {
            Closed = true;
            if (_referenceCount > 0)
            {
                _referenceCount = 0;
                _transaction.Rollback();
                OnTransactionRollback();
            }
        }

        public event Action TransactionRollback;

        public void IncReference()
        {
            _referenceCount++;
        }

        public bool Closed { get; private set; }

        public IRelationalConnection Connection => _transaction.Connection;

        public event Action TransactionCommit;

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