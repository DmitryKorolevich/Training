using System;
using System.Collections.Generic;
using VitalChoice.Domain.Entities.eCommerce.Base;
using VitalChoice.Domain.Entities.eCommerce.Customers;

namespace VitalChoice.Domain.Entities.eCommerce.Promotions
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
    }
}