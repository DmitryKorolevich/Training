using System;
using Microsoft.EntityFrameworkCore.Storage;
using VitalChoice.Data.Context;

namespace VitalChoice.Data.Transaction
{
    public interface IInnerEmbeddingTransaction: IDbContextTransaction
    {
        IDataContextAsync DbContext { get; }
        bool Closed { get; }
        event Action TransactionCommit;
        event Action TransactionRollback;
        void IncReference();
    }
}