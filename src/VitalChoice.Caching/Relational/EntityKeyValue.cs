using System;
using VitalChoice.Caching.Relational.Base;

namespace VitalChoice.Caching.Relational
{
    public class EntityKeyValue : EntityValue<EntityKeyInfo>, IEquatable<EntityKeyValue>
    {
        public bool Equals(EntityKeyValue other)
        {
            return base.Equals(other);
        }

        public EntityKeyValue(EntityKeyInfo keyInfo, object key) : base(keyInfo, key)
        {
        }
    }
}