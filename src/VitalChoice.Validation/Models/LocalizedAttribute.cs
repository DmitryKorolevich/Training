using System;

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
    }
}
