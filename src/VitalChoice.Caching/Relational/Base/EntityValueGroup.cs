using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using VitalChoice.Ecommerce.Domain.Helpers;

namespace VitalChoice.Caching.Relational.Base
{
    public abstract class EntityValueGroup<TValue, TInfo> : IEquatable<EntityValueGroup<TValue, TInfo>>
        where TValue: EntityValue<TInfo>
        where TInfo: EntityValueInfo
    {
        internal readonly TValue[] Values;

        protected EntityValueGroup(IEnumerable<TValue> values)
        {
            Values = values.OrderBy(v => v.ValueInfo.Name, StringComparer.Ordinal).ToArray();
        }

        public bool Equals(EntityValueGroup<TValue, TInfo> other)
        {
            if (ReferenceEquals(this, other)) return true;
            if (ReferenceEquals(null, other)) return false;

            // ReSharper disable once LoopCanBeConvertedToQuery
            for (var index = 0; index < Values.Length; index++)
            {
                if (!Values[index].Value?.Equals(other.Values[index].Value) ?? false)
                {
                    return false;
                }
            }
            return true;
        }

        public override string ToString()
        {
            return $"({string.Join(", ", Values.Select(v => v.ToString()))})";
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(this, obj)) return true;
            if (ReferenceEquals(null, obj)) return false;
            var primaryKey = obj as EntityValueGroup<TValue, TInfo>;
            if (primaryKey != null)
                return Equals(primaryKey);
            return false;
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return Values.Aggregate(0, (current, entityKey) => (current*397) ^ entityKey.GetHashCode());
            }
        }

        public static bool operator ==(EntityValueGroup<TValue, TInfo> left, EntityValueGroup<TValue, TInfo> right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(EntityValueGroup<TValue, TInfo> left, EntityValueGroup<TValue, TInfo> right)
        {
            return !Equals(left, right);
        }
    }
}