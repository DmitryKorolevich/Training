using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VitalChoice.Workflow.Base;
using VitalChoice.Workflow.Contexts;

namespace VitalChoice.Business.Workflow.Actions.Shipping
{
    public class ShippingSurchargeOverrideAction : ComputableAction<OrderContext>
    {
        public ShippingSurchargeOverrideAction(ComputableTree<OrderContext> tree, string actionName) : base(tree, actionName)
        {
        }

        public override decimal ExecuteAction(OrderContext context)
        {
            decimal surchargeOverride = context.Order.Data.SurchargeOverride;
            decimal surchargeTotal = context.AlaskaHawaiiSurcharge + context.CanadaSurcharge;
            if (surchargeOverride > surchargeTotal)
            {
                surchargeOverride = surchargeTotal;
            }
            context.SurchargeOverride = surchargeOverride;
            return -surchargeOverride;
        }
    }
}
