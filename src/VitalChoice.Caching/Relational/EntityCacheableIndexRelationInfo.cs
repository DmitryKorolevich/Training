using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VitalChoice.Caching.Relational.Base;

namespace VitalChoice.Caching.Relational
{
    public class EntityCacheableIndexRelationInfo : EntityCacheableIndexInfo
    {
        public PrincipalToIndexMap KeyMapping { get; }
        public string Name { get; }

        public EntityCacheableIndexRelationInfo(ICollection<EntityValueInfo> foreignValues, string name, IEnumerable<EntityValueInfo> principalValues) : base(foreignValues)
        {
            KeyMapping = new PrincipalToIndexMap(principalValues, foreignValues);
            Name = name;
        }
    }
}