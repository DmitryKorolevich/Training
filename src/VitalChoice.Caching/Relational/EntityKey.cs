using System;
using System.Collections.Generic;
using System.Linq;
using VitalChoice.Caching.Relational.Base;

namespace VitalChoice.Caching.Relational
{
    public class EntityKey : EntityValueGroup<EntityValue<EntityValueInfo>, EntityValueInfo>, IEquatable<EntityKey>
    {
        public EntityKey(IEnumerable<EntityValue<EntityValueInfo>> keys): base(keys)
        {
        }

        public bool Equals(EntityKey other)
        {
            return base.Equals(other);
        }
    }
}