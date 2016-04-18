using System.Threading.Tasks;
using VitalChoice.Infrastructure.Domain.Transfer.Contexts;
using VitalChoice.Workflow.Base;
using VitalChoice.Workflow.Core;

namespace VitalChoice.Business.Workflow.Orders.Actions.Shipping
{
    public class StandardShippingUsCaRetailAction : ComputableAction<OrderDataContext>
    {
        public StandardShippingUsCaRetailAction(ComputableTree<OrderDataContext> tree, string actionName) : base(tree, actionName)
        {
        }

        public override Task<decimal> ExecuteActionAsync(OrderDataContext dataContext, IWorkflowExecutionContext executionContext)
        {
            if (dataContext.Data.PromoProducts < 99 && dataContext.Data.DeliveredAmount > 0)
            {
                if (dataContext.Data.PromoProducts < 99)
                {
                    dataContext.FreeShippingThresholdDifference = 99 - dataContext.Data.PromoProducts;
                }
                if (dataContext.Data.PromoProducts < 50)
                {
                    dataContext.StandardShippingCharges = (decimal) 4.95;
                    return Task.FromResult(dataContext.StandardShippingCharges);
                    //_deliveryServiceCostGroup = DeliveryServiceCostGroup.FirstCost;
                }
                if (dataContext.Data.PromoProducts < 99)
                {
                    dataContext.StandardShippingCharges = (decimal) 9.95;
                    return Task.FromResult(dataContext.StandardShippingCharges);
                    //_deliveryServiceCostGroup = DeliveryServiceCostGroup.SecondCost;
                }
            }
            return Task.FromResult<decimal>(0);
        }
    }
}