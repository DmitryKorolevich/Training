using System;
using System.Linq;

namespace VitalChoice.Caching.Relational.Base
{
    public abstract class EntityValueGroup<TValue, TInfo> : IEquatable<EntityValueGroup<TValue, TInfo>>
        where TValue: EntityValue<TInfo>
        where TInfo: EntityValueInfo
    {
        internal readonly TValue[] Values;

        protected EntityValueGroup(TValue[] orderedValues)
        {
            Values = orderedValues;
        }

        public bool Equals(EntityValueGroup<TValue, TInfo> other)
        {
            if ((object)this == (object)other) return true;
            if ((object)other == null) return false;

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

        public static bool Equals(EntityValueGroup<TValue, TInfo> left, EntityValueGroup<TValue, TInfo> right)
        {
            return left?.Equals(right) ?? (object) right == null;
        }

        public override bool Equals(object obj)
        {
            if ((object)this == obj) return true;
            var primaryKey = obj as EntityValueGroup<TValue, TInfo>;
            return primaryKey != null && Equals(primaryKey);
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

        public virtual bool IsValid
        {
            get
            {
                if (Values == null || !Values.Any())
                    return false;
                foreach (var value in Values)
                {
                    if (value.Value == null)
                        return false;
                    if ((value.Value as long?) <= 0)
                    {
                        return false;
                    }
                    if ((value.Value as int?) <= 0)
                    {
                        return false;
                    }
                }
                return true;
            }
        }
    }
}