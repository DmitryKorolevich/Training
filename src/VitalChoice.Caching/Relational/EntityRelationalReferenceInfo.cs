using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VitalChoice.Caching.Relational.Base;

namespace VitalChoice.Caching.Relational
{
    public class EntityRelationalReferenceInfo : EntityValueGroupInfo<EntityValueInfo>,
        IEquatable<EntityRelationalReferenceInfo>
    {
        public bool Equals(EntityRelationalReferenceInfo other)
        {
            return base.Equals(other);
        }

        public EntityRelationalReferenceInfo(IEnumerable<EntityValueInfo> keyInfos) : base(keyInfos)
        {
        }
    }
}