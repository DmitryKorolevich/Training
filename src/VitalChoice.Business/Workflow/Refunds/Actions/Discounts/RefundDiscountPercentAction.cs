using System.Threading.Tasks;
using VitalChoice.Business.Helpers;
using VitalChoice.Infrastructure.Domain.Transfer.Contexts;
using VitalChoice.Workflow.Base;
using VitalChoice.Workflow.Core;

namespace VitalChoice.Business.Workflow.Refunds.Actions.Discounts
{
    public class RefundDiscountPercentAction : ComputableAction<OrderRefundDataContext>
    {
        public RefundDiscountPercentAction(ComputableTree<OrderRefundDataContext> tree, string actionName) : base(tree, actionName)
        {
        }

        public override Task<decimal> ExecuteActionAsync(OrderRefundDataContext context, ITreeContext executionContext)
        {
            context.DiscountMessage = context.RefundOrder.Discount.GetDiscountMessage();
            decimal discountPercent = context.RefundOrder.Discount.Data.Percent;

            context.SplitInfo.PerishableDiscount = discountPercent*context.SplitInfo.DiscountablePerishable/100;
            context.SplitInfo.NonPerishableDiscount = discountPercent*context.SplitInfo.DiscountableNonPerishable/100;

            return Task.FromResult(-discountPercent*(decimal) context.Data.RefundDiscountableSubtotal / 100);
        }
    }
}