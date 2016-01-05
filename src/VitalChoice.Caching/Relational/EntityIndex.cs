using System;
using System.Collections.Generic;
using System.Linq;

namespace VitalChoice.Caching.Relational
{
    public class EntityIndex : IEquatable<EntityIndex>
    {
        private readonly Dictionary<string, EntityIndexValue> _indexValues;

        public EntityIndex(IEnumerable<EntityIndexValue> values)
        {
            _indexValues = values.ToDictionary(k => k.IndexInfo.Name);
        }

        public bool Equals(EntityIndex other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;

            // ReSharper disable once LoopCanBeConvertedToQuery
            foreach (var entityKey in _indexValues)
            {
                var thisKey = entityKey.Value;
                var otherKey = other._indexValues[entityKey.Key];
                if (!thisKey.Equals(otherKey))
                {
                    return false;
                }
            }
            return true;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            var primaryKey = obj as EntityIndex;
            if (primaryKey != null)
                return Equals(primaryKey);
            return false;
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return _indexValues.Aggregate(0, (current, indexValue) => (current*397) ^ indexValue.Value.GetHashCode());
            }
        }

        public static bool operator ==(EntityIndex left, EntityIndex right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(EntityIndex left, EntityIndex right)
        {
            return !Equals(left, right);
        }

    }
}
