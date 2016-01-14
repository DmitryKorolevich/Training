using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace VitalChoice.Caching.Relational.Base
{
    public class SyncOperation
    {
        public SyncType SyncType { get; set; }
        public Type EntityType { get; set; }
        public EntityKey Key { get; set; }
    }

    public enum SyncType
    {
        Invalid = 0,
        Add = 1,
        Update = 2,
        Delete = 3
    }
}
