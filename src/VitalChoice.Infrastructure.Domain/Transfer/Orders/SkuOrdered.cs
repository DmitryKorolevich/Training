using System.Collections.Generic;
using VitalChoice.Ecommerce.Domain.Entities.GiftCertificates;
using VitalChoice.Ecommerce.Domain.Exceptions;
using VitalChoice.Infrastructure.Domain.Dynamic;

namespace VitalChoice.Infrastructure.Domain.Transfer.Orders
{
    public class SkuOrdered
    {
        public SkuOrdered()
        {
            Messages = new List<MessageInfo>();
        }

        public SkuDynamic Sku { get; set; }
        public ICollection<GiftCertificate> GcsGenerated { get; set; }
        public decimal Amount { get; set; }
        public int Quantity { get; set; }
        public IList<MessageInfo> Messages { get; set; }
    }

    public class PromoOrdered: SkuOrdered
    {
        public PromoOrdered()
        {
            
        }

        public PromoOrdered(SkuOrdered skuOrdered, PromotionDynamic promo, bool enabled)
        {
            GcsGenerated = skuOrdered.GcsGenerated;
            Sku = skuOrdered.Sku;
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