using System.Collections.Generic;
using System.Linq;
using VitalChoice.Domain.Entities;
using VitalChoice.Domain.Entities.eCommerce.Product;

namespace VitalChoice.DynamicData.Entities
{
    public class ProductDynamic : DynamicObject<ProductEntity, ProductOptionValue, ProductOptionType>
    {
        public int Id { get; set; }

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
            return base.ToEntity();
        }

        public override IDynamicEntity<ProductEntity, ProductOptionValue, ProductOptionType> FromEntity(ProductEntity entity)
        {
            BaseConvert(entity);
            return base.FromEntity(entity);
        }

        public override IDynamicEntity<ProductEntity, ProductOptionValue, ProductOptionType> FromEntityWithDefaults(ProductEntity entity)
        {
            BaseConvert(entity);
            return base.FromEntityWithDefaults(entity);
        }

        private void BaseConvert(ProductEntity entity, bool withDefaults = false)
        {
            Id = entity.Id;
            Name = entity.Name;
            Url = entity.Url;
            Type = entity.IdProductType;
            StatusCode = entity.StatusCode;
            Hidden = entity.Hidden;
            IdExternal = entity.IdExternal;
            Skus = new List<SkuDynamic>();
            foreach(var sku in entity.Skus)
            {
                sku.OptionTypes = entity.OptionTypes;
                var skuDynamic = new SkuDynamic();
                if(withDefaults)
                {
                    skuDynamic.FromEntityWithDefaults(sku);
                }
                else
                {
                    skuDynamic.FromEntity(sku);
                }
                Skus.Add(skuDynamic);
            }
            CategoryIds = entity.ProductsToCategories.Select(p => p.IdCategory).ToList();
        }
    }
}
