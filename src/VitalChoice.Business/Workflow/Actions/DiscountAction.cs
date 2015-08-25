using VitalChoice.Workflow.Base;
using VitalChoice.Workflow.Contexts;

namespace VitalChoice.Business.Workflow.Actions
{
    public class DiscountAction: ComputableAction<OrderContext>
    {
        public DiscountAction(ComputableTree<OrderContext> tree, string actionName) : base(tree, actionName)
        {
        }

        public override decimal ExecuteAction(OrderContext context)
        {
            return 0; //(decimal) (-context.Order.Discount*(double) Tree.GetActionResult("Products", context));
        }
    }
}