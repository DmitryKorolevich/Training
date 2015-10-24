using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VitalChoice.Workflow.Base;
using VitalChoice.Workflow.Contexts;
using VitalChoice.Workflow.Core;

namespace VitalChoice.Business.Workflow.Actions.Shipping
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
            dataContext.AlaskaHawaiiSurcharge = result;
            return Task.FromResult(result);
        }
    }
}