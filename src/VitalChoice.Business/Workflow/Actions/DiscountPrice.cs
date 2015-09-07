using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VitalChoice.Workflow.Base;
using VitalChoice.Workflow.Contexts;

namespace VitalChoice.Business.Workflow.Actions
{
    public class DiscountPrice : ComputableAction<OrderContext>
    {
        public DiscountPrice(ComputableTree<OrderContext> tree, string actionName) : base(tree, actionName)
        {
        }

        public override decimal ExecuteAction(OrderContext context)
        {
            return -Math.Min(Tree.GetActionResult("Products", context), context.Order.Discount.Data.Amount);
        }
    }
}
