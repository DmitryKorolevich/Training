using VitalChoice.Domain.Entities.eCommerce.Base;

namespace VitalChoice.Domain.Entities.eCommerce.Product
{
    public class ProductOptionType : OptionType
    {
        public ProductType? IdProductType { get; set; }
    }
}
