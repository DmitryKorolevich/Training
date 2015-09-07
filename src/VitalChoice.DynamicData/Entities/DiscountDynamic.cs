using System.Collections.Generic;
using System.Linq;
using VitalChoice.Domain.Entities;
using VitalChoice.Domain.Entities.eCommerce.Products;
using VitalChoice.Domain.Entities.eCommerce.Discounts;
using System;
using VitalChoice.DynamicData.Base;

namespace VitalChoice.DynamicData.Entities
{
    public sealed class DiscountDynamic : MappedObject
    {
        public int? IdAddedBy { get; set; }

        public string Code { get; set; }

        public string Description { get; set; }

        public CustomerTypeCode Assigned { get; set; }

        public DateTime? StartDate { get; set; }

        public DateTime? ExpirationDate { get; set; }

        public bool ExcludeSkus { get; set; }

        public bool ExcludeCategories { get; set; }

        public ICollection<int> CategoryIds { get; set; }

        public ICollection<DiscountToSku> DiscountsToSkus { get; set; }

        public ICollection<DiscountToSelectedSku> DiscountsToSelectedSkus { get; set; }

        public ICollection<DiscountTier> DiscountTiers { get; set; }
    }
}