using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VitalChoice.Ecommerce.Domain.Helpers;

namespace VitalChoice.Caching.Relational.Base
{
    public class KeyMap
    {
        private readonly KeyValuePair<EntityValueInfo, EntityValueInfo>[] _keyMapping;

        public KeyMap(IEnumerable<EntityValueInfo> principalMapping, IEnumerable<EntityValueInfo> foreignMapping)
        {
            _keyMapping = principalMapping.SimpleJoin(foreignMapping).OrderBy(m => m.Key.Name).ToArray();
        }

        public EntityIndex MapPrincipalToForeignValues(EntityKey pk)
        {
            return new EntityIndex(pk.Values.Select((t, index) => new EntityValue<EntityValueInfo>(_keyMapping[index].Value, t.Value)));
        }
    }
}
