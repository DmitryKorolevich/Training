using System.Threading.Tasks;
using VitalChoice.Ecommerce.Domain.Entities.Customers;
using VitalChoice.Ecommerce.Domain.Entities.Orders;
using VitalChoice.Infrastructure.Domain.Transfer.Contexts;
using VitalChoice.Workflow.Base;
using VitalChoice.Workflow.Core;

namespace VitalChoice.Business.Workflow.Orders.ActionResolvers
{
	public enum ReductionType
	{
		AutoShip = 1,
		Discount = 2,
		HealthWise = 3
	}

	public class ReductionTypeActionResolver : ComputableActionResolver<OrderDataContext>
    {
        public ReductionTypeActionResolver(IWorkflowTree<OrderDataContext, decimal> tree, string actionName) : base(tree, actionName)
        {

        }

        public override Task<int> GetActionKeyAsync(OrderDataContext dataContext, IWorkflowExecutionContext executionContext)
        {
            if (dataContext.Order.IdObjectType == (int) OrderType.AutoShip ||
                dataContext.Order.IdObjectType == (int) OrderType.AutoShipOrder)
                return Task.FromResult((int) ReductionType.AutoShip);
            if (dataContext.Order.Customer.IdObjectType == (int) CustomerType.Retail)
            {
                if (((bool?)dataContext.Order.SafeData.IsHealthwise ?? false) && !((bool?)dataContext.Order.Customer.SafeData.HasHealthwiseOrders ?? false))
                {
                    dataContext.Order.IsFirstHealthwise = true;
                    return Task.FromResult((int) ReductionType.HealthWise);
                }
            }
            else
            {
                dataContext.Order.Data.IsHealthwise = false;
            }
            return Task.FromResult((int)ReductionType.Discount);
        }
    }
}