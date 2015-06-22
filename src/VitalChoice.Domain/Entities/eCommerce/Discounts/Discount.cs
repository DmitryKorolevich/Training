using System;
using System.Collections.Generic;
using VitalChoice.Domain.Entities.eCommerce.Base;

namespace VitalChoice.Domain.Entities.eCommerce.Discounts
{
    public class Discount : DynamicDataEntity<DiscountOptionValue, DiscountOptionType>
    {
        public int? IdAddedBy { get; set; }

        public string Code { get; set; }

        public string Description { get; set; }

        public CustomerTypeCode Assigned { get; set; }

        public DiscountType IdDiscountType { get; set; }

        public DateTime? StartDate { get; set; }

        public DateTime? ExpirationDate { get; set; }

        public bool ExcludeSkus { get; set; }

        public bool ExcludeCategories { get; set; }

        public ICollection<DiscountToCategory> DiscountsToCategories { get; set; }

        public ICollection<DiscountToSku> DiscountsToSkus { get; set; }

        public ICollection<DiscountToSelectedSku> DiscountsToSelectedSkus { get; set; }

        public ICollection<DiscountTier> DiscountTiers { get; set; }
    }
}