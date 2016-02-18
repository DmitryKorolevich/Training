using System;
using System.Reflection;
using Microsoft.Data.Entity.Metadata.Internal;
using VitalChoice.Ecommerce.Domain.Helpers;

namespace VitalChoice.Caching.Relational.Base
{
    public abstract class EntityValueInfo : IEquatable<EntityValueInfo>, IClrPropertyGetter
    {
        private readonly IClrPropertyGetter _property;
        private readonly Func<object, object> _valueConvert;

        protected EntityValueInfo(string name, IClrPropertyGetter property, Type propertyType)
        {
            if (name == null)
                throw new ArgumentNullException(nameof(name));

            _property = property;
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
            return Name.GetHashCode();
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

        public virtual bool Equals(EntityValueInfo other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return string.Equals(Name, other.Name);
        }

        public object GetClrValue(object instance)
        {
            return _valueConvert == null ? _property.GetClrValue(instance) : _valueConvert(_property.GetClrValue(instance));
        }
    }
}