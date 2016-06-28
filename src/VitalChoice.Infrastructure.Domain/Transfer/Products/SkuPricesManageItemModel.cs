using VitalChoice.Ecommerce.Domain.Entities;
using VitalChoice.Ecommerce.Domain.Entities.Products;

namespace VitalChoice.Infrastructure.Domain.Transfer.Products
{
    public class SkuPricesManageItemModel
    {
        public int Id { get; set; }

        public int IdProduct { get; set; }

        public string ProductName { get; set; }

        public string Code { get; set; }

        public RecordStatusCode StatusCode { get; set; }

        public bool Hidden { get; set; }

        public decimal Price { get; set; }

        public decimal WholesalePrice { get; set; }
    }
}
