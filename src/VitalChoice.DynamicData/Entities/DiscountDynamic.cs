using System.Collections.Generic;
using System.Linq;
using NuGet;
using VitalChoice.Domain.Entities;
using VitalChoice.Domain.Entities.eCommerce.Products;
using VitalChoice.Domain.Entities.eCommerce.Discounts;
using System;

namespace VitalChoice.DynamicData.Entities
{
    public sealed class DiscountDynamic : DynamicObject<Discount, DiscountOptionValue, DiscountOptionType>
    {
        public DiscountDynamic()
        {

        }

        public DiscountDynamic(Discount entity, bool withDefaults = false) : base(entity, withDefaults)
        {
        }

        public int? IdAddedBy { get; set; }

        public string Code { get; set; }

        public string Description { get; set; }

        public DiscountType DiscountType { get; set; }

        public CustomerTypeCode Assigned { get; set; }

        public DateTime? StartDate { get; set; }

        public DateTime? ExpirationDate { get; set; }

        public bool ExcludeSkus { get; set; }

        public bool ExcludeCategories { get; set; }

        public ICollection<int> CategoryIds { get; set; }

        public ICollection<DiscountToSku> DiscountsToSkus { get; set; }

        public ICollection<DiscountToSelectedSku> DiscountsToSelectedSkus { get; set; }

        public ICollection<DiscountTier> DiscountTiers { get; set; }

        protected override void FillNewEntity(Discount entity)
        {
            SetDiscountTiersOrdering(DiscountTiers);
            entity.Code = Code;
            entity.Description = Description;
            entity.IdDiscountType = DiscountType;
            entity.Assigned = Assigned;
            entity.StartDate = StartDate;
            entity.ExpirationDate = ExpirationDate;
            entity.ExcludeSkus = ExcludeSkus;
            entity.ExcludeCategories = ExcludeCategories;
            entity.IdEditedBy = entity.IdEditedBy;

            entity.DiscountsToCategories = CategoryIds?.Select(c => new DiscountToCategory
            {
                IdCategory = c,
                IdDiscount = Id
            }).ToList();
            if(DiscountsToSkus!=null)
            {
                foreach(var item in DiscountsToSkus)
                {
                    item.Id = 0;
                    item.IdDiscount = Id;
                }
                entity.DiscountsToSkus = DiscountsToSkus.ToList();
            }
            if (DiscountsToSelectedSkus != null)
            {
                foreach (var item in DiscountsToSelectedSkus)
                {
                    item.Id = 0;
                    item.IdDiscount = Id;
                }
                entity.DiscountsToSelectedSkus = DiscountsToSelectedSkus.ToList();
            }
            if (DiscountTiers != null)
            {
                foreach (var item in DiscountTiers)
                {
                    item.Id = 0;
                    item.IdDiscount = Id;
                }
                entity.DiscountTiers = DiscountTiers.ToList();
            }
        }

        private static void SetDiscountTiersOrdering(IEnumerable<DiscountTier> tiers)
        {
            int order = 0;
            if (tiers != null)
            {
                foreach (var tier in tiers)
                {
                    tier.Order = order;
                    order++;
                }
            }
        }

        protected override void UpdateEntityInternal(Discount entity)
        {
            SetDiscountTiersOrdering(DiscountTiers);
            entity.Code = Code;
            entity.Description = Description;
            entity.IdDiscountType = DiscountType;
            entity.Assigned = Assigned;
            entity.StartDate = StartDate;
            entity.ExpirationDate = ExpirationDate;
            entity.ExcludeSkus = ExcludeSkus;
            entity.ExcludeCategories = ExcludeCategories;
            entity.IdAddedBy = entity.IdAddedBy;

            entity.DiscountsToCategories = CategoryIds?.Select(c => new DiscountToCategory
            {
                IdCategory = c,
                IdDiscount = Id
            }).ToList();
            if (DiscountsToSkus != null)
            {
                foreach (var item in DiscountsToSkus)
                {
                    item.Id = 0;
                    item.IdDiscount = Id;
                }
                entity.DiscountsToSkus = DiscountsToSkus.ToList();
            }
            if (DiscountsToSelectedSkus != null)
            {
                foreach (var item in DiscountsToSelectedSkus)
                {
                    item.Id = 0;
                    item.IdDiscount = Id;
                }
                entity.DiscountsToSelectedSkus = DiscountsToSelectedSkus.ToList();
            }
            if (DiscountTiers != null)
            {
                foreach (var item in DiscountTiers)
                {
                    item.Id = 0;
                    item.IdDiscount = Id;
                }
                entity.DiscountTiers = DiscountTiers.ToList();
            }

            //Set key on options
            foreach (var value in entity.OptionValues)
            {
                value.IdDiscount = Id;
            }

        }

        protected override void FromEntity(Discount entity, bool withDefaults = false)
        {
            Code = entity.Code;
            Description = entity.Description;
            DiscountType = entity.IdDiscountType;
            Assigned = entity.Assigned;
            StartDate = entity.StartDate;
            ExpirationDate = entity.ExpirationDate;
            ExcludeSkus = entity.ExcludeSkus;
            ExcludeCategories = entity.ExcludeCategories;
            IdEditedBy = entity.IdAddedBy;

            CategoryIds = entity.DiscountsToCategories?.Select(p => p.IdCategory).ToList();
            DiscountsToSkus = entity.DiscountsToSkus?.ToList();
            DiscountsToSelectedSkus = entity.DiscountsToSelectedSkus?.ToList();
            DiscountTiers = entity.DiscountTiers?.ToList();
        }
    }
}
