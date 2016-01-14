using System;
using System.Data.Common;
using Microsoft.Data.Entity.Storage;

namespace VitalChoice.Data.Transaction
{
    public class InnerEmbeddingTransaction : IRelationalTransaction
    {
        private readonly IRelationalTransaction _transaction;
        private int _referenceCount;

        public InnerEmbeddingTransaction(IRelationalTransaction transaction)
        {
            _transaction = transaction;
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
            }
        }

        public void IncReference()
        {
            _referenceCount++;
        }

        public bool Closed { get; private set; }

        public IRelationalConnection Connection => _transaction.Connection;
    }
}