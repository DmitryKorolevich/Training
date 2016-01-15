using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Data.Entity.ChangeTracking.Internal;
using VitalChoice.Caching.Relational;
using VitalChoice.Caching.Relational.Base;

namespace VitalChoice.Caching.Interfaces
{
    public interface ICacheSyncProvider
    {
        void SendChanges(IEnumerable<SyncOperation> syncOperations);
        void AcceptChanges(IEnumerable<SyncOperation> syncOperations);
    }
}