using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VitalChoice.Domain.Entities.eCommerce.Orders;
using VitalChoice.Workflow.Base;
using VitalChoice.Workflow.Contexts;

namespace VitalChoice.Business.Workflow.Actions.Discounts
{
    public class DiscountFreeShippingAction : ComputableAction<OrderContext>
    {
        public DiscountFreeShippingAction(ComputableTree<OrderContext> tree, string actionName) : base(tree, actionName)
        {
        }

        public override decimal ExecuteAction(OrderContext context)
        {
            context.DiscountMessage = "Free Shipping Discount";
            context.FreeShipping = true;
            return 0;
        }
    }
}