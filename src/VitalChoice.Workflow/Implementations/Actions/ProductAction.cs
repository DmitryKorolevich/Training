using System;
using System.Linq;
using VitalChoice.Workflow.Base;
using VitalChoice.Workflow.Implementations.Contexts;

namespace VitalChoice.Workflow.Implementations.Actions
{
    public class ProductAction: ComputableAction<OrderContext>
    {
        public ProductAction(ComputableActionTree<OrderContext> tree) : base(tree)
        {
        }

        public override decimal ExecuteAction(OrderContext context)
        {
            return context.ProductCosts.Sum();
        }
    }
}