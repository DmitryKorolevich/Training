using System;

namespace VitalChoice.Caching.Relational
{
    public class EntityIndexValue : IEquatable<EntityIndexValue>
    {
        public bool Equals(EntityIndexValue other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return IndexInfo.Equals(other.IndexInfo) && Value.Equals(other.Value);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            var entityKey = obj as EntityIndexValue;
            if (entityKey != null)
                return Equals(entityKey);
            return false;
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (IndexInfo.GetHashCode() * 397) ^ Value.GetHashCode();
            }
        }

        public static bool operator ==(EntityIndexValue left, EntityIndexValue right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(EntityIndexValue left, EntityIndexValue right)
        {
            return !Equals(left, right);
        }

        public EntityIndexValue(EntityIndexInfo indexInfo, object value)
        {
            if (indexInfo == null)
                throw new ArgumentNullException(nameof(indexInfo));
            if (value == null)
                throw new ArgumentNullException(nameof(value));

            IndexInfo = indexInfo;
            Value = value;
        }

        public EntityIndexInfo IndexInfo { get; }

        public object Value { get; }
    }
}
