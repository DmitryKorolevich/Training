using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VitalChoice.Workflow.Base;
using VitalChoice.Workflow.Contexts;

namespace VitalChoice.Business.Workflow.Actions.Shipping
{
    public class ShippingSurchargeCaAction : ComputableAction<OrderContext>
    {
        public ShippingSurchargeCaAction(ComputableTree<OrderContext> tree, string actionName) : base(tree, actionName)
        {
        }

        public override decimal ExecuteAction(OrderContext context)
        {
            context.CanadaSurcharge = 0;
            return 0;
        }
    }
}
