using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VitalChoice.Caching.Relational.Base;

namespace VitalChoice.Caching.Relational
{
    public class EntityPrimaryKeyInfo : EntityValueGroupInfo<EntityKeyInfo>, IEquatable<EntityPrimaryKeyInfo>
    {
        public bool Equals(EntityPrimaryKeyInfo other)
        {
            return base.Equals(other);
        }

        public EntityPrimaryKeyInfo(IEnumerable<EntityKeyInfo> keyInfos): base(keyInfos)
        {
        }
    }
}