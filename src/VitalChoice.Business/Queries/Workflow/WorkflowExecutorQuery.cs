using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VitalChoice.Data.Helpers;
using VitalChoice.Domain.Entities.Workflow;
using VitalChoice.Domain.Workflow;

namespace VitalChoice.Business.Queries.Workflow
{
    public class WorkflowExecutorQuery: QueryObject<WorkflowExecutor>
    {
        public WorkflowExecutorQuery WithName(string name)
        {
            Add(tree => tree.Name == name);
            return this;
        }

        public WorkflowExecutorQuery WithType(WorkflowActionType actionType)
        {
            Add(tree => tree.ActionType == actionType);
            return this;
        }
    }
}
