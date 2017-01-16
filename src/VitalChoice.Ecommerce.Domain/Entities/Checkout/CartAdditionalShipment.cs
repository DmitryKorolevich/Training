using System;
using System.Collections.Generic;
using VitalChoice.Ecommerce.Domain.Entities.Addresses;
using VitalChoice.Ecommerce.Domain.Entities.Discounts;

namespace VitalChoice.Ecommerce.Domain.Entities.Checkout
{
    public class CartAdditionalShipment : Entity
    {
        public int IdOrder { get; set; }

        public string Name { get; set; }

        public bool IsGiftOrder { get; set; }

        public string GiftMessage { get; set; }

        public int IdShippingAddress { get; set; }

        public OrderAddress ShippingAddress { get; set; }

        public ICollection<CartAdditionalShipmentToSku> Skus { get; set; }
    }
}