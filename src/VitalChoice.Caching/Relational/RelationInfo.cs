using System;
using System.Collections.Generic;
using System.Linq;
using VitalChoice.Caching.Expressions;

namespace VitalChoice.Caching.Relational
{
    public class RelationInfo : IEquatable<RelationInfo>
    {
        public bool Equals(RelationInfo other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;

            if (!string.Equals(Name, other.Name) || RelationEntityType != other.RelationEntityType || ParentEntityType != other.ParentEntityType)
                return false;

            if (other.RelationsDict.Count != RelationsDict.Count)
                return false;

            return other.RelationsDict.All(r =>
            {
                RelationInfo related;
                return RelationsDict.TryGetValue(r.Key, out related) && related.Equals(r.Value);
            });
        }

        public bool LessThanOrEqualTo(RelationInfo other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;

            if (!string.Equals(Name, other.Name) || RelationEntityType != other.RelationEntityType || ParentEntityType != other.ParentEntityType)
                return false;

            if (RelationsDict.Count > other.RelationsDict.Count)
                return false;

            return RelationsDict.All(r =>
            {
                RelationInfo related;
                return other.RelationsDict.TryGetValue(r.Key, out related) && r.Value.LessThanOrEqualTo(related);
            });
        }

        public bool GreaterThanOrEqualTo(RelationInfo other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;

            if (!string.Equals(Name, other.Name) || RelationEntityType != other.RelationEntityType || ParentEntityType != other.ParentEntityType)
                return false;

            if (RelationsDict.Count < other.RelationsDict.Count)
                return false;

            return !RelationsDict.Any(r =>
            {
                RelationInfo related;
                return other.RelationsDict.TryGetValue(r.Key, out related) && r.Value.LessThanOrEqualTo(related);
            });
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (!(obj is RelationInfo)) return false;
            return Equals((RelationInfo) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (Name.GetHashCode()*397) ^ RelationEntityType.GetHashCode();
            }
        }

        public static bool operator ==(RelationInfo left, RelationInfo right)
        {
            if ((object) left == (object) right)
                return true;
            return (object) left != null && left.Equals(right);
        }

        public static bool operator !=(RelationInfo left, RelationInfo right)
        {
            if ((object) left == (object) right)
                return false;
            return (object) left == null || !left.Equals(right);
        }

        public static bool operator >=(RelationInfo left, RelationInfo right)
        {
            return left?.GreaterThanOrEqualTo(right) ?? false;
        }

        public static bool operator <=(RelationInfo left, RelationInfo right)
        {
            return left?.LessThanOrEqualTo(right) ?? false;
        }

        public RelationInfo(string name, Type relatedType, Type ownedType)
        {
            ParentEntityType = ownedType;
            RelationEntityType = relatedType;
            Name = name;
        }

        public string Name { get; }
        public Type ParentEntityType { get; }
        public Type RelationEntityType { get; }
        internal Dictionary<RelationCacheInfo, RelationInfo> RelationsDict { get; } = new Dictionary<RelationCacheInfo, RelationInfo>();
        public ICollection<RelationInfo> Relations => RelationsDict.Values;

    }
}