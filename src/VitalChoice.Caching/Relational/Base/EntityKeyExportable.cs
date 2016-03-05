using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading.Tasks;

namespace VitalChoice.Caching.Relational.Base
{
    [DataContract]
    public class EntityKeyExportable
    {
        [DataMember]
        public string EntityType { get; set; }

        [DataMember]
        public ICollection<EntityValueExportable> Values { get; set; }

        public override string ToString()
        {
            return string.Join(", ", Values.Select(v => v.ToString()));
        }
    }
}