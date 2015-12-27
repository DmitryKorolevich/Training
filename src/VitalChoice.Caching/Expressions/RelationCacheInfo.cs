using System;

namespace VitalChoice.Caching.Expressions
{
    public struct RelationCacheInfo : IEquatable<RelationCacheInfo>
    {
        public RelationCacheInfo(string name, Type relatedType, Type ownType)
        {
            if (name == null) throw new ArgumentNullException(nameof(name));
            if (relatedType == null) throw new ArgumentNullException(nameof(relatedType));
            if (ownType == null) throw new ArgumentNullException(nameof(ownType));

            _name = name;
            _relatedType = relatedType;
            _ownType = ownType;
        }

        public bool Equals(RelationCacheInfo other)
        {
            return string.Equals(_name, other._name) && _relatedType == other._relatedType && _ownType == other._ownType;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            return obj is RelationCacheInfo && Equals((RelationCacheInfo) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = _name.GetHashCode();
                hashCode = (hashCode*397) ^ _relatedType.GetHashCode();
                hashCode = (hashCode*397) ^ _ownType.GetHashCode();
                return hashCode;
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
        private readonly Type _ownType;
    }
}