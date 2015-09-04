using System;
using System.Collections.Generic;

namespace VitalChoice.Domain.Workflow
{
    public class WorkflowActionResolverDefinition : IEquatable<WorkflowActionResolverDefinition>
    {
        public WorkflowActionResolverDefinition(Type type, string name)
        {
            Type = type;
            Name = name;
        }

        public Type Type { get; }
        public string Name { get; set; }
        public Dictionary<int, Type> Actions { get; set; }

        public bool Equals(WorkflowActionResolverDefinition other) => other?.Type == Type;

        public override int GetHashCode()
        {
            return Type?.GetHashCode() ?? 0;
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;
            var descriptor = obj as WorkflowActionResolverDefinition;
            return descriptor?.Type == Type;
        }
    }
}