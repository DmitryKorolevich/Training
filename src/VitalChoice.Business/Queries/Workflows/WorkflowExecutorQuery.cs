using System;
using VitalChoice.Data.Helpers;
using VitalChoice.Ecommerce.Domain.Entities.Workflow;
using VitalChoice.Workflow.Data;

namespace VitalChoice.Business.Queries.Workflow
{
    public class WorkflowExecutorQuery: QueryObject<WorkflowExecutor>
    {
        public WorkflowExecutorQuery WithName(string name)
        {
            Add(tree => tree.Name == name);
            return this;
        }

        public WorkflowExecutorQuery WithImplementationType(Type implementationType)
        {
            Add(tree => tree.ImplementationType == implementationType.FullName);
            return this;
        }

        public WorkflowExecutorQuery WithType(WorkflowActionType actionType)
        {
            Add(tree => tree.ActionType == actionType);
            return this;
        }
    }
}
