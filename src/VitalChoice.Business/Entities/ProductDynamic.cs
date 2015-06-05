using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VitalChoice.Domain.Entities.eCommerce.Product;
using VitalChoice.DynamicData;

namespace VitalChoice.Business.Entities
{
    public class ProductDynamic : DynamicObject<Product, ProductOptionValue, ProductOptionType>
    {
        public override Product ToEntity()
        {
            return base.ToEntity();
        }

        public override IDynamicEntity<Product, ProductOptionValue, ProductOptionType> FromEntity(Product entity)
        {
            return base.FromEntity(entity);
        }

        public override IDynamicEntity<Product, ProductOptionValue, ProductOptionType> FromEntityWithDefaults(Product entity)
        {
            return base.FromEntityWithDefaults(entity);
        }

        public string Name { get; set; }

        public string Url { get; set; }
    }
}
