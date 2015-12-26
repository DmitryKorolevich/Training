using System;

namespace VitalChoice.Caching.Expressions
{
    public struct RelationCacheInfo : IEquatable<RelationCacheInfo>
    {
        public RelationCacheInfo(string name, Type relatedType)
        {
            if (name == null) throw new ArgumentNullException(nameof(name));
            if (relatedType == null) throw new ArgumentNullException(nameof(relatedType));

            _name = name;
            _relatedType = relatedType;
        }

        public bool Equals(RelationCacheInfo other)
        {
            return string.Equals(_name, other._name) && _relatedType == other._relatedType;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            return obj is RelationCacheInfo && Equals((RelationCacheInfo)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (_name.GetHashCode() * 397) ^ _relatedType.GetHashCode();
            }
        }

        public static bool operator ==(RelationCacheInfo left, RelationCacheInfo right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(RelationCacheInfo left, RelationCacheInfo right)
        {
            return !left.Equals(right);
        }

        private readonly string _name;
        private readonly Type _relatedType;
    }
}