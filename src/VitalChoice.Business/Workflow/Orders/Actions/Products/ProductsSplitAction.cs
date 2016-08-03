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
            var gcsProducts = products.Where(p => p.Sku.IdObjectType == (int) ProductType.Gc).ToArray();
            context.SplitInfo.PerishableCount = perishableProducts.Length;
            context.SplitInfo.NonPerishableCount = nonPerishableProducts.Length;
            context.SplitInfo.PerishableAmount = perishableProducts.Sum(p => p.Amount*p.Quantity);
            context.SplitInfo.NonPerishableAmount = nonPerishableProducts.Sum(p => p.Amount*p.Quantity);
            context.SplitInfo.NonPerishableOrphanCount =
                products.Count(
                    s => s.Sku.IdObjectType == (int) ProductType.NonPerishable && s.Sku.Data.OrphanType);
            context.SplitInfo.ThresholdReached =
                products.Any(
                    s =>
                        s.Sku.IdObjectType == (int) ProductType.NonPerishable && s.Sku.Data.OrphanType &&
                        s.Quantity > s.Sku.Data.QTYThreshold);
            context.SplitInfo.SpecialSkuAdded = products.Any(s => s.Sku.Code.ToLowerInvariant() == "emp");

            //TODO: move NonPerishableOrphanCount threshold to global admin config
            context.SplitInfo.ShouldSplit = (context.SplitInfo.PerishableCount > 0 &&
                                             context.SplitInfo.NonPerishableOrphanCount > 4
                                             ||
                                             context.SplitInfo.PerishableCount > 0 && context.SplitInfo.ThresholdReached
                                             ||
                                             context.SplitInfo.NonPerishableNonOrphanCount + gcsProducts.Length > 0 &&
                                             context.SplitInfo.PerishableCount > 0)
                                            && !context.SplitInfo.SpecialSkuAdded;
            if (context.SplitInfo.ShouldSplit)
                context.SplitInfo.ProductTypes = POrderType.PNP;
            else
            {
                if (context.SplitInfo.PerishableCount > 0)
                    context.SplitInfo.ProductTypes = POrderType.P;
                else if (context.SplitInfo.NonPerishableCount + gcsProducts.Length > 0)
                    context.SplitInfo.ProductTypes = POrderType.NP;
                else
                    context.SplitInfo.ProductTypes = POrderType.Other;
            }
            context.SplitInfo.OtherProductsAmount =
                products.Where(
                    s => s.Sku.IdObjectType != (int) ProductType.Perishable && s.Sku.IdObjectType != (int) ProductType.NonPerishable)
                    .Sum(p => p.Amount*p.Quantity);
            return TaskCache<decimal>.DefaultCompletedTask;
        }
    }
}