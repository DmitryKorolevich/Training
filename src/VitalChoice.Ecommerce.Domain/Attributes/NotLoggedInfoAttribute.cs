using System;

namespace VitalChoice.Ecommerce.Domain.Attributes
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class NotLoggedInfoAttribute : Attribute
    {
        public NotLoggedInfoAttribute()
        {
        }
    }
}