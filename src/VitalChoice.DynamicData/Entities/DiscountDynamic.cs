using System.Collections.Generic;
using System.Linq;
using VitalChoice.Domain.Entities;
using VitalChoice.Domain.Entities.eCommerce.Products;
using VitalChoice.Domain.Entities.eCommerce.Discounts;
using System;
using VitalChoice.DynamicData.Base;
using VitalChoice.Domain.Entities.eCommerce.Customers;

namespace VitalChoice.DynamicData.Entities
{
    public sealed class DiscountDynamic : MappedObject
    {
        public int? IdAddedBy { get; set; }

        public string Code { get; set; }

        public string Description { get; set; }

        public CustomerType? Assigned { get; set; }

        public DateTime? StartDate { get; set; }

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