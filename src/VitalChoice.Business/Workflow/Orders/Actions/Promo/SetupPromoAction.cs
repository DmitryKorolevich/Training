using System.Collections.Generic;
using System.Threading.Tasks;
using VitalChoice.Ecommerce.Domain.Entities.Customers;
using VitalChoice.Ecommerce.Domain.Entities.Orders;
using VitalChoice.Infrastructure.Domain.Dynamic;
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
            ITreeContext executionContext)
        {
            if (context.Order.IdObjectType == (int) OrderType.AutoShip || context.Order.IdObjectType == (int) OrderType.AutoShipOrder)
            {
                context.Promotions = new List<PromotionDynamic>();
            }
            else
            {
                var promoService = executionContext.Resolve<IPromotionService>();
                context.Promotions =
                    await
                        promoService.GetActivePromotions((CustomerType?) context.Order.Customer?.IdObjectType ?? CustomerType.Retail);
            }
            return 0;
        }
    }
}