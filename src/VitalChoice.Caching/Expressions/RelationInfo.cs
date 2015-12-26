using System;
using System.Collections.Generic;
using VitalChoice.Caching.Expressions.Visitors;

namespace VitalChoice.Caching.Expressions
{
    public class RelationInfo : IEquatable<RelationInfo>
    {
        public bool Equals(RelationInfo other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return string.Equals(Name, other.Name) && RelationEntityType == other.RelationEntityType;
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
            return Equals(left, right);
        }

        public static bool operator !=(RelationInfo left, RelationInfo right)
        {
            return !Equals(left, right);
        }

        public RelationInfo(string name, Type relatedType)
        {
            RelationEntityType = relatedType;
            Name = name;
            Relations = new Dictionary<RelationCacheInfo, RelationInfo>();
        }

        public string Name { get; }
        public Type RelationEntityType { get; }
        public Dictionary<RelationCacheInfo, RelationInfo> Relations { get; }
    }
}