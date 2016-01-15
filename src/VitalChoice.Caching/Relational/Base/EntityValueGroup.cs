using System;
using System.Collections.Generic;
using System.Linq;

namespace VitalChoice.Caching.Relational.Base
{
    public abstract class EntityValueGroup<TValue, TInfo> : IEquatable<EntityValueGroup<TValue, TInfo>>
        where TValue: EntityValue<TInfo>
        where TInfo: EntityValueInfo
    {
        internal readonly Dictionary<string, TValue> Values;

        protected EntityValueGroup(IEnumerable<TValue> values)
        {
            Values = values.ToDictionary(v => v.ValueInfo.Name);
        }

        public bool Equals(EntityValueGroup<TValue, TInfo> other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;

            // ReSharper disable once LoopCanBeConvertedToQuery
            foreach (var entityKey in Values)
            {
                var thisKey = entityKey.Value;
                var otherKey = other.Values[entityKey.Key];
                if (!thisKey.Equals(otherKey))
                {
                    return false;
                }
            }
            return true;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            var primaryKey = obj as EntityValueGroup<TValue, TInfo>;
            if (primaryKey != null)
                return Equals(primaryKey);
            return false;
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return Values.Aggregate(0, (current, entityKey) => (current*397) ^ entityKey.Value.GetHashCode());
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