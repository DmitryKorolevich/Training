using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Autofac.Features.Indexed;
using VitalChoice.Business.Queries.Product;
using VitalChoice.Data.Helpers;
using VitalChoice.Data.Repositories.Specifics;
using VitalChoice.Domain.Entities.eCommerce.Discounts;
using VitalChoice.DynamicData.Base;
using VitalChoice.DynamicData.Entities;
using VitalChoice.DynamicData.Interfaces;
using VitalChoice.Data.Extensions;

namespace VitalChoice.Business.Services.Dynamic
{
    public class DiscountMapper : DynamicObjectMapper<DiscountDynamic, Discount, DiscountOptionType, DiscountOptionValue>
    {
        public DiscountMapper(IIndex<Type, IDynamicToModelMapper> mappers,
            IIndex<Type, IModelToDynamicConverter> container,
            IEcommerceRepositoryAsync<DiscountOptionType> discountRepositoryAsync)
            : base(mappers, container, discountRepositoryAsync)
        {
        }

        public override IQueryObject<DiscountOptionType> GetOptionTypeQuery(int? idType)
        {
            return new DiscountOptionTypeQuery().WithType((DiscountType?)idType);
        }

        protected override Task FromEntityRangeInternalAsync(ICollection<DynamicEntityPair<DiscountDynamic, Discount>> items, bool withDefaults = false)
        {
            items.ForEach(pair =>
            {
                var entity = pair.Entity;
                var dynamic = pair.Dynamic;

                dynamic.Code = entity.Code;
                dynamic.Description = entity.Description;
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
            });
            return Task.Delay(0);
        }

        protected override Task UpdateEntityRangeInternalAsync(ICollection<DynamicEntityPair<DiscountDynamic, Discount>> items)
        {
            items.ForEach(pair =>
            {
                var entity = pair.Entity;
                var dynamic = pair.Dynamic;

                SetDiscountTiersOrdering(dynamic.DiscountTiers);
                entity.Code = dynamic.Code;
                entity.Description = dynamic.Description;
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
            });
            return Task.Delay(0);
        }

        protected override Task ToEntityRangeInternalAsync(ICollection<DynamicEntityPair<DiscountDynamic, Discount>> items)
        {
            items.ForEach(pair =>
            {
                var entity = pair.Entity;
                var dynamic = pair.Dynamic;

                SetDiscountTiersOrdering(dynamic.DiscountTiers);
                entity.Code = dynamic.Code;
                entity.Description = dynamic.Description;
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
            });
            return Task.Delay(0);
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
