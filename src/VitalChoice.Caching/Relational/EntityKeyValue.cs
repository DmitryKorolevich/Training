using System;

namespace VitalChoice.Caching.Relational
{
    public class EntityKeyValue : IEquatable<EntityKeyValue>
    {
        public bool Equals(EntityKeyValue other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return KeyInfo.Equals(other.KeyInfo) && Key.Equals(other.Key);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            var entityKey = obj as EntityKeyValue;
            if (entityKey != null)
                return Equals(entityKey);
            return false;
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (KeyInfo.GetHashCode()*397) ^ Key.GetHashCode();
            }
        }

        public static bool operator ==(EntityKeyValue left, EntityKeyValue right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(EntityKeyValue left, EntityKeyValue right)
        {
            return !Equals(left, right);
        }

        public EntityKeyValue(EntityKeyInfo keyInfo, object key)
        {
            if (keyInfo == null)
                throw new ArgumentNullException(nameof(keyInfo));
            if (key == null)
                throw new ArgumentNullException(nameof(key));

            KeyInfo = keyInfo;
            Key = key;
        }

        public EntityKeyInfo KeyInfo { get; }

        public object Key { get; }
    }
}