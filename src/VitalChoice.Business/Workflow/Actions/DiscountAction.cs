using VitalChoice.Business.Workflow.Contexts;
using VitalChoice.Workflow.Base;

namespace VitalChoice.Business.Workflow.Actions
{
    public class DiscountAction: ComputableAction<OrderContext>
    {
        public DiscountAction(ComputableActionTree<OrderContext> tree, string actionName) : base(tree, actionName)
        {
        }

        public override decimal ExecuteAction(OrderContext context)
        {
            return (decimal) (-context.DiscountPercent*(double) ActionTree.GetActionResult("Products", context));
        }
    }
}