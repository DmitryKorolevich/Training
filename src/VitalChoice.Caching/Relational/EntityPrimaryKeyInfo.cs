using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VitalChoice.Caching.Relational.Base;

namespace VitalChoice.Caching.Relational
{
    public class EntityPrimaryKeyInfo : EntityValueGroupInfo<EntityValueInfo>, IEquatable<EntityPrimaryKeyInfo>
    {
        public bool Equals(EntityPrimaryKeyInfo other)
        {
            return base.Equals(other);
        }

        public EntityPrimaryKeyInfo(IEnumerable<EntityValueInfo> keyInfos): base(keyInfos)
        {
        }
    }
}