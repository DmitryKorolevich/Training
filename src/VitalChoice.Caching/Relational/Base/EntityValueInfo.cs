using System;
using Microsoft.Data.Entity.Metadata.Internal;

namespace VitalChoice.Caching.Relational.Base
{
    public abstract class EntityValueInfo : IEquatable<EntityValueInfo>
    {
        protected EntityValueInfo(string name, IClrPropertyGetter property)
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
            var keyInfo = obj as EntityValueInfo;
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

        public static bool operator ==(EntityValueInfo left, EntityValueInfo right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(EntityValueInfo left, EntityValueInfo right)
        {
            return !Equals(left, right);
        }

        public virtual string Name { get; }

        public virtual IClrPropertyGetter Property { get; }

        public virtual bool Equals(EntityValueInfo other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return string.Equals(Name, other.Name) && Property.Equals(other.Property);
        }
    }
}