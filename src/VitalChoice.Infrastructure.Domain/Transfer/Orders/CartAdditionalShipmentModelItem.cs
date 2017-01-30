using System;
using System.Collections.Generic;
using VitalChoice.Ecommerce.Domain.Entities.Orders;
using VitalChoice.Infrastructure.Domain.Dynamic;

namespace VitalChoice.Infrastructure.Domain.Transfer.Orders
{
    public class CartAdditionalShipmentModelItem
    {
        public CartAdditionalShipmentModelItem()
        {
            Skus = new List<SkuOrdered>();
            GiftCertificateIds = new List<int>();
        }

        public int Id { get; set; }

        public string Name { get; set; }

        public bool IsGiftOrder { get; set; }

        public string GiftMessage { get; set; }

        public string DiscountCode { get; set; }

        public DateTime? ShipDelayDate { get; set; }

        public int? ShippingUpgradeP { get; set; }

        public int? ShippingUpgradeNP { get; set; }

        public AddressDynamic ShippingAddress { get; set; }

        public ICollection<SkuOrdered> Skus { get; set; }

        public ICollection<int> GiftCertificateIds { get; set; }
    }
}