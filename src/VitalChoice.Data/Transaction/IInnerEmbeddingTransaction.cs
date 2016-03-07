using System;
using Microsoft.Data.Entity.Storage;
using VitalChoice.Data.Context;

namespace VitalChoice.Data.Transaction
{
    public interface IInnerEmbeddingTransaction: IRelationalTransaction
    {
        IDataContextAsync DbContext { get; }
        bool Closed { get; }
        event Action TransactionCommit;
        event Action TransactionRollback;
        void IncReference();
    }
}