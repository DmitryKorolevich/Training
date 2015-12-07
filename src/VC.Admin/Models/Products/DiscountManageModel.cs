using System;
using System.Collections.Generic;
using VC.Admin.Validators.Product;
using VitalChoice.Ecommerce.Domain.Attributes;
using VitalChoice.Validation.Models;
using VitalChoice.Validation.Attributes;
using VitalChoice.Ecommerce.Domain.Entities;
using VitalChoice.Ecommerce.Domain.Entities.Customers;
using VitalChoice.Ecommerce.Domain.Entities.Discounts;
using VitalChoice.Infrastructure.Domain.Entities.Localization.Groups;

namespace VC.Admin.Models.Product
{
    [ApiValidator(typeof(DiscountManageModelValidator))]
    public class DiscountManageModel : BaseModel
    {
        [Map]
        public int Id { get; set; }

        [Map]
        [Localized(GeneralFieldNames.Code)]
        public string Code { get; set; }

        [Map]
        [Localized(GeneralFieldNames.Description)]
        public string Description { get; set; }

        [Map("IdObjectType")]
        public DiscountType DiscountType { get; set; }

        [Map]
        public CustomerType? Assigned { get; set; }
        
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
        public bool AllowHealthwise { get; set; }

        [Map]
        public bool RequireMinimumPerishable { get; set; }

        [Map]
        public decimal RequireMinimumPerishableAmount { get; set; }

        [Map]
        public bool FreeShipping { get; set; }

        [Map]
        public int? MaxTimesUse { get; set; }

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
        [Map]
        public string ProductSKU { get; set; }

        [Map]
        public IList<int> CategoryIds { get; set; }
        
        [Map]
        public IList<int> CategoryIdsAppliedOnlyTo { get; set; }

        [Map("SkusFilter")]
        public IList<DiscountToSku> DiscountsToSkus { get; set; }

        [Map("SkusAppliedOnlyTo")]
        public IList<DiscountToSelectedSku> DiscountsToSelectedSkus { get; set; }

        [Map]
        public IList<DiscountTier> DiscountTiers { get; set; }

        public DiscountManageModel()
        {
            CategoryIds = new List<int>();
            DiscountsToSkus = new List<DiscountToSku>();
            CategoryIdsAppliedOnlyTo = new List<int>();
            DiscountsToSelectedSkus = new List<DiscountToSelectedSku>();
            DiscountTiers = new List<DiscountTier>();
        }
    }
}