using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace VitalChoice.Caching.Relational.ChangeTracking
{
    public struct TrackedEntityKey
    {
        public bool Equals(TrackedEntityKey other)
        {
            return EntityType == other.EntityType && PrimaryKey == other.PrimaryKey;
        }

        public override bool Equals(object obj)
        {
            if ((object) this == obj) return true;
            if (obj == null) return false;
            var other = obj as TrackedEntityKey? ?? new TrackedEntityKey();
            return Equals(other);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (EntityType.GetHashCode()*397) ^ (PrimaryKey?.GetHashCode() ?? 0);
            }
        }

        public static bool Equals(TrackedEntityKey left, TrackedEntityKey right)
        {
            return left.Equals(right);
        }

        public static bool operator ==(TrackedEntityKey left, TrackedEntityKey right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(TrackedEntityKey left, TrackedEntityKey right)
        {
            return !Equals(left, right);
        }

        public TrackedEntityKey(Type entityType, EntityKey primaryKey)
        {
            EntityType = entityType;
            PrimaryKey = primaryKey;
        }

        public Type EntityType { get; }
        public EntityKey PrimaryKey { get; }
    }
}
