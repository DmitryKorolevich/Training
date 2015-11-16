using VitalChoice.Infrastructure.Domain.Content.Products;
using VitalChoice.Infrastructure.Domain.Dynamic;

namespace VitalChoice.Infrastructure.Domain.Transfer.Products
{
    public class ProductContentTransferEntity
    {
        public ProductContent ProductContent { get; set; }

        public ProductDynamic ProductDynamic { get; set; }
    }
}
