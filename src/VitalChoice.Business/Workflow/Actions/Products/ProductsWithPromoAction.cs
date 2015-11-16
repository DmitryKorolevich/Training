using System.Linq;
using System.Threading.Tasks;
using VitalChoice.Infrastructure.Domain.Transfer.Contexts;
using VitalChoice.Workflow.Base;
using VitalChoice.Workflow.Core;

namespace VitalChoice.Business.Workflow.Actions.Products
{
    public class ProductsWithPromoAction : ComputableAction<OrderDataContext>
    {
        public ProductsWithPromoAction(ComputableTree<OrderDataContext> tree, string actionName) : base(tree, actionName)
        {
        }

        public override Task<decimal> ExecuteActionAsync(OrderDataContext dataContext, IWorkflowExecutionContext executionContext)
        {
            var promoAmount = dataContext.PromoSkus.Sum(p => p.Amount*p.Quantity);
            dataContext.ProductsSubtotal = dataContext.Data.Products + promoAmount;
            return Task.FromResult(promoAmount);
        }
    }
}
