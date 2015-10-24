using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VitalChoice.Domain.Entities.eCommerce.Promotions;
using VitalChoice.Domain.Helpers;
using VitalChoice.Interfaces.Services.Products;
using VitalChoice.Workflow.Base;
using VitalChoice.Workflow.Contexts;
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
                HashSet<int> promoskuIds = new HashSet<int>(promo.PromotionsToBuySkus.Select(s => s.IdSku));
                var applyCount = context.Order.Skus.Count(s => promoskuIds.Contains(s.Sku.Id));
                int? maxUsage = promo.Data.MaxTimesUse;
                applyCount = Math.Min(applyCount, maxUsage ?? applyCount);
                if (applyCount > 0)
                {
                    var skuListCache = promo.PromotionsToGetSkus.ToDictionary(s => s.IdSku);
                    var productService = executionContext.Resolve<IProductService>();
                    var promoSkus = await
                        productService.GetSkusOrderedAsync(promo.PromotionsToGetSkus.Select(p => p.IdSku).ToArray());
                    foreach (var sku in promoSkus)
                    {
                        var promoGetSku = skuListCache[sku.Sku.Id];
                        sku.Amount = sku.Sku.Price - sku.Sku.Price*promoGetSku.Percent/100;
                        sku.Quantity = promoGetSku.Quantity*applyCount;
                        context.PromoSkus.Add(sku);
                    }
                }
            }
            return 0;
        }
    }
}
