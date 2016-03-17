using System;
using System.Reflection;
using Microsoft.Data.Entity.Metadata.Internal;
using VitalChoice.Ecommerce.Domain.Helpers;

namespace VitalChoice.Caching.Relational.Base
{
    public class EntityValueInfo : IEquatable<EntityValueInfo>, IClrPropertyGetter
    {
        private readonly IClrPropertyGetter _property;
        public Type PropertyType { get; }
        private readonly Func<object, object> _valueConvert;

        public EntityValueInfo(string name, IClrPropertyGetter property, Type propertyType)
        {
            if (name == null)
                throw new ArgumentNullException(nameof(name));

            _property = property;
            PropertyType = propertyType;
            if (propertyType.GetTypeInfo().IsEnum)
            {
#if !DOTNET5_4
                var enumTypeCode = Enum.GetUnderlyingType(propertyType).GetTypeCode();
                _valueConvert = value => Convert.ChangeType(value, enumTypeCode);
#else
                var enumType = Enum.GetUnderlyingType(propertyType);
                _valueConvert = value => Convert.ChangeType(value, enumType);
#endif
            }

            Name = name;
        }

        public virtual bool Equals(EntityValueInfo other)
        {
            if ((object) this == (object) other) return true;
            if ((object) other == null) return false;
            return string.Equals(Name, other.Name);
        }

        public override bool Equals(object obj)
        {
            if ((object)this == obj)
                return true;
            var keyInfo = obj as EntityValueInfo;
            if (keyInfo != null)
                return Equals(keyInfo);
            return false;
        }

        public override int GetHashCode()
        {
            return Name.GetHashCode();
        }

        public override string ToString()
        {
            return $"[{PropertyType}] {Name}";
        }

        public static bool Equals(EntityValueInfo left, EntityValueInfo right)
        {
            return left?.Equals(right) ?? (object) right == null;
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

        public object GetClrValue(object instance)
        {
            return _valueConvert == null ? _property.GetClrValue(instance) : _valueConvert(_property.GetClrValue(instance));
        }
    }
}