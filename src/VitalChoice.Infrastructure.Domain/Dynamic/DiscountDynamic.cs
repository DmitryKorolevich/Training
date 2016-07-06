using System;
using System.Collections.Generic;
using VitalChoice.Ecommerce.Domain.Dynamic;
using VitalChoice.Ecommerce.Domain.Entities.Customers;
using VitalChoice.Ecommerce.Domain.Entities.Discounts;

namespace VitalChoice.Infrastructure.Domain.Dynamic
{
    public class DiscountDynamic : MappedObject
    {
        public int? IdAddedBy { get; set; }

        public string Code { get; set; }

        public string Description { get; set; }

        public CustomerType? Assigned { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime? ExpirationDate { get; set; }

        public bool ExcludeSkus { get; set; }

        public bool ExcludeCategories { get; set; }

        public ICollection<int> CategoryIds { get; set; }

        public ICollection<DiscountToSku> SkusFilter { get; set; }

        public ICollection<DiscountToSelectedSku> SkusAppliedOnlyTo { get; set; }

        public ICollection<int> CategoryIdsAppliedOnlyTo { get; set; }

        public ICollection<DiscountTier> DiscountTiers { get; set; }
    }
}