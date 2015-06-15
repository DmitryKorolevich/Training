using VitalChoice.Domain.Entities.eCommerce.Base;

namespace VitalChoice.Domain.Entities.eCommerce.Products
{
    public class ProductOptionType : OptionType
    {
        public int? IdProductType { get; set; }

        public ProductType? ProductType { get; set; }
    }
}