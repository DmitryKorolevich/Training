using System;
using System.Collections.Generic;
using VitalChoice.Caching.Relational.Base;

namespace VitalChoice.Caching.Interfaces
{
    public interface ICacheSyncProvider
    {
        void SendChanges(IEnumerable<SyncOperation> syncOperations);

        void AcceptChanges(IEnumerable<SyncOperation> syncOperations);
    }
}