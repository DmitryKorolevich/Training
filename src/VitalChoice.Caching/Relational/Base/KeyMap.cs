using System;
using System.Collections.Generic;
using System.Linq;
using VitalChoice.Caching.Extensions;
using VitalChoice.Ecommerce.Domain.Helpers;

namespace VitalChoice.Caching.Relational.Base
{
    public abstract class KeyMap<T>
        where T : EntityValueInfo
    {
        protected KeyMap(IEnumerable<T> left, IEnumerable<T> right)
        {
            _keyMapping = left.SimpleJoin(right).ToArray();
            _keyMapping.NormalizeUpdate();
        }

        protected int PropertyCount => _keyMapping.Length;

        private readonly KeyValuePair<T, T>[] _keyMapping;

        protected EntityValue<T>[] MapValues(EntityValue<T>[] values)
        {
            var result = new EntityValue<T>[values.Length];
            for (var i = 0; i < values.Length; i++)
            {
                var info = _keyMapping[i].Value;
                if (info.ItemIndex == -1)
                    throw new ArgumentException();
                result[info.ItemIndex] = new EntityValue<T>(info, values[i].Value);
            }
            return result;
        }
    }
}