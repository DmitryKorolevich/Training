using System;
using System.Collections.Generic;
using System.Linq;
using VitalChoice.Caching.Relational.Base;

namespace VitalChoice.Caching.Relational
{
    public class EntityIndex : EntityValueGroup<EntityValue<EntityValueInfo>, EntityValueInfo>, IEquatable<EntityIndex>
    {
        public EntityIndex(IEnumerable<EntityValue<EntityValueInfo>> values):base(values)
        {
        }

        public bool Equals(EntityIndex other)
        {
            return base.Equals(other);
        }
    }
}
