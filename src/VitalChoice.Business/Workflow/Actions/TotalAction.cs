using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VitalChoice.Workflow.Base;
using VitalChoice.Workflow.Contexts;
using VitalChoice.Workflow.Core;

namespace VitalChoice.Business.Workflow.Actions
{
    public class TotalAction : ComputableAction<OrderDataContext>
    {
        public TotalAction(ComputableTree<OrderDataContext> tree, string actionName) : base(tree, actionName)
        {
        }

        public override Task<decimal> ExecuteAction(OrderDataContext dataContext, IWorkflowExecutionContext executionContext)
        {
            return Task.FromResult<decimal>(0);
        }
    }
}
