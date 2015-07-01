using System;
using System.Collections.Generic;
using System.Linq;
using Autofac.Features.Indexed;
using VitalChoice.Domain.Entities.eCommerce.Discounts;
using VitalChoice.DynamicData.Entities;
using VitalChoice.DynamicData.Interfaces.Services;
using VitalChoice.DynamicData.Services;

namespace VitalChoice.Business.Services.Dynamic
{
    public class DiscountMapper : DynamicObjectMapper<DiscountDynamic, Discount, DiscountOptionType, DiscountOptionValue>
    {
        public DiscountMapper(IIndex<Type, IDynamicToModelMapper> mappers, IModelToDynamicContainer container) : base(mappers, container)
        {
        }

        protected override void FromEntity(DiscountDynamic dynamic, Discount entity, bool withDefaults = false)
        {
            dynamic.Code = entity.Code;
            dynamic.Description = entity.Description;
            dynamic.DiscountType = entity.IdDiscountType;
            dynamic.Assigned = entity.Assigned;
            dynamic.StartDate = entity.StartDate;
            dynamic.ExpirationDate = entity.ExpirationDate;
            dynamic.ExcludeSkus = entity.ExcludeSkus;
            dynamic.ExcludeCategories = entity.ExcludeCategories;
            dynamic.IdAddedBy = entity.IdAddedBy;

            dynamic.CategoryIds = entity.DiscountsToCategories?.Select(p => p.IdCategory).ToList();
            dynamic.DiscountsToSkus = entity.DiscountsToSkus?.ToList();
            dynamic.DiscountsToSelectedSkus = entity.DiscountsToSelectedSkus?.ToList();
            dynamic.DiscountTiers = entity.DiscountTiers?.ToList();
        }

        protected override void UpdateEntityInternal(DiscountDynamic dynamic, Discount entity)
        {
            SetDiscountTiersOrdering(dynamic.DiscountTiers);
            entity.Code = dynamic.Code;
            entity.Description = dynamic.Description;
            entity.IdDiscountType = dynamic.DiscountType;
            entity.Assigned = dynamic.Assigned;
            entity.StartDate = dynamic.StartDate;
            entity.ExpirationDate = dynamic.ExpirationDate;
            entity.ExcludeSkus = dynamic.ExcludeSkus;
            entity.ExcludeCategories = dynamic.ExcludeCategories;
            entity.IdAddedBy = entity.IdAddedBy;

            entity.DiscountsToCategories = dynamic.CategoryIds?.Select(c => new DiscountToCategory
            {
                IdCategory = c,
                IdDiscount = dynamic.Id
            }).ToList();
            if (dynamic.DiscountsToSkus != null)
            {
                foreach (var item in dynamic.DiscountsToSkus)
                {
                    item.Id = 0;
                    item.IdDiscount = dynamic.Id;
                }
                entity.DiscountsToSkus = dynamic.DiscountsToSkus.ToList();
            }
            if (dynamic.DiscountsToSelectedSkus != null)
            {
                foreach (var item in dynamic.DiscountsToSelectedSkus)
                {
                    item.Id = 0;
                    item.IdDiscount = dynamic.Id;
                }
                entity.DiscountsToSelectedSkus = dynamic.DiscountsToSelectedSkus.ToList();
            }
            if (dynamic.DiscountTiers != null)
            {
                foreach (var item in dynamic.DiscountTiers)
                {
                    item.Id = 0;
                    item.IdDiscount = dynamic.Id;
                }
                entity.DiscountTiers = dynamic.DiscountTiers.ToList();
            }

            //Set key on options
            foreach (var value in entity.OptionValues)
            {
                value.IdDiscount = dynamic.Id;
            }
        }

        protected override void FillNewEntity(DiscountDynamic dynamic, Discount entity)
        {
            SetDiscountTiersOrdering(dynamic.DiscountTiers);
            entity.Code = dynamic.Code;
            entity.Description = dynamic.Description;
            entity.IdDiscountType = dynamic.DiscountType;
            entity.Assigned = dynamic.Assigned;
            entity.StartDate = dynamic.StartDate;
            entity.ExpirationDate = dynamic.ExpirationDate;
            entity.ExcludeSkus = dynamic.ExcludeSkus;
            entity.ExcludeCategories = dynamic.ExcludeCategories;
            entity.IdAddedBy = entity.IdEditedBy;

            entity.DiscountsToCategories = dynamic.CategoryIds?.Select(c => new DiscountToCategory
            {
                IdCategory = c,
                IdDiscount = dynamic.Id
            }).ToList();
            if (dynamic.DiscountsToSkus != null)
            {
                foreach (var item in dynamic.DiscountsToSkus)
                {
                    item.Id = 0;
                    item.IdDiscount = dynamic.Id;
                }
                entity.DiscountsToSkus = dynamic.DiscountsToSkus.ToList();
            }
            if (dynamic.DiscountsToSelectedSkus != null)
            {
                foreach (var item in dynamic.DiscountsToSelectedSkus)
                {
                    item.Id = 0;
                    item.IdDiscount = dynamic.Id;
                }
                entity.DiscountsToSelectedSkus = dynamic.DiscountsToSelectedSkus.ToList();
            }
            if (dynamic.DiscountTiers != null)
            {
                foreach (var item in dynamic.DiscountTiers)
                {
                    item.Id = 0;
                    item.IdDiscount = dynamic.Id;
                }
                entity.DiscountTiers = dynamic.DiscountTiers.ToList();
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
    }
}
