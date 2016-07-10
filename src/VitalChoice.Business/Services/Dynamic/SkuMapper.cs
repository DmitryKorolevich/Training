using System;
using System.Linq;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Internal;
using VitalChoice.Data.Repositories.Specifics;
using VitalChoice.DynamicData.Base;
using VitalChoice.DynamicData.Interfaces;
using VitalChoice.Ecommerce.Domain;
using VitalChoice.Ecommerce.Domain.Entities.Products;
using VitalChoice.Infrastructure.Domain.Dynamic;
using VitalChoice.ObjectMapping.Interfaces;
using VitalChoice.Ecommerce.Domain.Helpers;
using VitalChoice.ObjectMapping.Extensions;
using VitalChoice.Data.Extensions;

namespace VitalChoice.Business.Services.Dynamic
{
    public class SkuMapper : DynamicMapper<SkuDynamic, Sku, SkuOptionType, SkuOptionValue>
    {
        public ProductMapper ProductMapper { get; set; }

        public SkuMapper(ITypeConverter converter,
            IModelConverterService converterService,
            IEcommerceRepositoryAsync<SkuOptionType> productRepositoryAsync)
            : base(converter, converterService, productRepositoryAsync)
        {
        }

        public override Expression<Func<SkuOptionValue, int>> ObjectIdSelector => s => s.IdSku;

        protected override Task FromEntityRangeInternalAsync(ICollection<DynamicEntityPair<SkuDynamic, Sku>> items,
            bool withDefaults = false)
        {
            return items.ForEachAsync(async pair =>
            {
                var entity = pair.Entity;
                var dynamic = pair.Dynamic;

                dynamic.Code = entity.Code;
                dynamic.Hidden = entity.Hidden;
                dynamic.Price = entity.Price;
                dynamic.WholesalePrice = entity.WholesalePrice;
                dynamic.Order = entity.Order;
                dynamic.IdProduct = entity.IdProduct;
                dynamic.SkusToInventorySkus = entity.SkusToInventorySkus?.ToList();
                if ((entity.Product?.Skus?.Count ?? 0) > 0)
                {
                    entity.Product = entity.Product.Clone();
                    entity.Product.Skus = null;
                }
                dynamic.Product = await ProductMapper.FromEntityAsync(entity.Product, true);
            });
        }

        protected override Task UpdateEntityRangeInternalAsync(ICollection<DynamicEntityPair<SkuDynamic, Sku>> items)
        {
            items.ForEach(pair =>
            {
                var entity = pair.Entity;
                var dynamic = pair.Dynamic;

                entity.Code = dynamic.Code;
                entity.Hidden = dynamic.Hidden;
                entity.Price = dynamic.Price;
                entity.WholesalePrice = dynamic.WholesalePrice;
                entity.Order = dynamic.Order;

                if (dynamic.SkusToInventorySkus != null)
                {
                    entity.SkusToInventorySkus.MergeKeyed(dynamic.SkusToInventorySkus, p => p.IdInventorySku, (sku, rsku) => sku.Quantity = rsku.Quantity);
                }
            });
            return TaskCache.CompletedTask;
        }

        protected override Task ToEntityRangeInternalAsync(ICollection<DynamicEntityPair<SkuDynamic, Sku>> items)
        {
            items.ForEach(pair =>
            {
                var entity = pair.Entity;
                var dynamic = pair.Dynamic;

                entity.Code = dynamic.Code;
                entity.Hidden = dynamic.Hidden;
                entity.Price = dynamic.Price;
                entity.WholesalePrice = dynamic.WholesalePrice;
                entity.Order = dynamic.Order;

                entity.SkusToInventorySkus = dynamic.SkusToInventorySkus?.ToList();
            });
            return TaskCache.CompletedTask;
        }
    }
}
