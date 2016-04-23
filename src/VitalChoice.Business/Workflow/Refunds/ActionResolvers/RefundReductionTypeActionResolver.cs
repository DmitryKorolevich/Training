using System.Threading.Tasks;
using VitalChoice.Business.Workflow.Orders.ActionResolvers;
using VitalChoice.Data.Repositories.Specifics;
using VitalChoice.Ecommerce.Domain.Entities.Orders;
using VitalChoice.Infrastructure.Domain.Transfer.Contexts;
using VitalChoice.Workflow.Base;
using VitalChoice.Workflow.Core;

namespace VitalChoice.Business.Workflow.Refunds.ActionResolvers
{
    public class RefundReductionTypeActionResolver : ComputableActionResolver<OrderRefundDataContext>
    {
        public RefundReductionTypeActionResolver(IWorkflowTree<OrderRefundDataContext, decimal> tree, string actionName)
            : base(tree, actionName)
        {

        }

        public override async Task<int> GetActionKeyAsync(OrderRefundDataContext dataContext, IWorkflowExecutionContext executionContext)
        {
            if (dataContext.Order.IdOrderSource != null)
            {
                var orderRep = executionContext.Resolve<IEcommerceRepositoryAsync<Order>>();
                if (
                    await
                        orderRep.Query(o => o.Id == dataContext.Order.IdOrderSource && o.IdObjectType == (int) OrderType.AutoShipOrder)
                            .SelectAnyAsync())
                {
                    return (int) ReductionType.AutoShip;
                }
            }
            return (int) ReductionType.Discount;
        }
    }
}