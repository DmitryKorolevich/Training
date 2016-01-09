using System;
using System.Collections.Generic;
using System.Linq;
using VitalChoice.Caching.Relational.Base;

namespace VitalChoice.Caching.Relational
{
    public class EntityKey : EntityValueGroup<EntityKeyValue, EntityKeyInfo>, IEquatable<EntityKey>
    {
        public EntityKey(IEnumerable<EntityKeyValue> keys): base(keys)
        {
        }

        public bool Equals(EntityKey other)
        {
            return base.Equals(other);
        }
    }
}