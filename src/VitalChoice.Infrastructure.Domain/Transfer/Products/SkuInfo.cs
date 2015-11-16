using VitalChoice.Ecommerce.Domain.Entities.Products;

namespace VitalChoice.Infrastructure.Domain.Transfer.Products
{
    public class SkuInfo
    {
        public int Id { get; set; }

        public ProductType IdProductType { get; set; }
    }
}
