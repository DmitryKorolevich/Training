using System;
using System.Collections.Generic;
using VitalChoice.Domain.Workflow;

namespace VitalChoice.Domain.Entities.Workflow
{
    public class WorkflowExecutor: Entity
    {
        public string Name { get; set; }

        public string ImplementationType { get; set; }

        public WorkflowActionType ActionType { get; set; }

        public ICollection<WorkflowResolverPath> ResolverPaths { get; set; }

        public ICollection<WorkflowActionDependency> Dependencies { get; set; }
    }
}