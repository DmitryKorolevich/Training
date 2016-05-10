using System.Collections.Generic;
using System.Threading.Tasks;
using VC.Admin.Models.Products;
using VitalChoice.DynamicData.Base;
using VitalChoice.Ecommerce.Domain.Entities.Promotions;
using VitalChoice.Infrastructure.Domain.Dynamic;
using VitalChoice.Infrastructure.Domain.Transfer.Products;

namespace VC.Admin.ModelConverters
{
    public class PromotionModelConverter : BaseModelConverter<PromotionManageModel, PromotionDynamic>
    {
        public override Task DynamicToModelAsync(PromotionManageModel model, PromotionDynamic dynamic)
        {
            if(model.PromotionBuyType == 0)
            {
                model.PromotionBuyType = PromoBuyType.Any;
            }

            if(dynamic.PromotionsToBuySkus!=null)
            {
                model.PromotionsToBuySkus = new List<PromotionToBuySkuModel>();
                foreach(var item in dynamic.PromotionsToBuySkus)
                {
                    model.PromotionsToBuySkus.Add(new PromotionToBuySkuModel()
                    {
                        Id=item.Id,
                        IdPromotion=item.IdPromotion,
                        IdSku=item.IdSku,
                        Quantity = item.Quantity,
                        ShortSkuInfo = item.ShortSkuInfo,
                    });
                }
            }
            if (dynamic.PromotionsToGetSkus != null)
            {
                model.PromotionsToGetSkus = new List<PromotionToGetSkuModel>();
                foreach (var item in dynamic.PromotionsToGetSkus)
                {
                    model.PromotionsToGetSkus.Add(new PromotionToGetSkuModel()
                    {
                        Id = item.Id,
                        IdPromotion = item.IdPromotion,
                        IdSku = item.IdSku,
                        Quantity = item.Quantity,
                        Percent = item.Percent,
                        ShortSkuInfo = item.ShortSkuInfo,
                    });
                }
            }
            return Task.Delay(0);
        }

        public override Task ModelToDynamicAsync(PromotionManageModel model, PromotionDynamic dynamic)
        {
            if (model.PromotionsToBuySkus != null)
            {
                dynamic.PromotionsToBuySkus = new List<PromotionToBuySku>();
                foreach (var item in model.PromotionsToBuySkus)
                {
                    dynamic.PromotionsToBuySkus.Add(new PromotionToBuySku()
                    {
                        Id = item.Id,
                        IdPromotion = item.IdPromotion,
                        IdSku = item.IdSku,
                        Quantity = item.Quantity.HasValue ? item.Quantity.Value : 0,
                        ShortSkuInfo = item.ShortSkuInfo,
                    });
                }
            }

            if (model.PromotionsToGetSkus != null)
            {
                dynamic.PromotionsToGetSkus = new List<PromotionToGetSku>();
                foreach (var item in model.PromotionsToGetSkus)
                {
                    dynamic.PromotionsToGetSkus.Add(new PromotionToGetSku()
                    {
                        Id = item.Id,
                        IdPromotion = item.IdPromotion,
                        IdSku = item.IdSku,
                        Quantity = item.Quantity.HasValue ? item.Quantity.Value : 0,
                        Percent = item.Percent.HasValue ? item.Percent.Value : 0,
                        ShortSkuInfo = item.ShortSkuInfo,
                    });
                }
            }
            return Task.Delay(0);
        }
    }
}
