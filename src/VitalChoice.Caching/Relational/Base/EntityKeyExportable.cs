﻿using System;
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
        public Type EntityType { get; set; }

        [DataMember]
        public ICollection<EntityValueExportable> Values { get; set; }
    }
}