using VitalChoice.Ecommerce.Domain.Entities.Base;

namespace VitalChoice.Ecommerce.Domain.Entities.Products
{
    public class SkuOptionValue : OptionValue<SkuOptionType>
    {
        public int IdSku { get; set; }
    }
}