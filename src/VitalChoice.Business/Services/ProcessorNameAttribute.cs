using System;

namespace VitalChoice.Business.Services
{
    [AttributeUsage(AttributeTargets.Class, Inherited = false)]
    public class ProcessorNameAttribute : Attribute
    {
        public ProcessorNameAttribute(string name)
        {
            Name = name;
        }

        public string Name { get; }
    }
}