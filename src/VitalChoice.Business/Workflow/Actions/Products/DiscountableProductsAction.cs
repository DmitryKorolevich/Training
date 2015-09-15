using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VitalChoice.Workflow.Base;
using VitalChoice.Workflow.Contexts;

namespace VitalChoice.Business.Workflow.Actions.Products
{
    public class DiscountableProductsAction : ComputableAction<OrderContext>
    {
        public DiscountableProductsAction(ComputableTree<OrderContext> tree, string actionName) : base(tree, actionName)
        {
        }

        public override decimal ExecuteAction(OrderContext context)
        {
            return context.Order.Skus.Where(s => !s.Sku.Data.NonDiscountable).Sum(s => s.Amount * s.Quantity);
        }
    }
}