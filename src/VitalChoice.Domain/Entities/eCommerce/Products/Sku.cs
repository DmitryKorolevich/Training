using VitalChoice.Domain.Entities.eCommerce.Base;

namespace VitalChoice.Domain.Entities.eCommerce.Products
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
    }
}