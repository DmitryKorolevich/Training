using System.Collections.Generic;
using VitalChoice.Ecommerce.Domain.Entities.Base;

namespace VitalChoice.Ecommerce.Domain.Entities.Products
{
    public class Sku : DynamicDataEntity<SkuOptionValue, ProductOptionType>
    {
        public int IdProduct { get; set; }

        public Product Product { get; set; }

        public string Code { get; set; }

        public bool Hidden { get; set; }

        public decimal Price { get; set; }

        public decimal WholesalePrice { get; set; }

        public int Order { get; set; }

        public ICollection<SkuToInventorySku> SkusToInventorySkus { get; set; }
    }
}