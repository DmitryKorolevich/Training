using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VitalChoice.Infrastructure.Domain.Transfer.Contexts;
using VitalChoice.Workflow.Base;
using VitalChoice.Workflow.Core;

namespace VitalChoice.Business.Workflow.Refunds.Actions
{
    public class ManualOverrideAction : ComputableAction<OrderRefundDataContext>
    {
        public ManualOverrideAction(ComputableTree<OrderRefundDataContext> tree, string actionName) : base(tree, actionName)
        {
        }

        public override Task<decimal> ExecuteActionAsync(OrderRefundDataContext context, ITreeContext executionContext)
        {
            context.ManualRefundOverride = (decimal?) context.RefundOrder.SafeData.ManualRefundOverride ?? 0;
            return Task.FromResult(context.ManualRefundOverride);
        }
    }
}
