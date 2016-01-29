using System;
using VitalChoice.Infrastructure.Domain.Dynamic;

namespace VitalChoice.Infrastructure.Domain.Transfer.Cart
{
    public class CustomerCartOrder
    {
        public Guid CartUid { get; set; }

        public OrderDynamic Order { get; set; }
    }
}
