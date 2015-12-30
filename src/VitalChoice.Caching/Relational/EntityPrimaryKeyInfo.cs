using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace VitalChoice.Caching.Relational
{
    public class EntityPrimaryKeyInfo : IEquatable<EntityPrimaryKeyInfo>
    {
        public bool Equals(EntityPrimaryKeyInfo other)
        {
            if (ReferenceEquals(null, other))
                return false;
            if (ReferenceEquals(this, other))
                return true;
            if (_keyInfo.Count != other._keyInfo.Count)
                return false;
            return _keyInfo.All(i => other._keyInfo.Contains(i));
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
                return false;
            if (ReferenceEquals(this, obj))
                return true;
            if (!(obj is EntityPrimaryKeyInfo))
                return false;
            return Equals((EntityPrimaryKeyInfo)obj);
        }

        public override int GetHashCode()
        {
            return _keyInfo.Aggregate(0, (current, indexInfo) => (current * 397) ^ indexInfo.GetHashCode());
        }

        public static bool operator ==(EntityPrimaryKeyInfo left, EntityPrimaryKeyInfo right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(EntityPrimaryKeyInfo left, EntityPrimaryKeyInfo right)
        {
            return !Equals(left, right);
        }

        private readonly HashSet<EntityKeyInfo> _keyInfo;
        public IDictionary<string, EntityKeyInfo> KeyInfoFields { get; }

        public EntityPrimaryKeyInfo(IEnumerable<EntityKeyInfo> keyInfos)
        {
            if (keyInfos == null) throw new ArgumentNullException(nameof(keyInfos));
            _keyInfo = new HashSet<EntityKeyInfo>(keyInfos);
            KeyInfoFields = _keyInfo.ToDictionary(k => k.Name);
        }

        public ICollection<EntityKeyInfo> KeyInfo => _keyInfo;
    }
}