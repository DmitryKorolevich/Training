using System;

namespace VitalChoice.Domain.Attributes
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class NotLoggedInfoAttribute : Attribute
    {
        public NotLoggedInfoAttribute()
        {
        }
    }
}