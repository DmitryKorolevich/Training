using System;

namespace VitalChoice.Caching.Relational.Base
{
    public abstract class EntityValue<TInfo> : IEquatable<EntityValue<TInfo>>
        where TInfo: EntityValueInfo
    {
        protected EntityValue(TInfo valueInfo, object value)
        {
            if (valueInfo == null)
                throw new ArgumentNullException(nameof(valueInfo));
            if (value == null)
                throw new ArgumentNullException(nameof(value));

            ValueInfo = valueInfo;
            Value = value;
        }

        public bool Equals(EntityValue<TInfo> other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Value.Equals(other.Value);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            var entityKey = obj as EntityValue<TInfo>;
            if (entityKey != null)
                return Equals(entityKey);
            return false;
        }

        public override int GetHashCode()
        {
            return Value?.GetHashCode() ?? 0;
        }

        public static bool operator ==(EntityValue<TInfo> left, EntityValue<TInfo> right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(EntityValue<TInfo> left, EntityValue<TInfo> right)
        {
            return !Equals(left, right);
        }

        public virtual TInfo ValueInfo { get; }

        public virtual object Value { get; }
    }
}