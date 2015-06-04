using VitalChoice.Domain.Entities.eCommerce.Base;

namespace VitalChoice.Domain.Entities.eCommerce.Product
{
    public class ProductOptionValue : OptionValue<ProductOptionType>
    {
        public int? IdProduct { get; set; }

        public int? IdSku { get; set; }
    }
}