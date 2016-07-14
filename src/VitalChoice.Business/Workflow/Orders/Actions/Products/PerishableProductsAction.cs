using System.Linq;
using System.Threading.Tasks;
using VitalChoice.Ecommerce.Domain.Entities.Products;
using VitalChoice.Ecommerce.Domain.Exceptions;
using VitalChoice.Infrastructure.Domain.Entities.Settings;
using VitalChoice.Infrastructure.Domain.Transfer.Contexts;
using VitalChoice.Interfaces.Services.Settings;
using VitalChoice.Workflow.Base;
using VitalChoice.Workflow.Core;

namespace VitalChoice.Business.Workflow.Orders.Actions.Products
{
    public class PerishableProductsAction : ComputableAction<OrderDataContext>
    {
        public PerishableProductsAction(ComputableTree<OrderDataContext> tree, string actionName) : base(tree, actionName)
        {
        }

        public override Task<decimal> ExecuteActionAsync(OrderDataContext dataContext, ITreeContext executionContext)
        {
            var perishableList =
                dataContext.SkuOrdereds.Where(s => s.Sku.IdObjectType == (int) ProductType.Perishable).ToArray();
            var perishableAmount = perishableList
                .Sum(s => s.Amount*s.Quantity);
            var perishableCount = perishableList.Length;
            var appSettings = executionContext.Resolve<AppSettings>();
            if (perishableCount > 0 && perishableAmount < appSettings.GlobalPerishableThreshold)
            {
                dataContext.ProductsPerishableThresholdIssue = true;
            }
            if (perishableCount == 1 && (bool) perishableList[0].Sku.Data.DisallowSingle && perishableList[0].Quantity == 1)
            {
                perishableList[0].Messages.Add(
                                new MessageInfo()
                                {
                                    MessageLevel = MessageLevel.Error,
                                    Message = "It is not possible to ship the single perishable item above. If you wish to order this item, please add another perishable item to your cart."
                                });
            }
            return Task.FromResult(perishableAmount);
        }
    }
}