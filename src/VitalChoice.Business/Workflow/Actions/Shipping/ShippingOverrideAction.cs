using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VitalChoice.Workflow.Base;
using VitalChoice.Workflow.Contexts;

namespace VitalChoice.Business.Workflow.Actions.Shipping
{
    public class ShippingOverrideAction : ComputableAction<OrderContext>
    {
        public ShippingOverrideAction(ComputableTree<OrderContext> tree, string actionName) : base(tree, actionName)
        {
        }

        public override decimal ExecuteAction(OrderContext context)
        {
            decimal shippingOverride = context.Order.Data.ShippingOverride;
            decimal shippingTotal = context.StandardShippingCharges + context.Data.ShippingUpgrade;
            if (shippingOverride > shippingTotal)
            {
                shippingOverride = shippingTotal;
            }
            context.ShippingOverride = shippingOverride;
            return -shippingOverride;
        }
    }
}
