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
        }

        public int Id { get; set; }

        public string Name { get; set; }

        public AddressDynamic ShippingAddress { get; set; }

        public ICollection<SkuOrdered> Skus { get; set; }
    }
}