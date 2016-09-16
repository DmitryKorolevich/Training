using System;
using System.Threading.Tasks;
using VitalChoice.Business.Helpers;
using VitalChoice.Infrastructure.Domain.Transfer.Contexts;
using VitalChoice.Workflow.Base;
using VitalChoice.Workflow.Core;

namespace VitalChoice.Business.Workflow.Refunds.Actions.Discounts
{
    public class RefundDiscountPriceAction : ComputableAction<OrderRefundDataContext>
    {
        public RefundDiscountPriceAction(ComputableTree<OrderRefundDataContext> tree, string actionName) : base(tree, actionName)
        {
        }

        public override Task<decimal> ExecuteActionAsync(OrderRefundDataContext context, ITreeContext executionContext)
        {
            context.DiscountMessage = context.RefundOrder.Discount.GetDiscountMessage();

            decimal totalDiscount = Math.Min(context.Data.RefundDiscountableSubtotal, context.RefundOrder.Discount.Data.Amount);

            context.SplitInfo.PerishableDiscount = Math.Min(context.SplitInfo.DiscountablePerishable, context.RefundOrder.Discount.Data.Amount);
            context.SplitInfo.NonPerishableDiscount = totalDiscount - context.SplitInfo.PerishableDiscount;
            return Task.FromResult(-totalDiscount);
        }
    }
}