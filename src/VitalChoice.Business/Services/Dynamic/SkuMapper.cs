using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using VitalChoice.Data.Extensions;
using VitalChoice.Data.Repositories.Specifics;
using VitalChoice.DynamicData.Base;
using VitalChoice.DynamicData.Interfaces;
using VitalChoice.Ecommerce.Domain;
using VitalChoice.Ecommerce.Domain.Entities.Products;
using VitalChoice.Infrastructure.Domain.Dynamic;
using VitalChoice.ObjectMapping.Interfaces;
using VitalChoice.Ecommerce.Domain.Helpers;
using VitalChoice.ObjectMapping.Extensions;

namespace VitalChoice.Business.Services.Dynamic
{
    public class SkuMapper : DynamicMapper<SkuDynamic, Sku, ProductOptionType, SkuOptionValue>
    {
        public ProductMapper ProductMapper { get; set; }

        public SkuMapper(ITypeConverter converter,
            IModelConverterService converterService,
            IEcommerceRepositoryAsync<ProductOptionType> productRepositoryAsync)
            : base(converter, converterService, productRepositoryAsync)
        {
        }

        protected override async Task FromEntityRangeInternalAsync(ICollection<DynamicEntityPair<SkuDynamic, Sku>> items,
            bool withDefaults = false)
        {
            await items.ForEachAsync(async pair =>
            {
                var entity = pair.Entity;
                var dynamic = pair.Dynamic;

                dynamic.Code = entity.Code;
                dynamic.Hidden = entity.Hidden;
                dynamic.Price = entity.Price;
                dynamic.WholesalePrice = entity.WholesalePrice;
                dynamic.Order = entity.Order;
                dynamic.IdProduct = entity.IdProduct;
                dynamic.InventorySkuIds = entity.SkusToInventorySkus?.Select(p => p.IdInventorySku).ToList();
                if (entity.Product.Skus != null && entity.Product.Skus.Any())
                {
                    entity.Product = entity.Product.Clone<Product, Entity>();
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

                entity.SkusToInventorySkus.MergeKeyed(dynamic.InventorySkuIds, p => p.IdInventorySku, i => i,
                    i => new SkuToInventorySku
                    {
                        IdInventorySku = i,
                        IdSku = dynamic.Id
                    });
            });
            return Task.Delay(0);
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

                entity.SkusToInventorySkus = dynamic?.InventorySkuIds.Select(i => new SkuToInventorySku
                    {
                        IdInventorySku = i,
                        IdSku = dynamic.Id
                    }).ToList();
            });
            return Task.Delay(0);
        }
    }
}
