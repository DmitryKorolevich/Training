using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VitalChoice.Caching.Relational.Base;

namespace VitalChoice.Caching.Relational
{
    public class EntityForeignKey : EntityValueGroup<EntityValue<EntityValueInfo>, EntityValueInfo>, IEquatable<EntityForeignKey>
    {
        public EntityForeignKey(IEnumerable<EntityValue<EntityValueInfo>> keys) : base(keys)
        {
        }

        public bool Equals(EntityForeignKey other)
        {
            return base.Equals(other);
        }
    }
}