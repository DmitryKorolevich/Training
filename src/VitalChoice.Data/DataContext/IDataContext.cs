

using Microsoft.Data.Entity;
using System;
using System.Collections.Generic;

namespace VitalChoice.Data.DataContext
{
    public interface IDataContext : IDisposable
    {
        int SaveChanges();

        //void SyncObjectState(object entity);

        void TrackGraphForAdd(object entity);

        void SetState(object entity, EntityState state);
    }
}