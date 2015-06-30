﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Autofac.Features.Indexed;
using VitalChoice.Domain.Entities;
using VitalChoice.Domain.Entities.eCommerce.Products;
using VitalChoice.DynamicData.Entities;
using VitalChoice.DynamicData.Interfaces.Services;
using VitalChoice.DynamicData.Services;
using VitalChoice.DynamicData.Helpers;

namespace VitalChoice.Business.Services.Dynamic
{
    public class ProductMapper : DynamicObjectMapper<ProductMapped, Product, ProductOptionType, ProductOptionValue>
    {
        private readonly SkuMapper _skuMapper;

        public ProductMapper(IIndex<Type, IDynamicToModelMapper> mappers, IModelToDynamicContainer container,
            SkuMapper skuMapper)
            : base(mappers, container)
        {
            _skuMapper = skuMapper;
        }

        protected override void FillNewEntity(ProductMapped dynamic, Product entity)
        {
            SetSkuOrdering(dynamic.Skus);
            entity.Hidden = dynamic.Hidden;
            entity.IdProductType = dynamic.Type;
            entity.Name = dynamic.Name;
            entity.Url = dynamic.Url;
            entity.ProductsToCategories = dynamic.CategoryIds?.Select(c => new ProductToCategory
            {
                IdCategory = c,
                IdProduct = dynamic.Id
            }).ToList();

            entity.Skus = dynamic.Skus?.Select(s => _skuMapper.ToEntity(s, entity.OptionTypes)).ToList() ?? new List<Sku>();
        }

        private static void SetSkuOrdering(IEnumerable<SkuMapped> skus)
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

        protected override void FromEntity(ProductMapped dynamic, Product entity, bool withDefaults = false)
        {
            dynamic.Name = entity.Name;
            dynamic.Url = entity.Url;
            dynamic.Type = entity.IdProductType;
            dynamic.Hidden = entity.Hidden;
            dynamic.CategoryIds = entity.ProductsToCategories.Select(p => p.IdCategory).ToList();
            dynamic.Skus = new List<SkuMapped>();
            foreach (var sku in entity.Skus)
            {
                sku.OptionTypes = entity.OptionTypes;
                SkuMapped skuMapped;
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
                    skuMapped = _skuMapper.FromEntity(sku, true);
                }
                else
                {
                    skuMapped = _skuMapper.FromEntity(sku);
                }
                dynamic.Skus.Add(skuMapped);
            }
        }

        protected override void UpdateEntityInternal(ProductMapped dynamic, Product entity)
        {
            SetSkuOrdering(dynamic.Skus);
            entity.Hidden = dynamic.Hidden;
            entity.IdProductType = dynamic.Type;
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
                    (skuDynamic, sku) => new { skuDynamic, sku });
                foreach (var item in itemsToUpdate)
                {
                    _skuMapper.UpdateEntity(item.skuDynamic, item.sku);
                }

                //Delete
                var toDelete = entity.Skus.Where(e => dynamic.Skus.All(s => s.Id != e.Id));
                foreach (var sku in toDelete)
                {
                    sku.StatusCode = RecordStatusCode.Deleted;
                }

                //Insert
                entity.Skus.AddRange(dynamic.Skus.Where(s => s.Id == 0).Select(s =>
                {
                    var sku = _skuMapper.ToEntity(s, entity.OptionTypes);
                    sku.IdProduct = dynamic.Id;
                    return sku;
                }));
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
        }
    }
}