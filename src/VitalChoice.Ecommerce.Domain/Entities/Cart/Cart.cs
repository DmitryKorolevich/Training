using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VitalChoice.Ecommerce.Domain.Entities.Discounts;
using VitalChoice.Ecommerce.Domain.Entities.GiftCertificates;
using VitalChoice.Ecommerce.Domain.Entities.Orders;

namespace VitalChoice.Ecommerce.Domain.Entities.Cart
{
    public class Cart: Entity
    {
        public Guid CartUid { get; set; }

        public int? IdCustomer { get; set; }

        public int? IdOrder { get; set; }

        public Order Order { get; set; }

        public virtual int? IdDiscount { get; set; }

        public virtual Discount Discount { get; set; }

        public virtual ICollection<OrderToGiftCertificate> GiftCertificates { get; set; }

        public virtual ICollection<OrderToSku> Skus { get; set; }
    }
}
