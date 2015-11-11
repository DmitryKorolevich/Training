using VitalChoice.Domain.Entities.eCommerce.Base;

namespace VitalChoice.Domain.Entities.eCommerce.Products
{
    public class SkuOptionValue : OptionValue<ProductOptionType>
    {
        public int IdSku { get; set; }
    }
}