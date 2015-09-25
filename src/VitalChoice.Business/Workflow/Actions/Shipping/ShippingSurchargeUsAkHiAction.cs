using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VitalChoice.Workflow.Base;
using VitalChoice.Workflow.Contexts;

namespace VitalChoice.Business.Workflow.Actions.Shipping
{
    public class ShippingSurchargeUsAkHiAction : ComputableAction<OrderContext>
    {
        public ShippingSurchargeUsAkHiAction(ComputableTree<OrderContext> tree, string actionName) : base(tree, actionName)
        {
        }

        public override decimal ExecuteAction(OrderContext context)
        {
            decimal result;
            if (context.Data.DeliveredAmount > 200)
            {
                result = context.Data.DeliveredAmount*(decimal) 0.1;
            }
            else
            {
                result = (decimal) 19.95;
            }
            context.AlaskaHawaiiSurcharge = result;
            return result;
        }
    }
}