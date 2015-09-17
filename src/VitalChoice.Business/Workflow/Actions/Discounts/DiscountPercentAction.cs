using VitalChoice.Workflow.Base;
using VitalChoice.Workflow.Contexts;

namespace VitalChoice.Business.Workflow.Actions.Discounts
{
    public class DiscountPercentAction: ComputableAction<OrderContext>
    {
        public DiscountPercentAction(ComputableTree<OrderContext> tree, string actionName) : base(tree, actionName)
        {
        }

        public override decimal ExecuteAction(OrderContext context)
        {
            context.DiscountMessage = $"Percent Discount ({context.Order.Discount.Data.Percent / 100:P0})";
            return -context.Order.Discount.Data.Percent / (decimal)100.0*context.Data.DiscountableSubtotal;
        }
    }
}