using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VitalChoice.Workflow.Base;
using VitalChoice.Workflow.Contexts;
using VitalChoice.Workflow.Core;

namespace VitalChoice.Business.Workflow.Actions.Shipping
{
    public class ShippingOverrideAction : ComputableAction<OrderDataContext>
    {
        public ShippingOverrideAction(ComputableTree<OrderDataContext> tree, string actionName) : base(tree, actionName)
        {
        }

        public override Task<decimal> ExecuteAction(OrderDataContext dataContext, IWorkflowExecutionContext executionContext)
        {
            decimal shippingOverride = dataContext.Order.Data.ShippingOverride;
            decimal shippingTotal = dataContext.StandardShippingCharges + dataContext.Data.ShippingUpgrade;
            if (shippingOverride > shippingTotal)
            {
                shippingOverride = shippingTotal;
            }
            dataContext.ShippingOverride = shippingOverride;
            return Task.FromResult(-shippingOverride);
        }
    }
}
