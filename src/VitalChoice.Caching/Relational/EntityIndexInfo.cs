using System;
using Microsoft.Data.Entity.Metadata.Internal;

namespace VitalChoice.Caching.Relational
{
    public class EntityIndexInfo : IEquatable<EntityIndexInfo>
    {
        public EntityIndexInfo(string name, IClrPropertyGetter property, Type propertyType)
        {
            if (name == null)
                throw new ArgumentNullException(nameof(name));

            Property = property;
            PropertyType = propertyType;
            Name = name;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            var keyInfo = obj as EntityIndexInfo;
            if (keyInfo != null)
                return Equals(keyInfo);
            return false;
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (Name.GetHashCode() * 397) ^ Property.GetHashCode();
            }
        }

        public static bool operator ==(EntityIndexInfo left, EntityIndexInfo right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(EntityIndexInfo left, EntityIndexInfo right)
        {
            return !Equals(left, right);
        }

        public string Name { get; }

        public IClrPropertyGetter Property { get; }
        public Type PropertyType { get; }

        public bool Equals(EntityIndexInfo other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return string.Equals(Name, other.Name) && Property.Equals(other.Property);
        }
    }
}
