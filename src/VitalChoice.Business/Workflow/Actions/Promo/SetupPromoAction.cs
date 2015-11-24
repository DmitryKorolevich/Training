using System.Threading.Tasks;
using VitalChoice.Business.Queries.Product;
using VitalChoice.DynamicData.Interfaces;
using VitalChoice.Ecommerce.Domain.Entities.Customers;
using VitalChoice.Ecommerce.Domain.Entities.Promotion;
using VitalChoice.Infrastructure.Domain.Dynamic;
using VitalChoice.Infrastructure.Domain.Transfer;
using VitalChoice.Infrastructure.Domain.Transfer.Contexts;
using VitalChoice.Interfaces.Services.Products;
using VitalChoice.Workflow.Base;
using VitalChoice.Workflow.Core;

namespace VitalChoice.Business.Workflow.Actions.Promo
{
    public class SetupPromoAction : ComputableAction<OrderDataContext>
    {
        public SetupPromoAction(ComputableTree<OrderDataContext> tree, string actionName) : base(tree, actionName)
        {
        }

        public override async Task<decimal> ExecuteActionAsync(OrderDataContext context,
            IWorkflowExecutionContext executionContext)
        {
            var promoService = executionContext.Resolve<IPromotionService>();
            context.Promotions =
                await
                    promoService.GetActivePromotions((CustomerType) context.Order.Customer.IdObjectType);
            return 0;
        }
    }
}