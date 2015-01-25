

using Microsoft.Data.Entity;
using System;

namespace VitalChoice.Data.DataContext
{
    public interface IDataContext : IDisposable
    {
        int SaveChanges();

		//void SyncObjectState(object entity);

		void SetState(object entity, EntityState state);
    }
}