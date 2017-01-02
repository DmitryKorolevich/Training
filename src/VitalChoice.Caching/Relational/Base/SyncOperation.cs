using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading.Tasks;

namespace VitalChoice.Caching.Relational.Base
{
    [DataContract]
    public class SyncData
    {
        
    }

    [DataContract]
    public class PingSyncData : SyncData
    {
        [DataMember]
        public string AppName { get; set; }

        [DataMember]
        public double AveragePing { get; set; }

        [DataMember]
        public DateTime? SendTime { get; set; }
    }

    [DataContract]
    public class UpdateDeleteSyncData : SyncData
    {
        [DataMember]
        public EntityKeyExportable Key { get; set; }
    }

    [DataContract]
    public class AddSyncData : UpdateDeleteSyncData
    {
        [DataMember]
        public ICollection<EntityForeignKeyExportable> ForeignKeys { get; set; }
    }

    [DataContract]
    [KnownType(typeof(PingSyncData))]
    [KnownType(typeof(AddSyncData))]
    [KnownType(typeof(UpdateDeleteSyncData))]
    public class SyncOperation
    {
        [DataMember]
        public SyncType SyncType { get; set; }

        [DataMember]
        public SyncData Data { get; set; }
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