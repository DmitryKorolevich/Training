﻿using System;
using System.Collections.Generic;
using VC.Admin.Validators.Product;
using VitalChoice.Ecommerce.Domain.Attributes;
using VitalChoice.Validation.Models;
using VitalChoice.Validation.Attributes;
using VitalChoice.Ecommerce.Domain.Entities;
using VitalChoice.Ecommerce.Domain.Entities.Customers;
using VitalChoice.Ecommerce.Domain.Entities.Products;
using VitalChoice.Ecommerce.Domain.Entities.Promotion;
using VitalChoice.Infrastructure.Domain.Entities.Localization.Groups;
using VitalChoice.Infrastructure.Domain.Transfer.Products;

namespace VC.Admin.Models.Product
{
    [ApiValidator(typeof(PromotionManageModelValidator))]
    public class PromotionManageModel : BaseModel
    {
        [Map]
        public int Id { get; set; }

        [Map]
        [Localized(GeneralFieldNames.Description)]
        public string Description { get; set; }

        [Map]
        public PromotionType IdObjectType { get; set; }

        [Map]
        public CustomerType? Assigned { get; set; }
        
        [Map]
        public DateTime? StartDate { get; set; }

        [Map]
        public DateTime? ExpirationDate { get; set; }

        [Map]
        public RecordStatusCode StatusCode { get; set; }


        [Map]
        public bool AllowHealthwise { get; set; }

        //1
        [Map]
        public int? MaxTimesUse { get; set; }

        //2
        [Map]
        public decimal Percent { get; set; }

        [Map]
        public PromoBuyType PromotionBuyType { get; set; }


        [Map]
        public IList<PromotionToBuySkuModel> PromotionsToBuySkus { get; set; }

        [Map]
        public IList<PromotionToGetSkuModel> PromotionsToGetSkus { get; set; }

        [Map]
        public IList<int> SelectedCategoryIds { get; set; }

        public PromotionManageModel()
        {
            PromotionsToBuySkus = new List<PromotionToBuySkuModel>();
            PromotionsToGetSkus = new List<PromotionToGetSkuModel>();
            SelectedCategoryIds = new List<int>();
        }
    }

    public class PromotionToBuySkuModel : BaseModel
    {
        public int Id { get; set; }

        public int IdSku { get; set; }

        public int IdPromotion { get; set; }

        public int? Quantity { get; set; }

        public ShortSkuInfo ShortSkuInfo { get; set; }
    }


    public class PromotionToGetSkuModel : BaseModel
    {
        public int Id { get; set; }

        public int IdSku { get; set; }

        public int IdPromotion { get; set; }

        public int? Quantity { get; set; }

        public decimal? Percent { get; set; }

        public ShortSkuInfo ShortSkuInfo { get; set; }
    }
}