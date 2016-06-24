using System.Collections.Generic;
using VitalChoice.Ecommerce.Domain.Entities.Orders;
using VitalChoice.Ecommerce.Domain.Exceptions;
using VitalChoice.Infrastructure.Domain.Dynamic;
using VitalChoice.Infrastructure.Domain.Transfer.Contexts;

namespace VitalChoice.Infrastructure.Domain.Transfer.Orders
{
    public class RefundSkuOrdered : ItemOrdered
    {
        public RefundSkuOrdered()
        {
            Messages = new List<MessageInfo>();
        }

        public RedeemType Redeem { get; set; }

        public override decimal Amount => RefundValue*(decimal) RefundPercent/100;

        public decimal RefundPrice { get; set; }

        public decimal RefundValue { get; set; }

        public double RefundPercent { get; set; }

        public IList<MessageInfo> Messages { get; set; }
    }
}