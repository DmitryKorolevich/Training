using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VitalChoice.Infrastructure.Domain.Dynamic;
using VitalChoice.Infrastructure.Domain.Transfer.Orders;

namespace VitalChoice.Infrastructure.Domain.Transfer.Cart
{
    public class CustomerCart
    {
        public Guid CartUid { get; set; }

        public OrderDynamic Order { get; set; }

        public DiscountDynamic Discount { get; set; }

        public ICollection<GiftCertificateInOrder> GiftCertificates { get; set; }

        public ICollection<SkuOrdered> Skus { get; set; }
    }
}
