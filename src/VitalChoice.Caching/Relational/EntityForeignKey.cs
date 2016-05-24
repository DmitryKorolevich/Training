using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VitalChoice.Caching.Relational.Base;

namespace VitalChoice.Caching.Relational
{
    //public class EntityCollectionForeignKey : EntityForeignKey
    //{
    //    public IEnumerable<object> Collections { get; }

    //    public EntityCollectionForeignKey(IEnumerable<object> collections) : base(null)
    //    {
    //        Collections = collections;
    //    }
    //}

    public class EntityForeignKey : EntityValueGroup<EntityValue<EntityValueInfo>, EntityValueInfo>, IEquatable<EntityForeignKey>
    {
        public bool Equals(EntityForeignKey other)
        {
            return base.Equals(other);
        }

        public EntityForeignKey(EntityValue<EntityValueInfo>[] orderedValues) : base(orderedValues)
        {
        }
    }
}