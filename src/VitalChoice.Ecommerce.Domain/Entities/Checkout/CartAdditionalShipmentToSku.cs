using System;
using System.Collections.Generic;
using VitalChoice.Ecommerce.Domain.Entities.Addresses;
using VitalChoice.Ecommerce.Domain.Entities.Discounts;
using VitalChoice.Ecommerce.Domain.Entities.Products;

namespace VitalChoice.Ecommerce.Domain.Entities.Checkout
{
    public class CartAdditionalShipmentToSku : Entity
    {
        public int IdCartAdditionalShipment { get; set; }

        public int IdSku { get; set; }

        public Sku Sku { get; set; }

        public decimal Amount { get; set; }

        public int Quantity { get; set; }
    }
}