using System.Linq;
using VitalChoice.DynamicData.Entities.Transfer;
using VitalChoice.Workflow.Base;
using VitalChoice.Workflow.Contexts;

namespace VitalChoice.Business.Workflow.Actions.Products
{
    public class ProductAction: ComputableAction<OrderContext>
    {
        public ProductAction(ComputableTree<OrderContext> tree, string actionName) : base(tree, actionName)
        {
        }

        public override decimal ExecuteAction(OrderContext context)
        {
            //TODO: check if skus is unique
            context.SkuOrdereds = context.Order.Skus.ToList();
            return context.SkuOrdereds.Sum(s => s.Amount * s.Quantity);
        }
    }
}