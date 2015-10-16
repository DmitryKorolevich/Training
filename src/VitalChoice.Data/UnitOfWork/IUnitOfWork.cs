using System;
using System.Data;
using Microsoft.Data.Entity.Storage;

namespace VitalChoice.Data.UnitOfWork
{
    public interface IUnitOfWork : IDisposable
    {
        int SaveChanges();
        void Dispose(bool disposing);
	    IRelationalTransaction BeginTransaction(IsolationLevel isolation = IsolationLevel.ReadCommitted);
    }
}