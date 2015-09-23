using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VitalChoice.Domain.Entities.eCommerce.Products;
using VitalChoice.Workflow.Base;
using VitalChoice.Workflow.Contexts;

namespace VitalChoice.Business.Workflow.Actions.Products
{
    public class PerishableProductsAction : ComputableAction<OrderContext>
    {
        public PerishableProductsAction(ComputableTree<OrderContext> tree, string actionName) : base(tree, actionName)
        {
        }

        public override decimal ExecuteAction(OrderContext context)
        {
            return context.SkuOrdereds.Where(s => s.ProductWithoutSkus.IdObjectType == (int)ProductType.Perishable).Sum(s => s.Amount * s.Quantity);
        }
    }
}
