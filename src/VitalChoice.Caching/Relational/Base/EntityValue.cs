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

            ValueInfo = valueInfo;
            Value = value;
        }

        public bool Equals(EntityValue<TInfo> other)
        {
            if (ReferenceEquals(this, other)) return true;
            if (ReferenceEquals(null, other)) return false;
            return Value?.Equals(other.Value) ?? false;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(this, obj)) return true;
            if (ReferenceEquals(null, obj)) return false;
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