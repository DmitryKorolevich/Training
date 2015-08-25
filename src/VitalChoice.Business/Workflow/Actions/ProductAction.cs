using VitalChoice.Workflow.Base;
using System.Linq;
using VitalChoice.Workflow.Contexts;

namespace VitalChoice.Business.Workflow.Actions
{
    public class ProductAction: ComputableAction<OrderContext>
    {
        public ProductAction(ComputableTree<OrderContext> tree, string actionName) : base(tree, actionName)
        {
        }

        public override decimal ExecuteAction(OrderContext context)
        {
            return context.Order.Skus.Sum(s => s.Amount * s.Quantity);
        }
    }
}