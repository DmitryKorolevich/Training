using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Internal;
using VitalChoice.Ecommerce.Domain.Entities.Products;
using VitalChoice.Infrastructure.Domain.Transfer.Contexts;
using VitalChoice.Infrastructure.Domain.Transfer.Orders;
using VitalChoice.Workflow.Base;
using VitalChoice.Workflow.Core;

namespace VitalChoice.Business.Workflow.Orders.Actions.Products
{
    public class ProductsSplitAction : ComputableAction<OrderDataContext>
    {
        public ProductsSplitAction(ComputableTree<OrderDataContext> tree, string actionName) : base(tree, actionName)
        {
        }

        public override Task<decimal> ExecuteActionAsync(OrderDataContext context, ITreeContext executionContext)
        {
            var products = context.SkuOrdereds.Union(context.PromoSkus.Where(p => p.Enabled)).ToArray();
            var perishableProducts =
                products.Where(s => s.Sku.IdObjectType == (int) ProductType.Perishable).ToArray();
            var nonPerishableProducts =
                products.Where(s => s.Sku.IdObjectType == (int) ProductType.NonPerishable).ToArray();
            context.ProductSplitInfo.PerishableCount = perishableProducts.Length;
            context.ProductSplitInfo.NonPerishableCount = nonPerishableProducts.Length;
            context.ProductSplitInfo.PerishableAmount = perishableProducts.Sum(p => p.Amount*p.Quantity);
            context.ProductSplitInfo.NonPerishableAmount = nonPerishableProducts.Sum(p => p.Amount*p.Quantity);
            context.ProductSplitInfo.NonPerishableOrphanCount =
                products.Count(
                    s => s.Sku.IdObjectType == (int) ProductType.NonPerishable && s.Sku.Data.OrphanType);
            context.ProductSplitInfo.ThresholdReached =
                products.Any(
                    s =>
                        s.Sku.IdObjectType == (int) ProductType.NonPerishable && s.Sku.Data.OrphanType &&
                        s.Quantity > s.Sku.Data.QTYThreshold);
            context.ProductSplitInfo.SpecialSkuAdded = products.Any(s => s.Sku.Code.ToLowerInvariant() == "emp");

            //TODO: move NonPerishableOrphanCount threshold to global admin config
            context.ProductSplitInfo.ShouldSplit = (context.ProductSplitInfo.PerishableCount > 0 &&
                                             context.ProductSplitInfo.NonPerishableOrphanCount > 4
                                             ||
                                             context.ProductSplitInfo.PerishableCount > 0 && context.ProductSplitInfo.ThresholdReached
                                             ||
                                             context.ProductSplitInfo.NonPerishableNonOrphanCount > 0 &&
                                             context.ProductSplitInfo.PerishableCount > 0)
                                            && !context.ProductSplitInfo.SpecialSkuAdded;
            if (context.ProductSplitInfo.ShouldSplit)
                context.ProductSplitInfo.ProductTypes = POrderType.PNP;
            else
            {
                if (context.ProductSplitInfo.PerishableCount > 0)
                    context.ProductSplitInfo.ProductTypes = POrderType.P;
                else if (context.ProductSplitInfo.NonPerishableCount > 0)
                    context.ProductSplitInfo.ProductTypes = POrderType.NP;
                else
                    context.ProductSplitInfo.ProductTypes = POrderType.Other;
            }
            context.ProductSplitInfo.OtherProductsAmount =
                products.Where(
                    s => s.Sku.IdObjectType != (int) ProductType.Perishable && s.Sku.IdObjectType != (int) ProductType.NonPerishable)
                    .Sum(p => p.Amount*p.Quantity);
            return TaskCache<decimal>.DefaultCompletedTask;
        }
    }
}
