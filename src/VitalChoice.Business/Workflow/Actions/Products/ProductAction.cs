using System.Linq;
using System.Threading.Tasks;
using VitalChoice.Infrastructure.Domain.Transfer.Contexts;
using VitalChoice.Workflow.Base;
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
            foreach (var sku in dataContext.SkuOrdereds)
            {
                if (!(sku.Sku.SafeData.DisregardStock ?? true))
                {
                    if (sku.Sku.SafeData.Stock < sku.Quantity)
                    {
                        sku.Messages.Add("Out of stock");
                    }
                }
            }
            return Task.FromResult(dataContext.SkuOrdereds.Sum(s => s.Amount * s.Quantity));
        }
    }
}