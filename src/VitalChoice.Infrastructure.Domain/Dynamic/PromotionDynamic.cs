using System;
using System.Collections.Generic;
using VitalChoice.Ecommerce.Domain.Dynamic;
using VitalChoice.Ecommerce.Domain.Entities.Customers;
using VitalChoice.Ecommerce.Domain.Entities.Promotions;

namespace VitalChoice.Infrastructure.Domain.Dynamic
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