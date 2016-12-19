using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Internal;
using VitalChoice.Data.Repositories.Specifics;
using VitalChoice.DynamicData.Base;
using VitalChoice.DynamicData.Interfaces;
using VitalChoice.Data.Extensions;
using VitalChoice.Ecommerce.Domain.Entities.Discounts;
using VitalChoice.Ecommerce.Domain.Helpers;
using VitalChoice.Infrastructure.Domain.Dynamic;
using VitalChoice.Interfaces.Services.Products;
using VitalChoice.ObjectMapping.Interfaces;

namespace VitalChoice.Business.Services.Dynamic
{
    public class DiscountMapper : DynamicMapper<DiscountDynamic, Discount, DiscountOptionType, DiscountOptionValue>
    {
        private readonly IProductService _productService;

        public DiscountMapper(ITypeConverter converter,
            IModelConverterService converterService,
            IEcommerceRepositoryAsync<DiscountOptionType> discountRepositoryAsync, IProductService productService)
            : base(converter, converterService, discountRepositoryAsync)
        {
            _productService = productService;
        }

        public override Expression<Func<DiscountOptionValue, int>> ObjectIdSelector => d => d.IdDiscount;

        protected override Task FromEntityRangeInternalAsync(ICollection<DynamicEntityPair<DiscountDynamic, Discount>> items, bool withDefaults = false)
        {
            return items.ForEachAsync(async pair =>
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
                dynamic.CategoryIdsAppliedOnlyTo = entity.DiscountsToSelectedCategories?.Select(p => p.IdCategory).ToList();
                dynamic.SkusFilter = entity.DiscountsToSkus?.ToList();
                dynamic.SkusAppliedOnlyTo = entity.DiscountsToSelectedSkus?.ToList();
                dynamic.DiscountTiers = entity.DiscountTiers?.ToList();
                if (dynamic.IdObjectType == (int)DiscountType.Threshold && withDefaults)
                {
                    dynamic.Data.ThresholdSku = await _productService.GetSkuOrderedAsync((string) dynamic.SafeData.ProductSKU);
                }
            });
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

                entity.DiscountsToCategories.MergeKeyed(dynamic.CategoryIds, category => category.IdCategory, i => i,
                    i => new DiscountToCategory
                    {
                        IdCategory = i,
                        IdDiscount = dynamic.Id
                    });

                entity.DiscountsToSelectedCategories.MergeKeyed(dynamic.CategoryIdsAppliedOnlyTo, category => category.IdCategory, i => i,
                    i => new DiscountToSelectedCategory
                    {
                        IdCategory = i,
                        IdDiscount = dynamic.Id
                    });
                if (dynamic.SkusFilter != null)
                {
                    foreach (var item in dynamic.SkusFilter)
                    {
                        item.Id = 0;
                        item.IdDiscount = dynamic.Id;
                    }
                    entity.DiscountsToSkus.MergeKeyed(dynamic.SkusFilter.ToList(), sku => sku.IdSku);
                }
                if (dynamic.SkusAppliedOnlyTo != null)
                {
                    foreach (var item in dynamic.SkusAppliedOnlyTo)
                    {
                        item.Id = 0;
                        item.IdDiscount = dynamic.Id;
                    }
                    entity.DiscountsToSelectedSkus.MergeKeyed(dynamic.SkusAppliedOnlyTo, sku => sku.IdSku);
                }
                if (dynamic.DiscountTiers != null)
                {
                    entity.DiscountTiers.MergeKeyed(dynamic.DiscountTiers, p => p.Id, (a, b) =>
                    {
                        a.From = b.From;
                        a.To = b.To;
                        a.Amount = b.Amount;
                        a.Percent = b.Percent;
                    });

                    if (entity.IdObjectType != (int) DiscountType.Tiered)
                    {
                        entity.DiscountTiers.Clear();
                    }
                }
            });
            return TaskCache.CompletedTask;
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
                entity.DiscountsToSelectedCategories = dynamic.CategoryIdsAppliedOnlyTo?.Select(c => new DiscountToSelectedCategory
                {
                    IdCategory = c,
                    IdDiscount = dynamic.Id
                }).ToList();
                if (dynamic.SkusFilter != null)
                {
                    foreach (var item in dynamic.SkusFilter)
                    {
                        item.Id = 0;
                        item.IdDiscount = dynamic.Id;
                    }
                    entity.DiscountsToSkus = dynamic.SkusFilter.ToList();
                }
                if (dynamic.SkusAppliedOnlyTo != null)
                {
                    foreach (var item in dynamic.SkusAppliedOnlyTo)
                    {
                        item.Id = 0;
                        item.IdDiscount = dynamic.Id;
                    }
                    entity.DiscountsToSelectedSkus = dynamic.SkusAppliedOnlyTo.ToList();
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
            return TaskCache.CompletedTask;
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
