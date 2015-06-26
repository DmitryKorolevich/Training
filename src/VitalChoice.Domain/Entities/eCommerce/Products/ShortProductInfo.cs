using VitalChoice.Domain.Entities.eCommerce.Base;

namespace VitalChoice.Domain.Entities.eCommerce.Products
{
    public class ShortProductInfo : Entity
    {
        public string ProductName { get; set; }

        public ShortProductInfo(Product product)
        {
            if(product != null)
            {
                Id = product.Id;
                ProductName = product.Name;
            }
        }
    }
}