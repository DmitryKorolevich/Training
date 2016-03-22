using System.Collections;
using System.Collections.Generic;
using VitalChoice.Ecommerce.Domain.Entities.GiftCertificates;
using VitalChoice.Ecommerce.Domain.Entities.Products;

namespace VitalChoice.Ecommerce.Domain.Entities.Orders
{
    public class OrderToSku : Entity
    {
        public int IdSku { get; set; }
        public Sku Sku { get; set; }
        public ICollection<GiftCertificate> GeneratedGiftCertificates { get; set; }
        public int IdOrder { get; set; }
        public Order Order { get; set; }
        public decimal Amount { get; set; }
        public int Quantity { get; set; }
        public ICollection<OrderToSkuToInventorySku> InventorySkus { get; set; }
    }
}