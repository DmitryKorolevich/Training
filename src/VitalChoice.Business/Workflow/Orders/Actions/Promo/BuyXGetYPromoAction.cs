using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VitalChoice.Ecommerce.Domain.Entities.Promotions;
using VitalChoice.Infrastructure.Domain.Dynamic;
using VitalChoice.Infrastructure.Domain.Transfer.Contexts;
using VitalChoice.Infrastructure.Domain.Transfer.Orders;
using VitalChoice.Infrastructure.Domain.Transfer.Products;
using VitalChoice.Interfaces.Services.Products;
using VitalChoice.Workflow.Base;
using VitalChoice.Workflow.Core;

namespace VitalChoice.Business.Workflow.Orders.Actions.Promo
{
    public class BuyXGetYPromoAction : ComputableAction<OrderDataContext>
    {
        public BuyXGetYPromoAction(ComputableTree<OrderDataContext> tree, string actionName) : base(tree, actionName)
        {
        }

        public override async Task<decimal> ExecuteActionAsync(OrderDataContext context,
            IWorkflowExecutionContext executionContext)
        {
            IEnumerable<PromotionDynamic> eligiable = context.Promotions.Where(p => p.IdObjectType == (int)PromotionType.BuyXGetY);
            if (context.Order.Discount != null && context.Order.Discount.Id != 0)
            {
                eligiable = eligiable.Where(p => (bool)p.Data.CanUseWithDiscount);
            }
            if ((bool?)context.Order.SafeData.IsHealthwise ?? false)
            {
                eligiable = context.Promotions.Where(p => (bool)p.Data.AllowHealthwise);
            }
            var productService = executionContext.Resolve<IProductService>();
            foreach (var promo in eligiable)
            {
                if (promo.Data.PromotionBuyType == (int) PromoBuyType.Any)
                {
                    Dictionary<int, PromotionToBuySku> promoskuIds = promo.PromotionsToBuySkus.ToDictionary(s => s.IdSku);
                    var applyCount =
                        context.Order.Skus.Where(s => promoskuIds.ContainsKey(s.Sku.Id))
                            .Sum(s => s.Quantity/promoskuIds[s.Sku.Id].Quantity);
                    await AddPromoProducts(context, productService, promo, applyCount);
                }
                else if (promo.Data.PromotionBuyType == (int) PromoBuyType.All)
                {
                    Dictionary<int, SkuOrdered> orderSkuIds = context.Order.Skus.ToDictionary(s => s.Sku.Id);
                    if (promo.PromotionsToBuySkus.All(s => orderSkuIds.ContainsKey(s.IdSku)))
                    {
                        var applyCount = promo.PromotionsToBuySkus.Min(s => orderSkuIds[s.IdSku].Quantity/s.Quantity);
                        await AddPromoProducts(context, productService, promo, applyCount);
                    }
                }
            }
            return 0;
        }

        private static async Task AddPromoProducts(OrderDataContext context, IProductService productService,
            PromotionDynamic promo, int applyCount)
        {
            int? maxUsage = promo.SafeData.MaxTimesUse;

            applyCount = Math.Min(applyCount, maxUsage ?? applyCount);
            if (applyCount > 0)
            {
                var skuListCache = promo.PromotionsToGetSkus.ToDictionary(s => s.IdSku);
                var promoSkus = await
                    productService.GetSkusOrderedAsync(
                        promo.PromotionsToGetSkus.Select(p => p.IdSku).ToArray());
                foreach (var sku in promoSkus)
                {
                    var promoGetSku = skuListCache[sku.Sku.Id];
                    sku.Amount = sku.Sku.Price - Math.Round(sku.Sku.Price*promoGetSku.Percent/100, 2);
                    sku.Quantity = promoGetSku.Quantity*applyCount;
                    context.PromoSkus.Add(new PromoOrdered(sku, promo, GetPromoEnabled(context.Order.PromoSkus, promo.Id, sku.Sku.Id)));
                }
            }
        }

        private static bool GetPromoEnabled(ICollection<PromoOrdered> promos, int id, int idSku)
        {
            var inOrder = promos?.FirstOrDefault(p => p.Promotion?.Id == id && p.Sku?.Id == idSku);
            return inOrder?.Enabled ?? true;
        }
    }
}