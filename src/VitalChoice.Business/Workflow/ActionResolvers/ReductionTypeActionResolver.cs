using System.Linq;
using System.Threading.Tasks;
using VitalChoice.Ecommerce.Domain.Entities.Orders;
using VitalChoice.Infrastructure.Domain.Transfer.Contexts;
using VitalChoice.Workflow.Base;
using VitalChoice.Workflow.Core;

namespace VitalChoice.Business.Workflow.ActionResolvers
{
	public enum ReductionType
	{
		AutoShip = 1,
		Discount = 2,
		HealthWise = 3//todo: handle it
	}

	public class ReductionTypeActionResolver : ComputableActionResolver<OrderDataContext>
    {
        public ReductionTypeActionResolver(IWorkflowTree<OrderDataContext, decimal> tree, string actionName) : base(tree, actionName)
        {

        }

        public override Task<int> GetActionKeyAsync(OrderDataContext dataContext, IWorkflowExecutionContext executionContext)
        {
            return Task.FromResult((dataContext.Order.IdObjectType == (int)OrderType.AutoShip || dataContext.Order.IdObjectType == (int)OrderType.AutoShipOrder) ? (int)ReductionType.AutoShip : (int)ReductionType.Discount);
        }
    }
}