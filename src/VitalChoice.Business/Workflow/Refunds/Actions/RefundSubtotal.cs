using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VitalChoice.Infrastructure.Domain.Transfer.Contexts;
using VitalChoice.Workflow.Base;
using VitalChoice.Workflow.Core;

namespace VitalChoice.Business.Workflow.Refunds.Actions
{
    public class RefundSubtotal:ComputableAction<OrderRefundDataContext>
    {
        public RefundSubtotal(ComputableTree<OrderRefundDataContext> tree, string actionName) : base(tree, actionName)
        {
        }

        public override Task<decimal> ExecuteActionAsync(OrderRefundDataContext context, IWorkflowExecutionContext executionContext)
        {
            decimal discount = 0;
            if (context.SafeData.Discount != null)
            {
                discount = context.Data.Discount;
            }

            context.DiscountTotal = -discount;
            context.DiscountedSubtotal = context.ProductsSubtotal + discount;
            context.AutoTotal = context.DiscountedSubtotal + context.ShippingTotal + context.TaxTotal;

            return Task.FromResult<decimal>(0);
        }
    }
}