using System.Threading.Tasks;
using VitalChoice.Business.Queries.Product;
using VitalChoice.Ecommerce.Domain.Entities.Customers;
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
                    promoService.SelectAsync(queryObject:
                        new PromotionQuery().IsActive()
                            .WithExpiredType(ExpiredType.NotExpired)
                            .AllowCustomerType((CustomerType) context.Order.Customer.IdObjectType), withDefaults: true);

            return 0;
        }
    }
}