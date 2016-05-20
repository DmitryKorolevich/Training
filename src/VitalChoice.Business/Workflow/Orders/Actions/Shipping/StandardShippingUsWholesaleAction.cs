using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Internal;
using VitalChoice.Infrastructure.Domain.Transfer.Contexts;
using VitalChoice.Workflow.Base;
using VitalChoice.Workflow.Core;

namespace VitalChoice.Business.Workflow.Orders.Actions.Shipping
{
    public class StandardShippingUsWholesaleAction : ComputableAction<OrderDataContext>
    {
        public StandardShippingUsWholesaleAction(ComputableTree<OrderDataContext> tree, string actionName) : base(tree, actionName)
        {
        }

        public override Task<decimal> ExecuteActionAsync(OrderDataContext dataContext, IWorkflowExecutionContext executionContext)
        {
            if (dataContext.Data.PromoProducts < 200 && dataContext.Data.DeliveredAmount > 0)
            {
                if (dataContext.Data.PromoProducts < 50)
                {
                    dataContext.StandardShippingCharges = (decimal) 4.95;
                    return Task.FromResult(dataContext.StandardShippingCharges);
                    //_deliveryServiceCostGroup = DeliveryServiceCostGroup.FirstCost;
                }
                if (dataContext.Data.PromoProducts < 100)
                {
                    dataContext.StandardShippingCharges = (decimal) 9.95;
                    return Task.FromResult(dataContext.StandardShippingCharges);
                    //_deliveryServiceCostGroup = DeliveryServiceCostGroup.SecondCost;
                }
                if (dataContext.Data.PromoProducts < 150)
                {
                    dataContext.StandardShippingCharges = (decimal)14.95;
                    return Task.FromResult(dataContext.StandardShippingCharges);
                    //_deliveryServiceCostGroup = DeliveryServiceCostGroup.SecondCost;
                }
                if (dataContext.Data.PromoProducts < 200)
                {
                    dataContext.StandardShippingCharges = (decimal)19.95;
                    return Task.FromResult(dataContext.StandardShippingCharges);
                    //_deliveryServiceCostGroup = DeliveryServiceCostGroup.SecondCost;
                }
            }
            return TaskCache<decimal>.DefaultCompletedTask;
        }
    }
}