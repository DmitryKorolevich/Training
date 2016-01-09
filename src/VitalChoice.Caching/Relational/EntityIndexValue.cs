using System;
using VitalChoice.Caching.Relational.Base;

namespace VitalChoice.Caching.Relational
{
    public class EntityIndexValue : EntityValue<EntityIndexInfo>, IEquatable<EntityIndexValue>
    {
        public bool Equals(EntityIndexValue other)
        {
            return base.Equals(other);
        }

        public EntityIndexValue(EntityIndexInfo indexInfo, object value) : base(indexInfo, value)
        {
        }
    }
}