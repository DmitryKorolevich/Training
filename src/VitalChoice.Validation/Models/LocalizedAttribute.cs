using System;
using System.Linq;
using System.Reflection;

namespace VitalChoice.Validation.Models
{
    [AttributeUsage(AttributeTargets.Property, Inherited = false, AllowMultiple = false)]
    public sealed class LocalizedAttribute : Attribute
    {
        public string PropertyKey { get; private set; }

        public LocalizedAttribute(string propertyKey)
        {
            PropertyKey = propertyKey;
        }

        public static string GetFieldLabel(PropertyInfo propertyInfo)
        {
            var attribute = propertyInfo.GetCustomAttributes(typeof(LocalizedAttribute), false).FirstOrDefault() as
                LocalizedAttribute;
            if (attribute == null)
            {
                throw new ArgumentException(string.Format("LocalizedAttribute not set on property {0}.", propertyInfo.Name),
                    propertyInfo.Name);
            }
            return attribute.PropertyKey;
        }
    }
}
