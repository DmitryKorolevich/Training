using System;
using System.Collections;
using System.Collections.Generic;

namespace VitalChoice.Caching.Relational.Base
{
    public class EntityValue<TInfo> : IEquatable<EntityValue<TInfo>>
        where TInfo: EntityValueInfo
    {
        private readonly IEqualityComparer _comparer;

        public EntityValue(TInfo valueInfo, object value)
        {
            if (valueInfo == null)
                throw new ArgumentNullException(nameof(valueInfo));

            ValueInfo = valueInfo;
            Value = value;
            if (valueInfo.PropertyType == typeof (string))
            {
                _comparer = new StringComparer(StringComparison.OrdinalIgnoreCase);
            }
            else
            {
                _comparer = new ValueComparer();
            }
        }

        public bool Equals(EntityValue<TInfo> other)
        {
            return _comparer.Equals(Value, other?.Value);
        }

        public override bool Equals(object obj)
        {
            if ((object)this == obj) return true;
            var entityKey = obj as EntityValue<TInfo>;
            return entityKey != null && Equals(entityKey);
        }

        public override string ToString()
        {
            return $"{ValueInfo.Name}: {Value}";
        }

        public override int GetHashCode()
        {
            return Value?.GetHashCode() ?? 0;
        }

        public static bool Equals(EntityValue<TInfo> left, EntityValue<TInfo> right)
        {
            return left?.Equals(right) ?? (object) right == null;
        }

        public static bool operator ==(EntityValue<TInfo> left, EntityValue<TInfo> right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(EntityValue<TInfo> left, EntityValue<TInfo> right)
        {
            return !Equals(left, right);
        }

        public TInfo ValueInfo { get; }

        public object Value { get; }

        private struct ValueComparer : IEqualityComparer
        {
            bool IEqualityComparer.Equals(object x, object y)
            {
                if (y == null) return false;
                return x.Equals(y);
            }

            public int GetHashCode(object obj)
            {
                return obj?.GetHashCode() ?? 0;
            }
        }

        private struct StringComparer : IEqualityComparer
        {
            private readonly StringComparison _comparision;

            public StringComparer(StringComparison comparision)
            {
                _comparision = comparision;
            }

            bool IEqualityComparer.Equals(object x, object y)
            {
                return string.Equals((string) x, (string) y, _comparision);
            }

            public int GetHashCode(object obj)
            {
                return obj?.GetHashCode() ?? 0;
            }
        }
    }
}