using System;
using System.Collections.Generic;
using VitalChoice.Ecommerce.Domain.Entities.Base;
using VitalChoice.Ecommerce.Domain.Entities.Customers;

namespace VitalChoice.Ecommerce.Domain.Entities.Promotion
{
    public class Promotion : DynamicDataEntity<PromotionOptionValue, PromotionOptionType>
    {
        public int? IdAddedBy { get; set; }

        public string Description { get; set; }

        public CustomerType? Assigned { get; set; }

        public DateTime? StartDate { get; set; }

        public DateTime? ExpirationDate { get; set; }

        public ICollection<PromotionToGetSku> PromotionsToGetSkus { get; set; }

        public ICollection<PromotionToBuySku> PromotionsToBuySkus { get; set; }

        public ICollection<PromotionToSelectedCategory> PromotionsToSelectedCategories { get; set; }
    }
}