using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VitalChoice.Caching.Relational.Base;

namespace VitalChoice.Caching.Relational
{
    public class RelationReference
    {
        public RelationReference(EntityValueGroupInfo<EntityValueInfo> valueInfo)
        {
            ValueInfo = valueInfo;
        }

        public EntityValueGroupInfo<EntityValueInfo> ValueInfo { get; }
    }
}