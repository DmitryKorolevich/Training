using System.Collections.Generic;
using VitalChoice.Infrastructure.Domain.Dynamic;

namespace VitalChoice.Infrastructure.Domain.Transfer.Orders
{
    public class SkuOrdered
    {
        public SkuOrdered()
        {
            Messages = new List<string>();
        }

        public SkuDynamic Sku { get; set; }
        public ProductDynamic ProductWithoutSkus { get; set; }
        public decimal Amount { get; set; }
        public int Quantity { get; set; }
        public IList<string> Messages { get; set; }
    }

    public class PromoOrdered: SkuOrdered
    {
        public PromoOrdered()
        {
            
        }

        public PromoOrdered(SkuOrdered skuOrdered, PromotionDynamic promo, bool enabled)
        {
            Sku = skuOrdered.Sku;
            ProductWithoutSkus = skuOrdered.ProductWithoutSkus;
            Amount = skuOrdered.Amount;
            Quantity = skuOrdered.Quantity;
            Messages = skuOrdered.Messages;
            Promotion = promo;
            Enabled = enabled;
        }

        public bool Enabled { get; set; }

        public PromotionDynamic Promotion { get; set; }
    }
}