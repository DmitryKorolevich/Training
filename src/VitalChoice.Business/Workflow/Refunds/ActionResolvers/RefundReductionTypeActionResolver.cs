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

        public override Task<int> GetActionKeyAsync(OrderRefundDataContext dataContext, IWorkflowExecutionContext executionContext)
        {
            if (dataContext.Order.OriginalOrder.IdObjectType == (int) OrderType.AutoShipOrder)
            {
                return Task.FromResult((int) ReductionType.AutoShip);
            }
            return Task.FromResult((int) ReductionType.Discount);
        }
    }
}