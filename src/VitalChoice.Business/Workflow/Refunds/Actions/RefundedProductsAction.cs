using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VitalChoice.Infrastructure.Domain.Transfer.Contexts;
using VitalChoice.Workflow.Base;
using VitalChoice.Workflow.Core;

namespace VitalChoice.Business.Workflow.Refunds.Actions
{
    public class RefundedProductsAction:ComputableAction<OrderRefundDataContext>
    {
        public RefundedProductsAction(ComputableTree<OrderRefundDataContext> tree, string actionName) : base(tree, actionName)
        {
        }

        public override Task<decimal> ExecuteActionAsync(OrderRefundDataContext context, IWorkflowExecutionContext executionContext)
        {
            decimal productsSubtotal = 0;
            foreach (var refundSku in context.RefundSkus)
            {
                productsSubtotal += refundSku.RefundValue*(decimal) refundSku.RefundPercent/(decimal) 100.0*refundSku.Quantity;
            }
            context.RefundSkus = context.RefundSkus.ToList();
            return Task.FromResult(productsSubtotal);
        }
    }
}