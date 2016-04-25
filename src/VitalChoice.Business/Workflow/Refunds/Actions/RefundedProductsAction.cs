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
            context.RefundSkus = context.Order.RefundSkus.ToList();
            foreach (var refundSku in context.RefundSkus)
            {
                refundSku.RefundValue = refundSku.RefundPrice * (decimal)refundSku.RefundPercent / (decimal)100.0 * refundSku.Quantity;
                productsSubtotal += refundSku.RefundValue;
            }
            context.RefundSkus = context.RefundSkus.ToList();
            context.ProductsSubtotal = productsSubtotal;
            return Task.FromResult(productsSubtotal);
        }
    }
}