using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VitalChoice.Ecommerce.Domain.Entities.Promotion;
using VitalChoice.Infrastructure.Domain.Dynamic;
using VitalChoice.Infrastructure.Domain.Transfer.Contexts;
using VitalChoice.Infrastructure.Domain.Transfer.Orders;
using VitalChoice.Infrastructure.Domain.Transfer.Products;
using VitalChoice.Interfaces.Services.Products;
using VitalChoice.Workflow.Base;
using VitalChoice.Workflow.Core;

namespace VitalChoice.Business.Workflow.Actions.Promo
{
    public class BuyXGetYPromoAction : ComputableAction<OrderDataContext>
    {
        public BuyXGetYPromoAction(ComputableTree<OrderDataContext> tree, string actionName) : base(tree, actionName)
        {
        }

        public override async Task<decimal> ExecuteActionAsync(OrderDataContext context,
            IWorkflowExecutionContext executionContext)
        {
            var eligiable = context.Promotions.Where(p => p.IdObjectType == (int) PromotionType.BuyXGetY);
            foreach (var promo in eligiable)
            {
                if (promo.Data.PromotionBuyType == (int)PromoBuyType.Any)
                {
                    Dictionary<int, PromotionToBuySku> promoskuIds = promo.PromotionsToBuySkus.ToDictionary(s => s.IdSku);
                    var applyCount =
                        context.Order.Skus.Where(s => promoskuIds.ContainsKey(s.Sku.Id))
                            .Sum(s => s.Quantity/promoskuIds[s.Sku.Id].Quantity);
                    await AddPromoProducts(context, executionContext, promo, applyCount);
                }
                else if (promo.Data.PromotionBuyType == (int)PromoBuyType.All)
                {
                    Dictionary<int, SkuOrdered> orderSkuIds = context.Order.Skus.ToDictionary(s => s.Sku.Id);
                    if (promo.PromotionsToBuySkus.All(s => orderSkuIds.ContainsKey(s.IdSku)))
                    {
                        var applyCount = promo.PromotionsToBuySkus.Min(s => orderSkuIds[s.IdSku].Quantity / s.Quantity);
                        await AddPromoProducts(context, executionContext, promo, applyCount);
                    }
                }
            }
            return 0;
        }

        private static async Task AddPromoProducts(OrderDataContext context, IWorkflowExecutionContext executionContext,
            PromotionDynamic promo, int applyCount)
        {
            int? maxUsage = promo.Data.MaxTimesUse;

            applyCount = Math.Min(applyCount, maxUsage ?? applyCount);
            if (applyCount > 0)
            {
                var skuListCache = promo.PromotionsToGetSkus.ToDictionary(s => s.IdSku);
                var productService = executionContext.Resolve<IProductService>();
                var promoSkus = await
                    productService.GetSkusOrderedAsync(
                        promo.PromotionsToGetSkus.Select(p => p.IdSku).ToArray());
                foreach (var sku in promoSkus)
                {
                    var promoGetSku = skuListCache[sku.Sku.Id];
                    sku.Amount = sku.Sku.Price - sku.Sku.Price*promoGetSku.Percent/100;
                    sku.Quantity = promoGetSku.Quantity*applyCount;
                    context.PromoSkus.Add(sku);
                }
            }
        }
    }
}
