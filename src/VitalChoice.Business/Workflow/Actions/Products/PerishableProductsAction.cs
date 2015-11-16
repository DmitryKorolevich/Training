using System.Linq;
using System.Threading.Tasks;
using VitalChoice.Ecommerce.Domain.Entities.Products;
using VitalChoice.Infrastructure.Domain.Transfer.Contexts;
using VitalChoice.Interfaces.Services.Settings;
using VitalChoice.Workflow.Base;
using VitalChoice.Workflow.Core;

namespace VitalChoice.Business.Workflow.Actions.Products
{
    public class PerishableProductsAction : ComputableAction<OrderDataContext>
    {
        public PerishableProductsAction(ComputableTree<OrderDataContext> tree, string actionName) : base(tree, actionName)
        {
        }

        public override async Task<decimal> ExecuteActionAsync(OrderDataContext dataContext, IWorkflowExecutionContext executionContext)
        {
            var perishableAmount = dataContext.SkuOrdereds.Where(s => s.ProductWithoutSkus.IdObjectType == (int) ProductType.Perishable)
                .Sum(s => s.Amount*s.Quantity);
            var settingsService = executionContext.Resolve<ISettingService>();
            var settings = await settingsService.GetAppSettingsAsync();
            if (perishableAmount < settings.GlobalPerishableThreshold)
            {
                dataContext.ProductsPerishableThresholdIssue = true;
            }
            return perishableAmount;
        }
    }
}
