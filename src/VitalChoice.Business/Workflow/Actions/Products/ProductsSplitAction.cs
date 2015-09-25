using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VitalChoice.Domain.Entities.eCommerce.Products;
using VitalChoice.Workflow.Base;
using VitalChoice.Workflow.Contexts;

namespace VitalChoice.Business.Workflow.Actions.Products
{
    public class ProductsSplitAction : ComputableAction<OrderContext>
    {
        public ProductsSplitAction(ComputableTree<OrderContext> tree, string actionName) : base(tree, actionName)
        {
        }

        public override decimal ExecuteAction(OrderContext context)
        {
            context.SplitInfo.PerishableCount =
                context.SkuOrdereds.Count(s => s.ProductWithoutSkus.IdObjectType == (int) ProductType.Perishable);
            context.SplitInfo.NonPerishableCount =
                context.SkuOrdereds.Count(s => s.ProductWithoutSkus.IdObjectType == (int) ProductType.NonPerishable);
            context.SplitInfo.NonPerishableOrphanCount =
                context.SkuOrdereds.Count(
                    s => s.ProductWithoutSkus.IdObjectType == (int) ProductType.NonPerishable && s.Sku.Data.OrphanType);
            //context.SplitInfo.ThresholdReached = context.SkuOrdereds.Any(
            //    s =>
            //        s.ProductWithoutSkus.IdObjectType == (int) ProductType.NonPerishable && s.Sku.Data.OrphanType &&
            //        s.Sku.Data.OrphanQtyThreshold >= s.Quantity);
            context.SplitInfo.SpecialSkuAdded = context.SkuOrdereds.Any(s => s.Sku.Code.ToLowerInvariant() == "emp");

            //TODO: move NonPerishableOrphanCount threshold to global admin config
            context.SplitInfo.ShouldSplit = (context.SplitInfo.PerishableCount > 0 &&
                                             context.SplitInfo.NonPerishableOrphanCount > 4
                                             ||
                                             context.SplitInfo.PerishableCount > 0 && context.SplitInfo.ThresholdReached
                                             ||
                                             context.SplitInfo.NonPerishableNonOrphanCount > 0 &&
                                             context.SplitInfo.PerishableCount > 0)
                                            && !context.SplitInfo.SpecialSkuAdded;

            return 0;
        }
    }
}
