using VitalChoice.Workflow.Base;
using VitalChoice.Workflow.Contexts;

namespace VitalChoice.Business.Workflow.Actions
{
    public class DiscountPercentAction: ComputableAction<OrderContext>
    {
        public DiscountPercentAction(ComputableTree<OrderContext> tree, string actionName) : base(tree, actionName)
        {
        }

        public override decimal ExecuteAction(OrderContext context)
        {
            return -context.Order.Discount.Data.Percent*context.Data.DiscountableSubtotal;
        }
    }
}