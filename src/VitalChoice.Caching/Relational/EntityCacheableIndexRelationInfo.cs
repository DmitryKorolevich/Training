using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VitalChoice.Caching.Relational.Base;

namespace VitalChoice.Caching.Relational
{
    public class EntityCacheableIndexRelationInfo : EntityCacheableIndexInfo
    {
        private readonly KeyMap _keyMapping;
        public string Name { get; }

        public EntityCacheableIndexRelationInfo(IEnumerable<EntityValueInfo> valueInfos, string name, KeyMap keyMapping) : base(valueInfos)
        {
            _keyMapping = keyMapping;
            Name = name;
        }

        public EntityIndex MapPrincipalToForeignValues(EntityKey pk)
        {
            return _keyMapping.MapPrincipalToForeignValues(pk);
        }
    }
}
