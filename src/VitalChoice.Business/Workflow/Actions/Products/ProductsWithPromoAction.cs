using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VitalChoice.Workflow.Base;
using VitalChoice.Workflow.Contexts;

namespace VitalChoice.Business.Workflow.Actions.Products
{
    public class ProductsWithPromoAction : ComputableAction<OrderContext>
    {
        public ProductsWithPromoAction(ComputableTree<OrderContext> tree, string actionName) : base(tree, actionName)
        {
        }

        public override decimal ExecuteAction(OrderContext context)
        {
            var promoAmount = context.PromoSkus.Sum(p => p.Amount*p.Quantity);
            context.ProductsSubtotal = context.Data.Products + promoAmount;
            return promoAmount;
        }
    }
}
