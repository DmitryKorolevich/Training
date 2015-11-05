using System.Linq;
using System.Threading.Tasks;
using VitalChoice.DynamicData.Entities.Transfer;
using VitalChoice.Workflow.Base;
using VitalChoice.Workflow.Contexts;
using VitalChoice.Workflow.Core;

namespace VitalChoice.Business.Workflow.Actions.Products
{
    public class ProductAction: ComputableAction<OrderDataContext>
    {
        public ProductAction(ComputableTree<OrderDataContext> tree, string actionName) : base(tree, actionName)
        {
        }

        public override Task<decimal> ExecuteActionAsync(OrderDataContext dataContext, IWorkflowExecutionContext executionContext)
        {
            dataContext.SkuOrdereds = dataContext.Order.Skus.Where(s => s.Quantity > 0).ToList();
            return Task.FromResult(dataContext.SkuOrdereds.Sum(s => s.Amount * s.Quantity));
        }
    }
}