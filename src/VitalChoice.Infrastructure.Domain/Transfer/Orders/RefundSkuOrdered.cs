using System.Collections.Generic;
using VitalChoice.Ecommerce.Domain.Entities.Orders;
using VitalChoice.Infrastructure.Domain.Dynamic;

namespace VitalChoice.Infrastructure.Domain.Transfer.Orders
{
    public class RefundSkuOrdered
    {
        public RefundSkuOrdered()
        {
            Messages = new List<string>();
        }

        public SkuDynamic Sku { get; set; }

        public RedeemType Redeem { get; set; }

        public int Quantity { get; set; }

        public decimal RefundPrice { get; set; }

        public decimal RefundValue { get; set; }

        public double RefundPercent { get; set; }

        public IList<string> Messages { get; set; }
    }
}