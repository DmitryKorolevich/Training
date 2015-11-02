using System;

namespace VitalChoice.ContentProcessing.Base
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