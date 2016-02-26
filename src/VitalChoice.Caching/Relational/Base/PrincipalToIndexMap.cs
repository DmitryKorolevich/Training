using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace VitalChoice.Caching.Relational.Base
{
    public class PrincipalToIndexMap : KeyMap<EntityValueInfo>
    {
        public EntityIndex MapPrincipalToForeign(EntityKey pk)
        {
            var result = new EntityIndex(MapValues(pk.Values));
            return result;
        }

        public PrincipalToIndexMap(IEnumerable<EntityValueInfo> principal, IEnumerable<EntityValueInfo> foreign) 
            : base(principal, foreign)
        {
        }
    }
}