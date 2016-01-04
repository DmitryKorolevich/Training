using System;
using System.Collections.Generic;
using System.Linq;

namespace VitalChoice.Caching.Relational
{
    public class EntityUniqueIndexInfo : IEquatable<EntityUniqueIndexInfo>
    {
        public bool Equals(EntityUniqueIndexInfo other)
        {
            if (ReferenceEquals(null, other))
                return false;
            if (ReferenceEquals(this, other))
                return true;
            if (IndexInfoInternal.Count != other.IndexInfoInternal.Count)
                return false;
            return IndexInfoInternal.All(i => other.IndexInfoInternal.Contains(i));
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
                return false;
            if (ReferenceEquals(this, obj))
                return true;
            if (!(obj is EntityUniqueIndexInfo))
                return false;
            return Equals((EntityUniqueIndexInfo) obj);
        }

        public override int GetHashCode()
        {
            return IndexInfoInternal.Aggregate(0, (current, indexInfo) => (current*397) ^ indexInfo.GetHashCode());
        }

        public static bool operator ==(EntityUniqueIndexInfo left, EntityUniqueIndexInfo right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(EntityUniqueIndexInfo left, EntityUniqueIndexInfo right)
        {
            return !Equals(left, right);
        }

        internal Dictionary<string, EntityIndexInfo> IndexInfoInternal { get; }

        public EntityUniqueIndexInfo(IEnumerable<EntityIndexInfo> indexInfo)
        {
            if (indexInfo == null) throw new ArgumentNullException(nameof(indexInfo));
            IndexInfoInternal = indexInfo.ToDictionary(i => i.Name);
        }

        public ICollection<EntityIndexInfo> IndexInfos => IndexInfoInternal.Values;
    }
}
