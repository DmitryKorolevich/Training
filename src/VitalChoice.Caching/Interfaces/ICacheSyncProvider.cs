using System;
using System.Collections.Generic;
using VitalChoice.Caching.Relational.Base;

namespace VitalChoice.Caching.Interfaces
{
    public interface ICacheSyncProvider
    {
        ICollection<KeyValuePair<string, int>> AverageLatency { get; }

        void SendChanges(IEnumerable<SyncOperation> syncOperations);

        void AcceptChanges(IEnumerable<SyncOperation> syncOperations);
    }
}