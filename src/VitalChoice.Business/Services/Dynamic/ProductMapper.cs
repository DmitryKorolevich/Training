using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Autofac.Features.Indexed;
using VitalChoice.Business.Queries.Product;
using VitalChoice.Data.Helpers;
using VitalChoice.Data.Repositories.Specifics;
using VitalChoice.Domain.Entities;
using VitalChoice.Domain.Entities.eCommerce.Products;
using VitalChoice.DynamicData.Base;
using VitalChoice.DynamicData.Entities;
using VitalChoice.DynamicData.Helpers;
using VitalChoice.DynamicData.Interfaces;
using VitalChoice.Data.Extensions;
using VitalChoice.Domain.Helpers;

namespace VitalChoice.Business.Services.Dynamic
{
    public class ProductMapper : DynamicMapper<ProductDynamic, Product, ProductOptionType, ProductOptionValue>
    {
        private readonly SkuMapper _skuMapper;

        public ProductMapper(ITypeConverter converter,
            IModelConverterService converterService,
            SkuMapper skuMapper, IEcommerceRepositoryAsync<ProductOptionType> productRepositoryAsync)
            : base(converter, converterService, productRepositoryAsync)
        {
            _skuMapper = skuMapper;
        }

        protected override Expression<Func<ProductOptionValue, int?>> ObjectIdReferenceSelector
        {
            get { return c => c.IdProduct; }
        }

        protected override async Task FromEntityRangeInternalAsync(ICollection<DynamicEntityPair<ProductDynamic, Product>> items, bool withDefaults = false)
        {
            await items.ForEachAsync(async pair =>
            {
                var entity = pair.Entity;
                var dynamic = pair.Dynamic;

                dynamic.Name = entity.Name;
                dynamic.Hidden = entity.Hidden;
                dynamic.CategoryIds = entity.ProductsToCategories?.Select(p => p.IdCategory).ToList();
                if (entity.Skus != null)
                {
                    foreach (var sku in entity.Skus)
                    {
                        sku.OptionTypes = entity.OptionTypes;
                        if (withDefaults)
                        {
                            //combine product part in skus
                            foreach (var productValue in entity.OptionValues)
                            {
                                if (sku.OptionValues.All(p => p.IdOptionType != productValue.IdOptionType))
                                {
                                    sku.OptionValues.Add(productValue);
                                }
                            }
                        }
                    }
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
                    entity.ProductsToCategories = dynamic.CategoryIds.Select(c => new ProductToCategory
                    {
                        IdCategory = c,
                        IdProduct = dynamic.Id
                    }).ToList();
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
