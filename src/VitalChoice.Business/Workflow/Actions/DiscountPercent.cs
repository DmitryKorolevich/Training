using VitalChoice.Workflow.Base;
using VitalChoice.Workflow.Contexts;

namespace VitalChoice.Business.Workflow.Actions
{
    public class DiscountPercent: ComputableAction<OrderContext>
    {
        public DiscountPercent(ComputableTree<OrderContext> tree, string actionName) : base(tree, actionName)
        {
        }

        public override decimal ExecuteAction(OrderContext context)
        {
            return -context.Order.Discount.Data.Percent* Tree.GetActionResult("Products", context);
        }
    }
}