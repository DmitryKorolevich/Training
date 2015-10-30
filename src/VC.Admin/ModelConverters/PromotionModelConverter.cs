﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VC.Admin.Models.Product;
using VitalChoice.Domain.Entities.eCommerce.Promotions;
using VitalChoice.Domain.Transfer.Products;
using VitalChoice.DynamicData.Base;
using VitalChoice.DynamicData.Entities;
using VitalChoice.DynamicData.Interfaces;

namespace VC.Admin.ModelConverters
{
    public class PromotionModelConverter : BaseModelConverter<PromotionManageModel, PromotionDynamic>
    {
        public override void DynamicToModel(PromotionManageModel model, PromotionDynamic dynamic)
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
        }

        public override void ModelToDynamic(PromotionManageModel model, PromotionDynamic dynamic)
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
        }
    }
}
