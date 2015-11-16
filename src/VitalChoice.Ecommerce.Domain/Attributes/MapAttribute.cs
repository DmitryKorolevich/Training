using System;

namespace VitalChoice.Ecommerce.Domain.Attributes
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class MapAttribute : Attribute
    {
        public string Name { get; }

        public MapAttribute(string name = null)
        {
            Name = name;
        }
    }
}