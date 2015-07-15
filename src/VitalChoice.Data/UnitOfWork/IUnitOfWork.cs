using System;
using System.Data;
using Microsoft.Data.Entity.Relational;

namespace VitalChoice.Data.UnitOfWork
{
    public interface IUnitOfWork : IDisposable
    {
        int SaveChanges();
        void Dispose(bool disposing);
	    RelationalTransaction BeginTransaction(IsolationLevel isolation = IsolationLevel.ReadCommitted);
    }
}