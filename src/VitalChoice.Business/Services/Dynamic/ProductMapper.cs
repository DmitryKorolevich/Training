﻿using System;
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

namespace VitalChoice.Business.Services.Dynamic
{
    public class ProductMapper : DynamicObjectMapper<ProductDynamic, Product, ProductOptionType, ProductOptionValue>
    {
        private readonly SkuMapper _skuMapper;

        public ProductMapper(IIndex<Type, IDynamicToModelMapper> mappers, IIndex<Type, IModelToDynamicConverter> container,
            SkuMapper skuMapper, IEcommerceRepositoryAsync<ProductOptionType> productRepositoryAsync)
            : base(mappers, container, productRepositoryAsync)
        {
            _skuMapper = skuMapper;
        }

        public override Expression<Func<ProductOptionValue, int?>> ObjectIdSelector
        {
            get { return c => c.IdProduct; }
        }

        public override IQueryOptionType<ProductOptionType> GetOptionTypeQuery()
        {
            return new ProductOptionTypeQuery();
        }

        protected async override Task FromEntityRangeInternalAsync(ICollection<DynamicEntityPair<ProductDynamic, Product>> items, bool withDefaults = false)
        {
            await items.ForEachAsync(async pair =>
            {
                var entity = pair.Entity;
                var dynamic = pair.Dynamic;

                dynamic.Name = entity.Name;
                dynamic.Url = entity.Url;
                dynamic.Hidden = entity.Hidden;
                dynamic.CategoryIds = entity.ProductsToCategories?.Select(p => p.IdCategory).ToList();
                dynamic.Skus = new List<SkuDynamic>();
                foreach (var sku in entity.Skus)
                {
                    sku.OptionTypes = entity.OptionTypes;
                    SkuDynamic skuDynamic;
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
                        skuDynamic = await _skuMapper.FromEntityAsync(sku, true);
                    }
                    else
                    {
                        skuDynamic = await _skuMapper.FromEntityAsync(sku);
                    }
                    dynamic.Skus.Add(skuDynamic);
                }
            });
        }

        protected async override Task UpdateEntityRangeInternalAsync(ICollection<DynamicEntityPair<ProductDynamic, Product>> items)
        {
            await items.ForEachAsync(async pair =>
            {
                var entity = pair.Entity;
                var dynamic = pair.Dynamic;

                SetSkuOrdering(dynamic.Skus);
                entity.Hidden = dynamic.Hidden;
                entity.Name = dynamic.Name;
                entity.Url = dynamic.Url;

                entity.ProductsToCategories = dynamic.CategoryIds.Select(c => new ProductToCategory
                {
                    IdCategory = c,
                    IdProduct = dynamic.Id
                }).ToList();

                if (dynamic.Skus != null && dynamic.Skus.Any())
                {
                    //Update existing
                    var itemsToUpdate = dynamic.Skus.Join(entity.Skus, sd => sd.Id, s => s.Id,
                        (skuDynamic, sku) => new DynamicEntityPair<SkuDynamic, Sku>(skuDynamic, sku)).ToList();

                    await _skuMapper.UpdateEntityRangeAsync(itemsToUpdate);

                    //Delete
                    var toDelete = entity.Skus.Where(e => dynamic.Skus.All(s => s.Id != e.Id));
                    foreach (var sku in toDelete)
                    {
                        sku.StatusCode = RecordStatusCode.Deleted;
                    }

                    //Insert
                    var skus = await _skuMapper.ToEntityRangeAsync(dynamic.Skus.Where(s => s.Id == 0).ToList());
                    entity.Skus.AddRange(skus);
                }
                else
                {
                    foreach (var sku in entity.Skus)
                    {
                        sku.StatusCode = RecordStatusCode.Deleted;
                    }
                }

                //Set key on options
                foreach (var value in entity.OptionValues)
                {
                    value.IdProduct = dynamic.Id;
                }
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
                entity.Url = dynamic.Url;
                entity.ProductsToCategories = dynamic.CategoryIds?.Select(c => new ProductToCategory
                {
                    IdCategory = c,
                    IdProduct = dynamic.Id
                }).ToList();

                entity.Skus.AddRange(await _skuMapper.ToEntityRangeAsync(dynamic.Skus));
            });
        }

        private static void SetSkuOrdering(IEnumerable<SkuDynamic> skus)
        {
            int order = 0;
            foreach (var sku in skus)
            {
                if (sku.StatusCode != RecordStatusCode.Deleted)
                {
                    sku.Order = order;
                    order++;
                }
            }
        }
    }
}
