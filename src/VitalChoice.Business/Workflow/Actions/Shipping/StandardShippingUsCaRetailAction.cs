using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VitalChoice.Workflow.Base;
using VitalChoice.Workflow.Contexts;

namespace VitalChoice.Business.Workflow.Actions.Shipping
{
    public class StandardShippingUsCaRetailAction : ComputableAction<OrderContext>
    {
        public StandardShippingUsCaRetailAction(ComputableTree<OrderContext> tree, string actionName) : base(tree, actionName)
        {
        }

        public override decimal ExecuteAction(OrderContext context)
        {
            if (context.Data.PromoProducts < 99 && context.Data.DeliveredAmount > 0)
            {
                if (context.Data.PromoProducts < 50)
                {
                    context.StandardShippingCharges = (decimal) 4.95;
                    return context.StandardShippingCharges;
                    //_deliveryServiceCostGroup = DeliveryServiceCostGroup.FirstCost;
                }
                if (context.Data.PromoProducts < 99)
                {
                    context.StandardShippingCharges = (decimal) 9.95;
                    return context.StandardShippingCharges;
                    //_deliveryServiceCostGroup = DeliveryServiceCostGroup.SecondCost;
                }
            }
            return 0;
        }
    }
}