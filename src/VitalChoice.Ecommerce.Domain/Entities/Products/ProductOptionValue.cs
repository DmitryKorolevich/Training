using VitalChoice.Ecommerce.Domain.Entities.Base;

namespace VitalChoice.Ecommerce.Domain.Entities.Products
{
    public class ProductOptionValue : OptionValue<ProductOptionType>
    {
        public int IdProduct { get; set; }
    }
}