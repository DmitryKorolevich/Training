using System;
using VitalChoice.Workflow.Base;
using VitalChoice.Workflow.Implementations.Contexts;

namespace VitalChoice.Workflow.Implementations.Actions
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