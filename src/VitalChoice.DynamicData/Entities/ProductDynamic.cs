using System.Collections.Generic;
using System.Linq;
using VitalChoice.Domain.Entities;
using VitalChoice.Domain.Entities.eCommerce.Product;

namespace VitalChoice.DynamicData.Entities
{
    public class ProductDynamic : DynamicObject<ProductEntity, ProductOptionValue, ProductOptionType>
    {
        public string Name { get; set; }

        public string Url { get; set; }

        public ProductType Type { get; set; }

        public RecordStatusCode StatusCode { get; set; }

        public bool Hidden { get; set; }

        public int? IdExternal { get; set; }

        public ICollection<SkuDynamic> Skus { get; set; }

        public ICollection<int> CategoryIds { get; set; }

        public override ProductEntity ToEntity()
        {
            var entity = base.ToEntity();
            //entity.
            return entity;
        }

        public override IDynamicEntity<ProductEntity, ProductOptionValue, ProductOptionType> FromEntity(ProductEntity entity)
        {
            BaseConvert(entity);
            return base.FromEntity(entity);
        }

        public override IDynamicEntity<ProductEntity, ProductOptionValue, ProductOptionType> FromEntityWithDefaults(ProductEntity entity)
        {
            BaseConvert(entity, true);
            return base.FromEntityWithDefaults(entity);
        }

        private void BaseConvert(ProductEntity entity, bool withDefaults = false)
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
                var skuDynamic = new SkuDynamic();
                if (withDefaults)
                {
                    //combine product part in skus
                    foreach (var productValue in entity.OptionValues)
                    {
                        if (sku.OptionValues.FirstOrDefault(p => p.IdOptionType==productValue.IdOptionType) == null)
                        {
                            sku.OptionValues.Add(productValue);
                        }
                    }
                    skuDynamic.FromEntityWithDefaults(sku);
                }
                else
                {
                    skuDynamic.FromEntity(sku);
                }
                Skus.Add(skuDynamic);
            }
        }
    }
}
