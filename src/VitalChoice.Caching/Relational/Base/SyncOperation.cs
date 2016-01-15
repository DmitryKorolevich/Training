using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading.Tasks;

namespace VitalChoice.Caching.Relational.Base
{
    [DataContract]
    public class SyncOperation
    {
        [DataMember]
        public SyncType SyncType { get; set; }
        [DataMember]
        public Type EntityType { get; set; }
        [DataMember]
        public EntityKeyExportable Key { get; set; }
    }

    public enum SyncType
    {
        Invalid = 0,
        Add = 1,
        Update = 2,
        Delete = 3
    }
}
