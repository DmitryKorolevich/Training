using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VitalChoice.Ecommerce.Domain.Helpers;

namespace VitalChoice.Caching.Relational.Base
{
    public abstract class KeyMap<T>
        where T : EntityValueInfo
    {
        protected KeyMap(IEnumerable<T> left, IEnumerable<T> right)
        {
            _keyMapping = left.SimpleJoin(right).OrderBy(m => m.Key.Name).ToArray();
        }

        private readonly KeyValuePair<T, T>[] _keyMapping;

        protected IEnumerable<EntityValue<T>> MapValues(IEnumerable<EntityValue<T>> values)
        {
            return values.Select((t, index) => new EntityValue<T>(_keyMapping[index].Value, t.Value));
        }
    }

    public class PrincipalToIndexMap : KeyMap<EntityValueInfo>
    {
        public EntityIndex MapPrincipalToForeign(EntityKey pk)
        {
            return new EntityIndex(MapValues(pk.Values));
        }

        public PrincipalToIndexMap(IEnumerable<EntityValueInfo> principal, IEnumerable<EntityValueInfo> foreign) 
            : base(principal, foreign)
        {
        }
    }

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