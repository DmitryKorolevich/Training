using System;
using System.Collections.Generic;
using VitalChoice.Ecommerce.Domain.Entities.Addresses;
using VitalChoice.Ecommerce.Domain.Entities.Discounts;
using VitalChoice.Ecommerce.Domain.Entities.GiftCertificates;
using VitalChoice.Ecommerce.Domain.Entities.Products;

namespace VitalChoice.Ecommerce.Domain.Entities.Checkout
{
    public class CartAdditionalShipmentToGiftCertificate : Entity
    {
        public int IdCartAdditionalShipment { get; set; }

        public int IdGiftCertificate { get; set; }

        public GiftCertificate GiftCertificate { get; set; }
    }
}