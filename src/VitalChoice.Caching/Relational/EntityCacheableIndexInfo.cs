using System;
using System.Collections.Generic;
using System.Linq;
using VitalChoice.Caching.Relational.Base;

namespace VitalChoice.Caching.Relational
{
    public class EntityCacheableIndexInfo : EntityValueGroupInfo<EntityValueInfo>, IEquatable<EntityCacheableIndexInfo>
    {
        public bool Equals(EntityCacheableIndexInfo other)
        {
            return base.Equals(other);
        }

        public EntityCacheableIndexInfo(IEnumerable<EntityValueInfo> valueInfos) : base(valueInfos)
        {
        }
    }
}
