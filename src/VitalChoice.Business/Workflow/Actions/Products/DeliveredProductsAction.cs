using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VitalChoice.Domain.Entities.eCommerce.Products;
using VitalChoice.Workflow.Base;
using VitalChoice.Workflow.Contexts;

namespace VitalChoice.Business.Workflow.Actions.Products
{
    public class DeliveredProductsAction: ComputableAction<OrderContext>
    {
        public DeliveredProductsAction(ComputableTree<OrderContext> tree, string actionName) : base(tree, actionName)
        {
        }

        public override decimal ExecuteAction(OrderContext context)
        {
            return
                context.SkuOrdereds.Union(context.PromoSkus).Where(s =>
                    s.ProductWithoutSkus.IdObjectType == (int) ProductType.Perishable ||
                    s.ProductWithoutSkus.IdObjectType == (int) ProductType.NonPerishable).Sum(s => s.Amount*s.Quantity);
        }
    }
}
