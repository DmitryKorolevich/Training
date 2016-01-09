using System;
using System.Collections.Generic;
using System.Linq;
using VitalChoice.Caching.Relational.Base;

namespace VitalChoice.Caching.Relational
{
    public class EntityIndex : EntityValueGroup<EntityIndexValue, EntityIndexInfo>, IEquatable<EntityIndex>
    {
        public EntityIndex(IEnumerable<EntityIndexValue> values):base(values)
        {
        }

        public bool Equals(EntityIndex other)
        {
            return base.Equals(other);
        }
    }
}
