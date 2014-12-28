using System;
using VitalChoice.Data.Infrastructure;
using VitalChoice.Data.Repositories;

namespace VitalChoice.Data.UnitOfWork
{
    public interface IUnitOfWork : IDisposable
    {
        int SaveChanges();
        void Dispose(bool disposing);
        IRepository<TEntity> Repository<TEntity>() where TEntity : IObjectState;
        void BeginTransaction();
        bool Commit();
        void Rollback();
    }
}