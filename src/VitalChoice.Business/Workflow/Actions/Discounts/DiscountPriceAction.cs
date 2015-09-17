using System;
using VitalChoice.Workflow.Base;
using VitalChoice.Workflow.Contexts;

namespace VitalChoice.Business.Workflow.Actions.Discounts
{
    public class DiscountPriceAction : ComputableAction<OrderContext>
    {
        public DiscountPriceAction(ComputableTree<OrderContext> tree, string actionName) : base(tree, actionName)
        {
        }

        public override decimal ExecuteAction(OrderContext context)
        {
            context.DiscountMessage = $"Price Discount ({context.Order.Discount.Data.Amount:C})";
            context.FreeShipping = context.Order.Discount.Data.FreeShipping;
            return -Math.Min(context.Data.DiscountableSubtotal, context.Order.Discount.Data.Amount);
        }
    }
}
