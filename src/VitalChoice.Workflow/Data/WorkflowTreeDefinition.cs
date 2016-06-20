using System;
using System.Collections.Generic;

namespace VitalChoice.Workflow.Data
{
    public class WorkflowTreeDefinition : IEquatable<WorkflowTreeDefinition>
    {
        public WorkflowTreeDefinition(Type type, string name)
        {
            Type = type;
            Name = name;
        }

        public Type Type { get; }
        public string Name { get; }
        public Dictionary<Type, WorkflowActionDefinition> Actions { get; set; }
        public Dictionary<Type, WorkflowActionResolverDefinition> ActionResolvers { get; set; }

        public bool Equals(WorkflowTreeDefinition other) => other?.Type == Type;

        public override int GetHashCode()
        {
            return Type?.GetHashCode() ?? 0;
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;
            var descriptor = obj as WorkflowTreeDefinition;
            return descriptor?.Type == Type;
        }
    }
}