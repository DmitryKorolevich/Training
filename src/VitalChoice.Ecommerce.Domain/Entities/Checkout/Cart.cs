using System;
using System.Collections.Generic;
using VitalChoice.Ecommerce.Domain.Entities.Discounts;

namespace VitalChoice.Ecommerce.Domain.Entities.Checkout
{
    public class Cart: Entity
    {
        public Guid CartUid { get; set; }

        public int? IdCustomer { get; set; }

        public int? IdOrder { get; set; }

        public int? IdDiscount { get; set; }

        public Discount Discount { get; set; }

        public ICollection<CartToGiftCertificate> GiftCertificates { get; set; }

        public ICollection<CartToSku> Skus { get; set; }
    }
}
