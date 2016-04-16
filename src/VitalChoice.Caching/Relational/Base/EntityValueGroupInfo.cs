using System;
using System.Collections.Generic;
using System.Linq;
using VitalChoice.Caching.Extensions;

namespace VitalChoice.Caching.Relational.Base
{
    public abstract class EntityValueGroupInfo<TInfo> : IEquatable<EntityValueGroupInfo<TInfo>>
        where TInfo: EntityValueInfo
    {
        public virtual int Count => InfoDictionary.Count;

        internal readonly IReadOnlyDictionary<string, TInfo> InfoDictionary;

        public TInfo[] Infos { get; }

        protected EntityValueGroupInfo(IEnumerable<TInfo> valueInfos)
        {
            if (valueInfos == null)
                throw new ArgumentNullException(nameof(valueInfos));

            var dict = valueInfos.ToDictionary(v => v.Name);
            InfoDictionary = dict;
            Infos = dict.Values.Normalize();
        }

        public bool Equals(EntityValueGroupInfo<TInfo> other)
        {
            if ((object)this == (object)other)
                return true;
            if ((object)other == null)
                return false;
            if (InfoDictionary.Count != other.InfoDictionary.Count)
                return false;
            return InfoDictionary.All(i => other.InfoDictionary.ContainsKey(i.Key));
        }

        public override bool Equals(object obj)
        {
            if ((object)this == obj)
                return true;
            var groupInfo = obj as EntityValueGroupInfo<TInfo>;
            return groupInfo != null && Equals(groupInfo);
        }

        public override string ToString()
        {
            return $"({string.Join(", ", InfoDictionary.Values.Select(v => v.ToString()))})";
        }

        public override int GetHashCode()
        {
            int result = 0;
            // ReSharper disable once LoopCanBeConvertedToQuery
            foreach (var value in InfoDictionary.Values)
                result = (result*397) ^ value.GetHashCode();
            return result;
        }

        public static bool Equals(EntityValueGroupInfo<TInfo> left, EntityValueGroupInfo<TInfo> right)
        {
            return left?.Equals(right) ?? (object) right == null;
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
            return InfoDictionary.TryGetValue(name, out valueInfo);
        }
    }
}