using System.Collections.Generic;
using System.Linq;
using VitalChoice.Domain.Entities;
using VitalChoice.Domain.Entities.eCommerce.Products;
using VitalChoice.Domain.Entities.eCommerce.Discounts;
using System;
using VitalChoice.DynamicData.Base;
using VitalChoice.Domain.Entities.eCommerce.Customers;
using VitalChoice.Domain.Entities.eCommerce.Promotions;

namespace VitalChoice.DynamicData.Entities
{
    public sealed class PromotionDynamic : MappedObject
    {
        public int? IdAddedBy { get; set; }

        public string Description { get; set; }

        public CustomerType? Assigned { get; set; }

        public DateTime? StartDate { get; set; }

        public DateTime? ExpirationDate { get; set; }

        public ICollection<PromotionToBuySku> PromotionsToBuySkus { get; set; }

        public ICollection<PromotionToGetSku> PromotionsToGetSkus { get; set; }

        public ICollection<int> SelectedCategoryIds { get; set; }
    }
}