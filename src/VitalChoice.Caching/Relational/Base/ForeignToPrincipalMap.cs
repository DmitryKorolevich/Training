using System.Collections.Generic;

namespace VitalChoice.Caching.Relational.Base
{
    public class ForeignToPrincipalMap : KeyMap<EntityValueInfo>
    {
        public EntityKey MapForeignToPrincipal(EntityForeignKey fk)
        {
            return new EntityKey(MapValues(fk.Values));
        }

        public ForeignToPrincipalMap(IEnumerable<EntityValueInfo> foreign, IEnumerable<EntityValueInfo> principal)
            : base(foreign, principal)
        {
        }
    }
}