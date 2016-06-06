using System.Threading.Tasks;
using VitalChoice.Ecommerce.Domain.Entities.Orders;
using VitalChoice.Infrastructure.Domain.Transfer.Contexts;
using VitalChoice.Workflow.Base;
using VitalChoice.Workflow.Core;

namespace VitalChoice.Business.Workflow.Orders.ActionResolvers
{
    public enum ReductionType
    {
        AutoShip = 1,
        Discount = 2
    }

    public class ReductionTypeActionResolver : ComputableActionResolver<OrderDataContext>
    {
        public ReductionTypeActionResolver(IWorkflowTree<OrderDataContext, decimal> tree, string actionName) : base(tree, actionName)
        {

        }

        public override Task<int> GetActionKeyAsync(OrderDataContext dataContext, ITreeContext executionContext)
        {
            if (dataContext.Order.IdObjectType == (int) OrderType.AutoShip ||
                dataContext.Order.IdObjectType == (int) OrderType.AutoShipOrder)
                return Task.FromResult((int) ReductionType.AutoShip);
            return Task.FromResult((int) ReductionType.Discount);
        }
    }
}