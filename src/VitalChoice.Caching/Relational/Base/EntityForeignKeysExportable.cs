using System.Collections.Generic;
using System.Runtime.Serialization;

namespace VitalChoice.Caching.Relational.Base
{
    [DataContract]
    public class EntityForeignKeysExportable
    {
        [DataMember]
        public string DependentType { get; set; }

        [DataMember]
        public ICollection<EntityValueExportable> Values { get; set; }
    }
}