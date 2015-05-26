using VitalChoice.Business.Workflow.Contexts;
using VitalChoice.Workflow.Base;

namespace VitalChoice.Business.Workflow.Actions
{
    public class DiscountAction: ComputableAction<OrderContext>
    {
        public DiscountAction(ComputableTree<OrderContext> tree, string actionName) : base(tree, actionName)
        {
        }

        public override decimal ExecuteAction(OrderContext context)
        {
            return (decimal) (-context.DiscountPercent*(double) Tree.GetActionResult("Products", context));
        }
    }
}