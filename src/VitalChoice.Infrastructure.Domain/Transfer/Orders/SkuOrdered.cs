using System.Collections.Generic;
using VitalChoice.Ecommerce.Domain.Entities.GiftCertificates;
using VitalChoice.Ecommerce.Domain.Exceptions;
using VitalChoice.Infrastructure.Domain.Dynamic;
using VitalChoice.Infrastructure.Domain.Transfer.Contexts;

namespace VitalChoice.Infrastructure.Domain.Transfer.Orders
{
    public class SkuOrdered : ItemOrdered
    {
        public SkuOrdered()
        {
            Messages = new List<MessageInfo>();
        }

        public ICollection<GiftCertificate> GcsGenerated { get; set; }
        public IList<MessageInfo> Messages { get; set; }
    }

    public sealed class PromoOrdered: SkuOrdered
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