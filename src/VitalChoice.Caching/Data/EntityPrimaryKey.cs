﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace VitalChoice.Caching.Data
{
    internal class EntityPrimaryKey : IEquatable<EntityPrimaryKey>
    {
        private readonly IDictionary<string, EntityKeyValue> _keys;

        public EntityPrimaryKey(IEnumerable<EntityKeyValue> keys)
        {
            _keys = keys.ToDictionary(k => k.KeyInfo.Name);
        }

        public bool Equals(EntityPrimaryKey other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            
            // ReSharper disable once LoopCanBeConvertedToQuery
            foreach (var entityKey in _keys)
            {
                var thisKey = entityKey.Value;
                var otherKey = other._keys[entityKey.Key];
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
            var primaryKey = obj as EntityPrimaryKey;
            if (primaryKey != null)
                return Equals(primaryKey);
            return false;
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return _keys.Aggregate(0, (current, entityKey) => current*397 ^ entityKey.Value.GetHashCode());
            }
        }

        public static bool operator ==(EntityPrimaryKey left, EntityPrimaryKey right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(EntityPrimaryKey left, EntityPrimaryKey right)
        {
            return !Equals(left, right);
        }
    }
}