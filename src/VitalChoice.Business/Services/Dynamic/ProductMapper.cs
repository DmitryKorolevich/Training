using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using VitalChoice.Data.Repositories.Specifics;
using VitalChoice.DynamicData.Base;
using VitalChoice.DynamicData.Interfaces;
using VitalChoice.Data.Extensions;
using VitalChoice.Ecommerce.Domain.Entities;
using VitalChoice.Ecommerce.Domain.Entities.Products;
using VitalChoice.Ecommerce.Domain.Helpers;
using VitalChoice.Infrastructure.Domain.Dynamic;
using VitalChoice.Interfaces.Services;
using VitalChoice.ObjectMapping.Interfaces;

namespace VitalChoice.Business.Services.Dynamic
{
    public class ProductMapper : DynamicMapper<ProductDynamic, Product, ProductOptionType, ProductOptionValue>
    {
        private readonly SkuMapper _skuMapper;

        public ProductMapper(ITypeConverter converter,
            IModelConverterService converterService,
            SkuMapper skuMapper, IEcommerceRepositoryAsync<ProductOptionType> productRepositoryAsync, ILoggerProviderExtended loggerProvider)
            : base(converter, converterService, productRepositoryAsync)
        {
            _skuMapper = skuMapper;
        }

        protected override async Task FromEntityRangeInternalAsync(ICollection<DynamicEntityPair<ProductDynamic, Product>> items,
            bool withDefaults = false)
        {
            await items.ForEachAsync(async pair =>
            {
                var entity = pair.Entity;
                var dynamic = pair.Dynamic;

                dynamic.Name = entity.Name;
                dynamic.PublicId = entity.PublicId;
                dynamic.Hidden = entity.Hidden;
                dynamic.CategoryIds = entity.ProductsToCategories?.Select(p => p.IdCategory).ToList();
                if (entity.Skus != null)
                {
                    if (dynamic.Skus == null)
                        dynamic.Skus = new List<SkuDynamic>();
                    dynamic.Skus.AddRange(await _skuMapper.FromEntityRangeAsync(entity.Skus, withDefaults));
                }
            });
        }

        protected override async Task UpdateEntityRangeInternalAsync(ICollection<DynamicEntityPair<ProductDynamic, Product>> items)
        {
            await items.ForEachAsync(async pair =>
            {
                var entity = pair.Entity;
                var dynamic = pair.Dynamic;

                SetSkuOrdering(dynamic.Skus);
                entity.Hidden = dynamic.Hidden;
                entity.Name = dynamic.Name;

                if (dynamic.CategoryIds != null)
                {
                    entity.ProductsToCategories.MergeKeyed(dynamic.CategoryIds, category => category.IdCategory, i => i,
                        i => new ProductToCategory
                        {
                            IdCategory = i,
                            IdProduct = dynamic.Id
                        });
                }

                await _skuMapper.SyncCollectionsAsync(dynamic.Skus, entity.Skus, entity.OptionTypes);
            });
        }

        protected override async Task ToEntityRangeInternalAsync(
            ICollection<DynamicEntityPair<ProductDynamic, Product>> items)
        {
            await items.ForEachAsync(async pair =>
            {
                var entity = pair.Entity;
                var dynamic = pair.Dynamic;

                SetSkuOrdering(dynamic.Skus);
                entity.Hidden = dynamic.Hidden;
                entity.Name = dynamic.Name;
                entity.PublicId = dynamic.PublicId;
                entity.ProductsToCategories = dynamic.CategoryIds?.Select(c => new ProductToCategory
                {
                    IdCategory = c,
                    IdProduct = dynamic.Id
                }).ToList();

                if (entity.Skus == null)
                    entity.Skus = new List<Sku>();
                entity.Skus.AddRange(await _skuMapper.ToEntityRangeAsync(dynamic.Skus, entity.OptionTypes));
            });
        }

        private static void SetSkuOrdering(IEnumerable<SkuDynamic> skus)
        {
            int order = 0;
            foreach (var sku in skus)
            {
                if (sku.StatusCode != (int)RecordStatusCode.Deleted)
                {
                    sku.Order = order;
                    order++;
                }
            }
        }
    }
}
