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
        public string AppName { get; set; }

        [DataMember]
        public double AveragePing { get; set; }

        [DataMember]
        public DateTime? SendTime { get; set; }

        [DataMember]
        public SyncType SyncType { get; set; }

        [DataMember]
        public EntityKeyExportable Key { get; set; }

        [DataMember]
        public ICollection<EntityForeignKeysExportable> ForeignKeys { get; set; }

        public override string ToString()
        {
            return $"[{SyncType}:{Key.EntityType}]{Key}";
        }
    }

    public enum SyncType
    {
        Invalid = 0,
        Add = 1,
        Update = 2,
        Delete = 3,
        Ping = 4
    }
}