using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using VitalChoice.Data.Repositories.Specifics;
using VitalChoice.DynamicData.Base;
using VitalChoice.DynamicData.Interfaces;
using VitalChoice.Data.Extensions;
using VitalChoice.Ecommerce.Domain.Entities.Promotion;
using VitalChoice.Ecommerce.Domain.Helpers;
using VitalChoice.Infrastructure.Domain.Dynamic;
using VitalChoice.ObjectMapping.Interfaces;

namespace VitalChoice.Business.Services.Dynamic
{
    public class PromotionMapper : DynamicMapper<PromotionDynamic, Promotion, PromotionOptionType, PromotionOptionValue>
    {
        public PromotionMapper(ITypeConverter converter,
            IModelConverterService converterService,
            IEcommerceRepositoryAsync<PromotionOptionType> promotionRepositoryAsync)
            : base(converter, converterService, promotionRepositoryAsync)
        {
        }

        protected override Expression<Func<PromotionOptionValue, int>> ObjectIdReferenceSelector
        {
            get { return c => c.IdPromotion; }
        }

        protected override Task FromEntityRangeInternalAsync(ICollection<DynamicEntityPair<PromotionDynamic, Promotion>> items, bool withDefaults = false)
        {
            items.ForEach(pair =>
            {
                var entity = pair.Entity;
                var dynamic = pair.Dynamic;
                
                dynamic.Description = entity.Description;
                dynamic.Assigned = entity.Assigned;
                dynamic.StartDate = entity.StartDate;
                dynamic.ExpirationDate = entity.ExpirationDate;
                dynamic.IdAddedBy = entity.IdAddedBy;
                
                dynamic.PromotionsToBuySkus = entity.PromotionsToBuySkus?.ToList();
                dynamic.PromotionsToGetSkus = entity.PromotionsToGetSkus?.ToList();
                dynamic.SelectedCategoryIds = entity.PromotionsToSelectedCategories?.Select(p => p.IdCategory).ToList();
            });
            return Task.Delay(0);
        }

        protected override Task UpdateEntityRangeInternalAsync(ICollection<DynamicEntityPair<PromotionDynamic, Promotion>> items)
        {
            items.ForEach(pair =>
            {
                var entity = pair.Entity;
                var dynamic = pair.Dynamic;

                entity.Description = dynamic.Description;
                entity.Assigned = dynamic.Assigned;
                entity.StartDate = dynamic.StartDate;
                entity.ExpirationDate = dynamic.ExpirationDate;
                entity.IdAddedBy = entity.IdAddedBy;

                if (dynamic.PromotionsToBuySkus != null)
                {
                    entity.PromotionsToBuySkus.MergeKeyed(dynamic.PromotionsToBuySkus, sku => sku.IdSku,
                        (sku, rsku) => {
                            sku.Quantity = rsku.Quantity;
                        });
                }
                if (dynamic.PromotionsToGetSkus != null)
                {
                    entity.PromotionsToGetSkus.MergeKeyed(dynamic.PromotionsToGetSkus, sku => sku.IdSku,
                        (sku, rsku)=> {
                            sku.Percent = rsku.Percent;
                            sku.Quantity = rsku.Quantity;
                        });
                }
                entity.PromotionsToSelectedCategories.MergeKeyed(dynamic.SelectedCategoryIds, ecat => ecat.IdCategory, i => i,
                    i => new PromotionToSelectedCategory
                    {
                        IdCategory = i,
                        IdPromotion = dynamic.Id
                    });
            });
            return Task.Delay(0);
        }

        protected override Task ToEntityRangeInternalAsync(ICollection<DynamicEntityPair<PromotionDynamic, Promotion>> items)
        {
            items.ForEach(pair =>
            {
                var entity = pair.Entity;
                var dynamic = pair.Dynamic;
                
                entity.Description = dynamic.Description;
                entity.Assigned = dynamic.Assigned;
                entity.StartDate = dynamic.StartDate;
                entity.ExpirationDate = dynamic.ExpirationDate;
                entity.IdAddedBy = entity.IdEditedBy;
                
                if (dynamic.PromotionsToBuySkus != null)
                {
                    foreach (var item in dynamic.PromotionsToBuySkus)
                    {
                        item.Id = 0;
                        item.IdPromotion = dynamic.Id;
                    }
                    entity.PromotionsToBuySkus = dynamic.PromotionsToBuySkus.ToList();
                }
                if (dynamic.PromotionsToGetSkus != null)
                {
                    foreach (var item in dynamic.PromotionsToGetSkus)
                    {
                        item.Id = 0;
                        item.IdPromotion = dynamic.Id;
                    }
                    entity.PromotionsToGetSkus = dynamic.PromotionsToGetSkus.ToList();
                }
                entity.PromotionsToSelectedCategories = dynamic.SelectedCategoryIds?.Select(c => new PromotionToSelectedCategory
                {
                    IdCategory = c,
                    IdPromotion = dynamic.Id
                }).ToList();
            });
            return Task.Delay(0);
        }
    }
}
