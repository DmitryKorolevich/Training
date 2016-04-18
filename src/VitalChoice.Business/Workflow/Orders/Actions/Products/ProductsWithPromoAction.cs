using System.Linq;
using System.Threading.Tasks;
using VitalChoice.Ecommerce.Domain.Exceptions;
using VitalChoice.Infrastructure.Domain.Transfer.Contexts;
using VitalChoice.Workflow.Base;
using VitalChoice.Workflow.Core;

namespace VitalChoice.Business.Workflow.Orders.Actions.Products
{
    public class ProductsWithPromoAction : ComputableAction<OrderDataContext>
    {
        public ProductsWithPromoAction(ComputableTree<OrderDataContext> tree, string actionName) : base(tree, actionName)
        {
        }

        public override Task<decimal> ExecuteActionAsync(OrderDataContext dataContext, IWorkflowExecutionContext executionContext)
        {
            var promoAmount = dataContext.PromoSkus.Where(p => p.Enabled).Sum(p => p.Amount*p.Quantity);
            foreach (var promo in dataContext.PromoSkus)
            {
                if (!(promo.Sku.SafeData.DisregardStock ?? true))
                {
                    if (promo.Sku.SafeData.Stock < promo.Quantity)
                    {
                        promo.Messages.Add(
                                new MessageInfo()
                                {
                                    MessageLevel = MessageLevel.Error,
                                    Message = "Сurrently out of stock. Please remove to continue."
                                });
                        promo.Enabled = false;
                    }
                }
            }
            dataContext.ProductsSubtotal = dataContext.Data.Products + promoAmount;
            return Task.FromResult(promoAmount);
        }
    }
}