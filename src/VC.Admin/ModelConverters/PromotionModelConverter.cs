using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VC.Admin.Models.Product;
using VitalChoice.Domain.Entities.eCommerce.Promotions;
using VitalChoice.DynamicData.Entities;
using VitalChoice.DynamicData.Interfaces;

namespace VC.Admin.ModelConverters
{
    public class PromotionModelConverter : IModelToDynamicConverter<PromotionManageModel, PromotionDynamic>
    {
        public void DynamicToModel(PromotionManageModel model, PromotionDynamic dynamic)
        {
            model.ExpirationDate = model.ExpirationDate?.AddDays(-1);

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
        }

        public void ModelToDynamic(PromotionManageModel model, PromotionDynamic dynamic)
        {
            if (dynamic.StartDate.HasValue)
            {
                dynamic.StartDate = new DateTime(dynamic.StartDate.Value.Year, dynamic.StartDate.Value.Month, dynamic.StartDate.Value.Day);
            }
            if (dynamic.ExpirationDate != null)
            {
                dynamic.ExpirationDate = (new DateTime(dynamic.ExpirationDate.Value.Year, dynamic.ExpirationDate.Value.Month, dynamic.ExpirationDate.Value.Day)).AddDays(1);
            }

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
        }
    }
}
