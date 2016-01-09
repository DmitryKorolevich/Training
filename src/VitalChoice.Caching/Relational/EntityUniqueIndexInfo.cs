using System;
using System.Collections.Generic;
using System.Linq;
using VitalChoice.Caching.Relational.Base;

namespace VitalChoice.Caching.Relational
{
    public class EntityUniqueIndexInfo : EntityValueGroupInfo<EntityIndexInfo>, IEquatable<EntityUniqueIndexInfo>
    {
        public bool Equals(EntityUniqueIndexInfo other)
        {
            return base.Equals(other);
        }

        public EntityUniqueIndexInfo(IEnumerable<EntityIndexInfo> valueInfos) : base(valueInfos)
        {
        }
    }
}
