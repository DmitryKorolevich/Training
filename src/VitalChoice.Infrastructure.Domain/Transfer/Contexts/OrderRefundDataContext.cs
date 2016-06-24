using System.Collections.Generic;
using VitalChoice.Ecommerce.Domain.Exceptions;
using VitalChoice.Infrastructure.Domain.Dynamic;
using VitalChoice.Infrastructure.Domain.Transfer.Orders;

namespace VitalChoice.Infrastructure.Domain.Transfer.Contexts
{
    public class OrderRefundDataContext : BaseOrderContext<RefundSkuOrdered>
    {
        public OrderRefundDataContext()
        {
            Messages = new List<MessageInfo>();
            SplitInfo = new SplitInfoBase<RefundSkuOrdered>(() => RefundSkus);
        }

        public OrderRefundDynamic RefundOrder { get; set; }

        public override OrderDynamic Order => RefundOrder.OriginalOrder;

        public decimal TotalShipping { get; set; }

        public decimal ProductsSubtotal { get; set; }

        public decimal DiscountedSubtotal { get; set; }

        public string DiscountMessage { get; set; }

        public decimal TaxTotal { get; set; }

        public decimal Total { get; set; }

        public decimal AutoTotal { get; set; }

        public bool ShippingRefunded { get; set; }

        public decimal ManualShippingTotal { get; set; }

        public decimal ManualRefundOverride { get; set; }

        public decimal RefundGCsUsedOnOrder { get; set; }

        public ICollection<RefundSkuOrdered> RefundSkus { get; set; }

        public ICollection<RefundOrderToGiftCertificateUsed> RefundOrderToGiftCertificates { get; set; }

        public ICollection<MessageInfo> Messages { get; set; }

        public SplitInfoBase<RefundSkuOrdered> SplitInfo { get; set; }

        public override IEnumerable<RefundSkuOrdered> ItemsOrdered => RefundSkus;

        public override SplitInfoBase<RefundSkuOrdered> BaseSplitInfo => SplitInfo;
    }
}