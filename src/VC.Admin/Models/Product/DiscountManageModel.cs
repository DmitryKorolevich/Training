using System;
using System.Collections.Generic;
using System.Linq;
using VC.Admin.Validators.Product;
using VitalChoice.Domain.Entities;
using VitalChoice.Domain.Entities.Content;
using VitalChoice.Validation.Models;
using VitalChoice.Validation.Attributes;
using VitalChoice.Validation.Models.Interfaces;
using VitalChoice.DynamicData.Attributes;
using VitalChoice.DynamicData;
using VitalChoice.Domain.Constants;
using VitalChoice.Domain.Entities.eCommerce.Products;
using VitalChoice.Domain.Entities.Localization.Groups;
using VitalChoice.DynamicData.Entities;
using VitalChoice.Domain.Entities.eCommerce.Discounts;
using VitalChoice.DynamicData.Interfaces;

namespace VC.Admin.Models.Product
{
    [ApiValidator(typeof(DiscountManageModelValidator))]
    public class DiscountManageModel : Model<DiscountDynamic, IMode>, IModelToDynamic<DiscountDynamic>
    {
        [Map]
        public int Id { get; set; }

        [Map]
        [Localized(GeneralFieldNames.Code)]
        public string Code { get; set; }

        [Map]
        [Localized(GeneralFieldNames.Description)]
        public string Description { get; set; }

        [Map]
        public DiscountType DiscountType { get; set; }

        [Map]
        public CustomerTypeCode Assigned { get; set; }
        
        [Map]
        public DateTime? StartDate { get; set; }

        [Map]
        public DateTime? ExpirationDate { get; set; }

        [Map]
        public bool ExcludeSkus { get; set; }

        [Map]
        public bool ExcludeCategories { get; set; }

        [Map]
        public RecordStatusCode StatusCode { get; set; }


        [Map]
        public bool OneTimeOnly { get; set; }

        [Map]
        public bool AllowHealthwise { get; set; }

        [Map]
        public bool RequireMinimumPerishable { get; set; }

        [Map]
        public decimal RequireMinimumPerishableAmount { get; set; }

        [Map]
        public bool FreeShipping { get; set; }

        //1
        [Map]
        public decimal Amount { get; set; }

        //2
        [Map]
        public decimal Percent { get; set; }

        //4
        [Map]
        public decimal Threshold { get; set; }

        //4
        //A SKU code for the frontend UI
        [Localized(GeneralFieldNames.Code)]
        public string ProductSKU { get; set; }


        public IList<int> CategoryIds { get; set; }

        public IList<DiscountToSku> DiscountsToSkus { get; set; }

        public IList<DiscountToSelectedSku> DiscountsToSelectedSkus { get; set; }

        public IList<DiscountTier> DiscountTiers { get; set; }

        public DiscountManageModel()
        {
            CategoryIds = new List<int>();
            DiscountsToSkus = new List<DiscountToSku>();
            DiscountsToSelectedSkus = new List<DiscountToSelectedSku>();
            DiscountTiers = new List<DiscountTier>();
        }

        public override DiscountDynamic Convert()
        {
            DiscountDynamic toReturn = new DiscountDynamic();
            toReturn.FromModel<DiscountManageModel, DiscountDynamic>(this);

            if(toReturn.StartDate.HasValue)
            {
                toReturn.StartDate = new DateTime(toReturn.StartDate.Value.Year, toReturn.StartDate.Value.Month, toReturn.StartDate.Value.Day);
            }
            if (toReturn.ExpirationDate.HasValue)
            {
                toReturn.ExpirationDate = (new DateTime(ExpirationDate.Value.Year, ExpirationDate.Value.Month, ExpirationDate.Value.Day)).AddDays(1);
            }

            return toReturn;
        }

        public void FillDynamic(DiscountDynamic dynamicObject)
        {
            dynamicObject.CategoryIds = CategoryIds.ToList();
            dynamicObject.DiscountsToSkus = DiscountsToSkus.ToList();
            dynamicObject.DiscountsToSelectedSkus = DiscountsToSelectedSkus.ToList();
            dynamicObject.DiscountTiers = DiscountTiers.ToList();

            foreach(var item in dynamicObject.DiscountTiers)
            {
                if(item.IdDiscountType==DiscountType.PriceDiscount)
                {
                    item.Percent = null;
                }
                if (item.IdDiscountType == DiscountType.PercentDiscount)
                {
                    item.Amount = null;
                }
                if(item.Amount.HasValue)
                {
                    item.Amount = Math.Round(item.Amount.Value * 100)/100;
                }
                if (item.Percent.HasValue)
                {
                    item.Percent = Math.Round(item.Percent.Value * 100) / 100;
                }
            }
        }

        public void FillSelfFrom(DiscountDynamic dynamicObject)
        {
            if(ExpirationDate.HasValue)
            {
                ExpirationDate = ExpirationDate.Value.AddDays(-1);
            }

            CategoryIds = dynamicObject.CategoryIds.ToList();
            DiscountsToSkus = dynamicObject.DiscountsToSkus.ToList();
            DiscountsToSelectedSkus = dynamicObject.DiscountsToSelectedSkus.ToList();
            DiscountTiers = dynamicObject.DiscountTiers.ToList();
        }
    }
}