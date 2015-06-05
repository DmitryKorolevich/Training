using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VitalChoice.Domain.Entities.eCommerce.Product;
using VitalChoice.DynamicData;

namespace VitalChoice.Business.Entities
{
    public class ProductDynamic : DynamicObject<ProductEntity, ProductOptionValue, ProductOptionType>
    {
        public override ProductEntity ToEntity()
        {
            return base.ToEntity();
        }

        public override IDynamicEntity<ProductEntity, ProductOptionValue, ProductOptionType> FromEntity(ProductEntity entity)
        {
            return base.FromEntity(entity);
        }

        public override IDynamicEntity<ProductEntity, ProductOptionValue, ProductOptionType> FromEntityWithDefaults(ProductEntity entity)
        {
            return base.FromEntityWithDefaults(entity);
        }

        public string Name { get; set; }

        public string Url { get; set; }
    }
}
