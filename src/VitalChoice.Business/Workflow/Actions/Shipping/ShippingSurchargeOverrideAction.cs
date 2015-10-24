using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VitalChoice.Workflow.Base;
using VitalChoice.Workflow.Contexts;
using VitalChoice.Workflow.Core;

namespace VitalChoice.Business.Workflow.Actions.Shipping
{
    public class ShippingSurchargeOverrideAction : ComputableAction<OrderDataContext>
    {
        public ShippingSurchargeOverrideAction(ComputableTree<OrderDataContext> tree, string actionName) : base(tree, actionName)
        {
        }

        public override Task<decimal> ExecuteActionAsync(OrderDataContext dataContext, IWorkflowExecutionContext executionContext)
        {
            decimal surchargeOverride = dataContext.Order.Data.SurchargeOverride;
            decimal surchargeTotal = dataContext.AlaskaHawaiiSurcharge + dataContext.CanadaSurcharge;
            if (surchargeOverride > surchargeTotal)
            {
                surchargeOverride = surchargeTotal;
            }
            dataContext.SurchargeOverride = surchargeOverride;
            return Task.FromResult(-surchargeOverride);
        }
    }
}
