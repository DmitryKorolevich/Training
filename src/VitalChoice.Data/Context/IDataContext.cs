using System;
using System.Data;
using Microsoft.EntityFrameworkCore;
using VitalChoice.Data.Transaction;

namespace VitalChoice.Data.Context
{
    public interface IDataContext : IDisposable
    {
        object Tag { get; set; }

        bool Disposed { get; }

        Guid InstanceId { get; }

        IScopedTransaction BeginTransaction(IsolationLevel isolation = IsolationLevel.ReadUncommitted);

        bool InTransaction { get; }

        event Action TransactionCommit;
        event Action TransactionRollback;

        int SaveChanges();

        DbSet<T> Set<T>() 
            where T : class;

        void TrackGraphForAdd(object entity);

        void SetState(object entity, EntityState state);
    }
}