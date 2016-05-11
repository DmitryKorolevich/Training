using System;
using System.Threading.Tasks;
using VitalChoice.Infrastructure.Domain.Transfer.Contexts;
using VitalChoice.Workflow.Base;
using VitalChoice.Workflow.Core;

namespace VitalChoice.Business.Workflow.Orders.Actions.Shipping
{
    public class ShippingSurchargeUsAkHiAction : ComputableAction<OrderDataContext>
    {
        public ShippingSurchargeUsAkHiAction(ComputableTree<OrderDataContext> tree, string actionName) : base(tree, actionName)
        {
        }

        public override Task<decimal> ExecuteActionAsync(OrderDataContext dataContext, IWorkflowExecutionContext executionContext)
        {
            decimal result;
            if (dataContext.Data.DeliveredAmount > 200)
            {
                result = dataContext.Data.DeliveredAmount*(decimal) 0.1;
            }
            else
            {
                result = (decimal) 19.95;
            }
            dataContext.AlaskaHawaiiSurcharge = Math.Round(result, 2);
            return Task.FromResult(result);
        }
    }
}