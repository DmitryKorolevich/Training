using System.Linq;
using System.Threading.Tasks;
using VitalChoice.Ecommerce.Domain.Entities.Products;
using VitalChoice.Infrastructure.Domain.Transfer.Contexts;
using VitalChoice.Workflow.Base;
using VitalChoice.Workflow.Core;

namespace VitalChoice.Business.Workflow.Orders.Actions.Products
{
    public class DeliveredProductsAction: ComputableAction<OrderDataContext>
    {
        public DeliveredProductsAction(ComputableTree<OrderDataContext> tree, string actionName) : base(tree, actionName)
        {
        }

        public override Task<decimal> ExecuteActionAsync(OrderDataContext dataContext, IWorkflowExecutionContext executionContext)
        {
            return
                Task.FromResult(dataContext.SkuOrdereds.Union(dataContext.PromoSkus.Where(p => p.Enabled)).Where(s =>
                    s.Sku.IdObjectType == (int) ProductType.Perishable ||
                    s.Sku.IdObjectType == (int) ProductType.NonPerishable).Sum(s => s.Amount*s.Quantity));
        }
    }
}
