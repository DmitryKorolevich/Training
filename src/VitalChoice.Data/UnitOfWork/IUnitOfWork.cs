using System;
using Microsoft.Data.Entity.Relational;
using Microsoft.Data.Entity.Storage;

namespace VitalChoice.Data.UnitOfWork
{
    public interface IUnitOfWork : IDisposable
    {
        int SaveChanges();
        void Dispose(bool disposing);
	    IRelationalTransaction BeginTransaction();
    }
}