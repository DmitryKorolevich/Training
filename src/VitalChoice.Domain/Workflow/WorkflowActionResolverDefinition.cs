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
            Actions = new Dictionary<int, WorkflowActionResolverPathDefinition>();
            Dependencies = new HashSet<Type>();
        }

        public Type Type { get; }
        public string Name { get; set; }
        public Dictionary<int, WorkflowActionResolverPathDefinition> Actions { get; set; }
        public HashSet<Type> Dependencies { get; set; }

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