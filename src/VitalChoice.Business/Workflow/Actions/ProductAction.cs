using VitalChoice.Business.Workflow.Contexts;
using VitalChoice.Workflow.Base;
using System.Linq;

namespace VitalChoice.Business.Workflow.Actions
{
    public class ProductAction: ComputableAction<OrderContext>
    {
        public ProductAction(ComputableTree<OrderContext> tree, string actionName) : base(tree, actionName)
        {
        }

        public override decimal ExecuteAction(OrderContext context)
        {
            return context.ProductCosts.Sum();
        }
    }
}