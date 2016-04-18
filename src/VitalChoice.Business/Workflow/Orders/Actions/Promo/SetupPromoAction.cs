using System.Threading.Tasks;
using VitalChoice.Ecommerce.Domain.Entities.Customers;
using VitalChoice.Infrastructure.Domain.Transfer.Contexts;
using VitalChoice.Interfaces.Services.Products;
using VitalChoice.Workflow.Base;
using VitalChoice.Workflow.Core;

namespace VitalChoice.Business.Workflow.Orders.Actions.Promo
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