using System;
using VitalChoice.Workflow.Base;
using VitalChoice.Workflow.Implementations.Contexts;

namespace VitalChoice.Workflow.Implementations.Actions
{
    public class DiscountAction: ComputableAction<OrderContext>
    {
        public DiscountAction(ComputableActionTree<OrderContext> tree) : base(tree)
        {
        }

        public override decimal ExecuteAction(OrderContext context)
        {
            return (decimal) (-context.DiscountPercent*(double) ActionTree.GetActionResult<ProductAction>(context));
        }
    }
}