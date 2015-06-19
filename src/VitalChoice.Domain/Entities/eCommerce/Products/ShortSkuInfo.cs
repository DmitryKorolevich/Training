using VitalChoice.Domain.Entities.eCommerce.Base;

namespace VitalChoice.Domain.Entities.eCommerce.Products
{
    public class ShortSkuInfo : Entity
    {
        public int IdProduct { get; set; }

        public string Code { get; set; }

        public string ProductName { get; set; }

        public decimal Price { get; set; }

        public decimal WholesalePrice { get; set; }

        public ShortSkuInfo(Sku sku)
        {
            if(sku!=null && sku.Product!=null)
            {
                Id = sku.Id;
                IdProduct = sku.IdProduct;
                ProductName = sku.Product.Name;
                Code = sku.Code;
                Price = sku.Price;
                WholesalePrice = sku.WholesalePrice;
            }
        }
    }
}