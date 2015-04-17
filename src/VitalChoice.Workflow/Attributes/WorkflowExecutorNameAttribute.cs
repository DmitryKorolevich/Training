using System;

namespace VitalChoice.Workflow.Attributes
{
    [AttributeUsage(AttributeTargets.Class)]
    public class WorkflowExecutorNameAttribute: Attribute
    {
        public WorkflowExecutorNameAttribute(string name)
        {
            Name = name;
        }

        public string Name { get; set; }
    }
}