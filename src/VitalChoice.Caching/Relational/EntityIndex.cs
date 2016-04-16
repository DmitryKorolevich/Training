using System;
using System.Collections.Generic;
using System.Linq;
using VitalChoice.Caching.Relational.Base;

namespace VitalChoice.Caching.Relational
{
    public class EntityIndex : EntityValueGroup<EntityValue<EntityValueInfo>, EntityValueInfo>, IEquatable<EntityIndex>
    {
        public bool Equals(EntityIndex other)
        {
            return base.Equals(other);
        }

        public EntityIndex(EntityValue<EntityValueInfo>[] orderedValues) : base(orderedValues)
        {
        }
    }
}
