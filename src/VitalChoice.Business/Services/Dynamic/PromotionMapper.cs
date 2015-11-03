using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Autofac.Features.Indexed;
using VitalChoice.Business.Queries.Product;
using VitalChoice.Data.Helpers;
using VitalChoice.Data.Repositories.Specifics;
using VitalChoice.DynamicData.Base;
using VitalChoice.DynamicData.Entities;
using VitalChoice.DynamicData.Interfaces;
using VitalChoice.Data.Extensions;
using VitalChoice.Domain.Entities.eCommerce.Customers;
using VitalChoice.Domain.Entities.eCommerce.Promotion;
using VitalChoice.Domain.Entities.eCommerce.Promotions;

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

        public override Expression<Func<PromotionOptionValue, int?>> ObjectIdSelector
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
