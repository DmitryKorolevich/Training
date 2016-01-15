using System;
using System.Collections.Generic;
using System.Linq;

namespace VitalChoice.Caching.Relational.Base
{
    public abstract class EntityValueGroupInfo<TInfo> : IEquatable<EntityValueGroupInfo<TInfo>>
        where TInfo: EntityValueInfo
    {
        public virtual int Count => ValuesDictionary.Count;

        internal readonly Dictionary<string, TInfo> ValuesDictionary;

        public ICollection<TInfo> InfoCollection => ValuesDictionary.Values;

        protected EntityValueGroupInfo(IEnumerable<TInfo> valueInfos)
        {
            if (valueInfos == null)
                throw new ArgumentNullException(nameof(valueInfos));

            ValuesDictionary = valueInfos.ToDictionary(v => v.Name);
        }

        public bool Equals(EntityValueGroupInfo<TInfo> other)
        {
            if (ReferenceEquals(null, other))
                return false;
            if (ReferenceEquals(this, other))
                return true;
            if (ValuesDictionary.Count != other.ValuesDictionary.Count)
                return false;
            return ValuesDictionary.All(i => other.ValuesDictionary.ContainsKey(i.Key));
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
                return false;
            if (ReferenceEquals(this, obj))
                return true;
            if (!(obj is EntityValueGroupInfo<TInfo>))
                return false;
            return Equals((EntityValueGroupInfo<TInfo>) obj);
        }

        private int? _hashCode;

        public override int GetHashCode()
        {
            if (!_hashCode.HasValue)
                _hashCode = ValuesDictionary.Values.Aggregate(0, (current, indexInfo) => (current*397) ^ indexInfo.GetHashCode());
            return _hashCode.Value;
        }

        public static bool operator ==(EntityValueGroupInfo<TInfo> left, EntityValueGroupInfo<TInfo> right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(EntityValueGroupInfo<TInfo> left, EntityValueGroupInfo<TInfo> right)
        {
            return !Equals(left, right);
        }

        public virtual bool TryGet(string name, out TInfo valueInfo)
        {
            return ValuesDictionary.TryGetValue(name, out valueInfo);
        }
    }
}