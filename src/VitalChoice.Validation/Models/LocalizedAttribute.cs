using System;
using System.Linq;
using System.Reflection;
using VitalChoice.Domain.Entities.Localization;

namespace VitalChoice.Validation.Models
{
    [AttributeUsage(AttributeTargets.Property, Inherited = false, AllowMultiple = false)]
    public sealed class LocalizedAttribute : Attribute
    {
        public object EnumValue { get; private set; }

        public LocalizedAttribute(object enumValue)
        {
            EnumValue = enumValue;
        }

        public static object GetFieldLabel(PropertyInfo propertyInfo) 
        {
            var attribute = propertyInfo.GetCustomAttributes(typeof(LocalizedAttribute), false).FirstOrDefault() as
                LocalizedAttribute;
            if (attribute == null)
            {
                throw new ArgumentException(string.Format("LocalizedAttribute not set on property {0}.", propertyInfo.Name),
                    propertyInfo.Name);
            }
            return attribute.EnumValue;
        }
    }
}
