using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Data.Entity.Metadata.Internal;
using VitalChoice.DynamicData.Delegates;

namespace VitalChoice.Caching.Data
{
    internal class EntityKeyInfo : IEquatable<EntityKeyInfo>
    {
        public EntityKeyInfo(string name, IClrPropertyGetter property)
        {
            if (name == null)
                throw new ArgumentNullException(nameof(name));

            Property = property;
            Name = name;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            var keyInfo = obj as EntityKeyInfo;
            if (keyInfo != null)
                return Equals(keyInfo);
            return false;
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (Name.GetHashCode()*397) ^ Property.GetHashCode();
            }
        }

        public static bool operator ==(EntityKeyInfo left, EntityKeyInfo right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(EntityKeyInfo left, EntityKeyInfo right)
        {
            return !Equals(left, right);
        }

        public string Name { get; }

        public IClrPropertyGetter Property { get; }

        public bool Equals(EntityKeyInfo other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return string.Equals(Name, other.Name) && Property.Equals(other.Property);
        }
    }
}