using System.Collections.Generic;
using System.Linq;
using NuGet;
using VitalChoice.Domain.Entities;
using VitalChoice.Domain.Entities.eCommerce.Products;

namespace VitalChoice.DynamicData.Entities
{
    public sealed class ProductDynamic : DynamicObject<Product, ProductOptionValue, ProductOptionType>
    {
        public ProductDynamic()
        {
            
        }

        public ProductDynamic(Product entity, bool withDefaults = false) : base(entity, withDefaults)
        {
        }

        public string Name { get; set; }

        public string Url { get; set; }

        public ProductType Type { get; set; }

        public bool Hidden { get; set; }

        public int? IdExternal { get; set; }

        public ICollection<SkuDynamic> Skus { get; set; }

        public ICollection<int> CategoryIds { get; set; }

        protected override void FillNewEntity(Product entity)
        {
            entity.Hidden = Hidden;
            entity.IdExternal = IdExternal;
            entity.IdProductType = Type;
            entity.Name = Name;
            entity.Url = Url;
            entity.ProductsToCategories = CategoryIds?.Select(c => new ProductToCategory
            {
                IdCategory = c,
                IdProduct = Id
            }).ToList();

            entity.Skus = Skus?.Select(s => s.ToEntity()).ToList() ?? new List<Sku>();
        }

        protected override void UpdateEntityInternal(Product entity)
        {
            entity.Hidden = Hidden;
            entity.IdExternal = IdExternal;
            entity.IdProductType = Type;
            entity.Name = Name;
            entity.Url = Url;
            entity.ProductsToCategories = CategoryIds.Select(c => new ProductToCategory
            {
                IdCategory = c,
                IdProduct = Id
            }).ToList();

            if (Skus != null && Skus.Any())
            {
                //Update existing
                var itemsToUpdate = Skus.Join(entity.Skus, sd => sd.Id, s => s.Id,
                    (skuDynamic, sku) => new {skuDynamic, sku});
                foreach (var item in itemsToUpdate)
                {
                    item.skuDynamic.UpdateEntity(item.sku);
                }

                //Delete
                var toDelete = entity.Skus.Where(e => Skus.All(s => s.Id != e.Id));
                foreach (var sku in toDelete)
                {
                    sku.StatusCode = RecordStatusCode.Deleted;
                }

                //Insert
                entity.Skus.AddRange(Skus.Where(s => s.Id == 0).Select(s =>
                {
                    var sku = s.ToEntity();
                    sku.IdProduct = Id;
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
                value.IdProduct = Id;
            }
        }

        protected override void FromEntity(Product entity)
        {
            BaseConvert(entity);
        }

        protected override void FromEntityWithDefaults(Product entity)
        {
            BaseConvert(entity, true);
        }

        private void BaseConvert(Product entity, bool withDefaults = false)
        {
            Name = entity.Name;
            Url = entity.Url;
            Type = entity.IdProductType;
            Hidden = entity.Hidden;
            IdExternal = entity.IdExternal;
            CategoryIds = entity.ProductsToCategories.Select(p => p.IdCategory).ToList();
            Skus = new List<SkuDynamic>();
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
                    skuDynamic = new SkuDynamic(sku, true);
                }
                else
                {
                    skuDynamic = new SkuDynamic(sku);
                }
                Skus.Add(skuDynamic);
            }
        }
    }
}
