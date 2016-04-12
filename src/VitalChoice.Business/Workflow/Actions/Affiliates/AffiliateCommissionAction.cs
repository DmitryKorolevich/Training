using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VitalChoice.Infrastructure.Domain.Transfer.Contexts;
using VitalChoice.Workflow.Base;
using VitalChoice.Workflow.Core;

namespace VitalChoice.Business.Workflow.Actions.Affiliates
{
    public class AffiliateCommissionAction : ComputableAction<OrderDataContext>
    {
        public AffiliateCommissionAction(ComputableTree<OrderDataContext> tree, string actionName) : base(tree, actionName)
        {
        }

        public override Task<decimal> ExecuteActionAsync(OrderDataContext context, IWorkflowExecutionContext executionContext)
        {
            //context.Order.
            return Task.FromResult<decimal>(0);
        }
    }
}
