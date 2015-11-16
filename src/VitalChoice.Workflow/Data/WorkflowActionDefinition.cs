using System;
using System.Collections.Generic;

namespace VitalChoice.Workflow.Data
{
    public class WorkflowActionDefinition : IEquatable<WorkflowActionDefinition>
    {
        public WorkflowActionDefinition(Type type, string name)
        {
            Type = type;
            Name = name;
            Dependencies = new HashSet<Type>();
            Aggregations = new HashSet<Type>();
        }

        public Type Type { get; }
        public string Name { get; set; }
        public HashSet<Type> Dependencies { get; set; }
        public HashSet<Type> Aggregations { get; set; }
        public bool Equals(WorkflowActionDefinition other) => other?.Type == Type;

        public override int GetHashCode()
        {
            return Type?.GetHashCode() ?? 0;
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;
            var descriptor = obj as WorkflowActionDefinition;
            return descriptor?.Type == Type;
        }

        public static bool operator ==(WorkflowActionDefinition actionOne, WorkflowActionDefinition actionTwo)
        {
            return actionOne?.Equals(actionTwo) ?? (object) actionTwo == null;
        }

        public static bool operator !=(WorkflowActionDefinition actionOne, WorkflowActionDefinition actionTwo)
        {
            return !(actionOne == actionTwo);
        }
    }
}