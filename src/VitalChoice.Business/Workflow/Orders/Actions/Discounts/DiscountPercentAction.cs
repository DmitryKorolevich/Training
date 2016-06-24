using System.Threading.Tasks;
using VitalChoice.Business.Helpers;
using VitalChoice.Infrastructure.Domain.Transfer.Contexts;
using VitalChoice.Workflow.Base;
using VitalChoice.Workflow.Core;

namespace VitalChoice.Business.Workflow.Orders.Actions.Discounts
{
    public class DiscountPercentAction: ComputableAction<OrderDataContext>
    {
        public DiscountPercentAction(ComputableTree<OrderDataContext> tree, string actionName) : base(tree, actionName)
        {
        }

        public override Task<decimal> ExecuteActionAsync(OrderDataContext context, ITreeContext executionContext)
        {
            context.DiscountMessage = context.Order.Discount.GetDiscountMessage();
            context.FreeShipping = context.Order.Discount.Data.FreeShipping;

            decimal discountPercent = context.Order.Discount.Data.Percent;

            context.SplitInfo.PerishableDiscount = discountPercent*context.SplitInfo.DiscountablePerishable / 100;
            context.SplitInfo.NonPerishableDiscount = discountPercent*context.SplitInfo.DiscountableNonPerishable / 100;

            return Task.FromResult(-discountPercent*(decimal) context.Data.DiscountableSubtotal/100);
        }
    }
}