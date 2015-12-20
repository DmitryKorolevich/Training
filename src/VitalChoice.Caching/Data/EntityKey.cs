using System;

namespace VitalChoice.Caching.Data
{
    internal class EntityKey : IEquatable<EntityKey>
    {
        public bool Equals(EntityKey other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return KeyInfo.Equals(other.KeyInfo) && Key.Equals(other.Key);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            var entityKey = obj as EntityKey;
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

        public static bool operator ==(EntityKey left, EntityKey right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(EntityKey left, EntityKey right)
        {
            return !Equals(left, right);
        }

        public EntityKey(EntityKeyInfo keyInfo, object key)
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