using System.Collections.Generic;
using System.Runtime.Serialization;

namespace VitalChoice.Caching.Relational.Base
{
    [DataContract]
    public class EntityForeignKeyExportable
    {
        [DataMember]
        public string DependentType { get; set; }

        [DataMember]
        public ICollection<EntityValueExportable> Values { get; set; }
    }
}