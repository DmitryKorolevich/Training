using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Internal;
using VitalChoice.Data.Extensions;
using VitalChoice.Data.Repositories.Specifics;
using VitalChoice.DynamicData.Base;
using VitalChoice.DynamicData.Interfaces;
using VitalChoice.Ecommerce.Domain.Entities.Affiliates;
using VitalChoice.Ecommerce.Domain.Entities.InventorySkus;
using VitalChoice.Ecommerce.Domain.Entities.Users;
using VitalChoice.Infrastructure.Domain.Dynamic;
using VitalChoice.ObjectMapping.Interfaces;

namespace VitalChoice.Business.Services.Dynamic
{
    public class InventorySkuMapper : DynamicMapper<InventorySkuDynamic, InventorySku, InventorySkuOptionType, InventorySkuOptionValue>
    {
        public InventorySkuMapper(ITypeConverter converter,
            IModelConverterService converterService,
            IEcommerceRepositoryAsync<InventorySkuOptionType> optionTypesRepositoryAsync)
            : base(converter, converterService, optionTypesRepositoryAsync)
        {
        }

        protected override Task FromEntityRangeInternalAsync(
            ICollection<DynamicEntityPair<InventorySkuDynamic, InventorySku>> items, bool withDefaults = false)
        {
            items.ForEach(item =>
            {
                var entity = item.Entity;
                var dynamic = item.Dynamic;

                dynamic.Code = entity.Code;
                dynamic.Description = entity.Description;
                dynamic.IdInventorySkuCategory = entity.IdInventorySkuCategory;
            });
            return TaskCache.CompletedTask;
        }

        protected override Task UpdateEntityRangeInternalAsync(
            ICollection<DynamicEntityPair<InventorySkuDynamic, InventorySku>> items)
        {
            items.ForEach(item =>
            {
                var entity = item.Entity;
                var dynamic = item.Dynamic;

                entity.Code = dynamic.Code;
                entity.Description = dynamic.Description;
                entity.IdInventorySkuCategory = dynamic.IdInventorySkuCategory;
            });
            return TaskCache.CompletedTask;
        }

        protected override Task ToEntityRangeInternalAsync(ICollection<DynamicEntityPair<InventorySkuDynamic, InventorySku>> items)
        {
            items.ForEach(item =>
            {
                var entity = item.Entity;
                var dynamic = item.Dynamic;

                entity.Code = dynamic.Code;
                entity.Description = dynamic.Description;
                entity.IdInventorySkuCategory = dynamic.IdInventorySkuCategory;
            });
            return TaskCache.CompletedTask;
        }
    }
}