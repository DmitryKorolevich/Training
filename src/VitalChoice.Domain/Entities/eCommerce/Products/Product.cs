using System.Collections.Generic;
using VitalChoice.Domain.Entities.eCommerce.Base;

namespace VitalChoice.Domain.Entities.eCommerce.Products
{
    public class Product : DynamicDataEntity<ProductOptionValue, ProductOptionType>
    {
        public string Name { get; set; }

        public string Url { get; set; }

        public bool Hidden { get; set; }

        public ProductType IdProductType { get; set; }

        public ICollection<Sku> Skus { get; set; }

        public ICollection<ProductToCategory> ProductsToCategories { get; set; }
    }
}