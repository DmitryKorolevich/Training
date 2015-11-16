

using Microsoft.Data.Entity;
using System;

namespace VitalChoice.Data.DataContext
{
    public interface IDataContext : IDisposable
    {
        int SaveChanges();

        DbSet<T> Set<T>() 
            where T : class;

        void TrackGraphForAdd(object entity);

        void SetState(object entity, EntityState state);
    }
}