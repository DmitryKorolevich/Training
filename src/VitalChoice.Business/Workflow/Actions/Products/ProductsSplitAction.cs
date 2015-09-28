using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VitalChoice.Domain.Entities.eCommerce.Orders;
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
            var products = context.SkuOrdereds.Union(context.PromoSkus).ToArray();
            var perishableProducts =
                products.Where(s => s.ProductWithoutSkus.IdObjectType == (int) ProductType.Perishable).ToArray();
            var nonPerishableProducts =
                products.Where(s => s.ProductWithoutSkus.IdObjectType == (int) ProductType.NonPerishable).ToArray();
            context.SplitInfo.PerishableCount = perishableProducts.Length;
            context.SplitInfo.NonPerishableCount = nonPerishableProducts.Length;
            context.SplitInfo.PerishableAmount = perishableProducts.Sum(p => p.Amount*p.Quantity);
            context.SplitInfo.NonPerishableAmount = perishableProducts.Sum(p => p.Amount*p.Quantity);
            context.SplitInfo.NonPerishableOrphanCount =
                products.Count(
                    s => s.ProductWithoutSkus.IdObjectType == (int) ProductType.NonPerishable && s.Sku.Data.OrphanType);
            context.SplitInfo.ThresholdReached =
                products.Any(
                    s =>
                        s.ProductWithoutSkus.IdObjectType == (int) ProductType.NonPerishable && s.Sku.Data.OrphanType &&
                        s.Sku.Data.QTYThreshold >= s.Quantity);
            context.SplitInfo.SpecialSkuAdded = products.Any(s => s.Sku.Code.ToLowerInvariant() == "emp");

            //TODO: move NonPerishableOrphanCount threshold to global admin config
            context.SplitInfo.ShouldSplit = (context.SplitInfo.PerishableCount > 0 &&
                                             context.SplitInfo.NonPerishableOrphanCount > 4
                                             ||
                                             context.SplitInfo.PerishableCount > 0 && context.SplitInfo.ThresholdReached
                                             ||
                                             context.SplitInfo.NonPerishableNonOrphanCount > 0 &&
                                             context.SplitInfo.PerishableCount > 0)
                                            && !context.SplitInfo.SpecialSkuAdded;
            if (context.SplitInfo.ShouldSplit)
                context.SplitInfo.ProductTypes = POrderType.PNP;
            else
            {
                if (context.SplitInfo.PerishableCount > 0)
                    context.SplitInfo.ProductTypes = POrderType.P;
                else if (context.SplitInfo.NonPerishableCount > 0)
                    context.SplitInfo.ProductTypes = POrderType.NP;
                else
                    context.SplitInfo.ProductTypes = POrderType.Other;
            }
            return 0;
        }
    }
}
