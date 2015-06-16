using System;
using System.Collections.Generic;
using VitalChoice.Domain.Entities.eCommerce.Base;

namespace VitalChoice.Domain.Entities.eCommerce.Discounts
{
    public class Discount : DynamicDataEntity<DiscountOptionValue, DiscountOptionType>
    {
        public string Code { get; set; }

        public string Description { get; set; }

        public CustomerTypeCode Assigned { get; set; }

        public DiscountType IdDiscountType { get; set; }

        public int? IdExternal { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime ExpirationDate { get; set; }

        public ICollection<DiscountToCategory> DiscountsToCategories { get; set; }

        public ICollection<DiscountToProduct> DiscountsToProducts { get; set; }
    }
}