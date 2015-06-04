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
        public string Name { get; set; }

        public string Url { get; set; }
    }
}
