

using Microsoft.Data.Entity;
using System;
using System.Data;
using Microsoft.Data.Entity.Storage;

namespace VitalChoice.Data.Context
{
    public interface IDataContext : IDisposable
    {
        IRelationalTransaction BeginTransaction(IsolationLevel isolation = IsolationLevel.ReadUncommitted);

        int SaveChanges();

        DbSet<T> Set<T>() 
            where T : class;

        void TrackGraphForAdd(object entity);

        void SetState(object entity, EntityState state);
    }
}