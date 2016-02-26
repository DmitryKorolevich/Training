using System.Collections.Generic;
using System.Linq;
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

        protected int PropertyCount => _keyMapping.Length;

        private readonly KeyValuePair<T, T>[] _keyMapping;

        protected IEnumerable<EntityValue<T>> MapValues(IEnumerable<EntityValue<T>> values)
        {
            return
                values.Select((t, index) => new EntityValue<T>(_keyMapping[index].Value, t.Value));
        }
    }
}