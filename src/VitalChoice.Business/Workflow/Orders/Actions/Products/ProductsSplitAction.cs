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

        public override Task<decimal> ExecuteActionAsync(OrderDataContext dataContext, ITreeContext executionContext)
        {
            var products = dataContext.SkuOrdereds.Union(dataContext.PromoSkus.Where(p => p.Enabled)).ToArray();
            var perishableProducts =
                products.Where(s => s.Sku.IdObjectType == (int) ProductType.Perishable).ToArray();
            var nonPerishableProducts =
                products.Where(s => s.Sku.IdObjectType == (int) ProductType.NonPerishable).ToArray();
            dataContext.SplitInfo.PerishableCount = perishableProducts.Length;
            dataContext.SplitInfo.NonPerishableCount = nonPerishableProducts.Length;
            dataContext.SplitInfo.PerishableAmount = perishableProducts.Sum(p => p.Amount*p.Quantity);
            dataContext.SplitInfo.NonPerishableAmount = nonPerishableProducts.Sum(p => p.Amount*p.Quantity);
            dataContext.SplitInfo.NonPerishableOrphanCount =
                products.Count(
                    s => s.Sku.IdObjectType == (int) ProductType.NonPerishable && s.Sku.Data.OrphanType);
            dataContext.SplitInfo.ThresholdReached =
                products.Any(
                    s =>
                        s.Sku.IdObjectType == (int) ProductType.NonPerishable && s.Sku.Data.OrphanType &&
                        s.Quantity > s.Sku.Data.QTYThreshold);
            dataContext.SplitInfo.SpecialSkuAdded = products.Any(s => s.Sku.Code.ToLowerInvariant() == "emp");

            //TODO: move NonPerishableOrphanCount threshold to global admin config
            dataContext.SplitInfo.ShouldSplit = (dataContext.SplitInfo.PerishableCount > 0 &&
                                             dataContext.SplitInfo.NonPerishableOrphanCount > 4
                                             ||
                                             dataContext.SplitInfo.PerishableCount > 0 && dataContext.SplitInfo.ThresholdReached
                                             ||
                                             dataContext.SplitInfo.NonPerishableNonOrphanCount > 0 &&
                                             dataContext.SplitInfo.PerishableCount > 0)
                                            && !dataContext.SplitInfo.SpecialSkuAdded;
            if (dataContext.SplitInfo.ShouldSplit)
                dataContext.SplitInfo.ProductTypes = POrderType.PNP;
            else
            {
                if (dataContext.SplitInfo.PerishableCount > 0)
                    dataContext.SplitInfo.ProductTypes = POrderType.P;
                else if (dataContext.SplitInfo.NonPerishableCount > 0)
                    dataContext.SplitInfo.ProductTypes = POrderType.NP;
                else
                    dataContext.SplitInfo.ProductTypes = POrderType.Other;
            }
            return TaskCache<decimal>.DefaultCompletedTask;
        }
    }
}
