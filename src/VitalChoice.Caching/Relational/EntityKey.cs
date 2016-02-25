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

        public bool IsValid
        {
            get
            {
                if (Values == null || !Values.Any())
                    return false;
                foreach (var value in Values)
                {
                    if (value.Value == null)
                        return false;
                    if ((value.Value as long?) <= 0)
                    {
                        return false;
                    }
                    if ((value.Value as int?) <= 0)
                    {
                        return false;
                    }
                }
                return true;
            }
        }
    }
}