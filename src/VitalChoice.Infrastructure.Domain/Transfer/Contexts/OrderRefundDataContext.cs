using System.Collections.Generic;
using VitalChoice.Ecommerce.Domain.Entities.Addresses;
using VitalChoice.Ecommerce.Domain.Entities.Orders;
using VitalChoice.Ecommerce.Domain.Exceptions;
using VitalChoice.Ecommerce.Domain.Helpers;
using VitalChoice.Ecommerce.Domain.Transfer;
using VitalChoice.Infrastructure.Domain.Dynamic;
using VitalChoice.Infrastructure.Domain.Transfer.GiftCertificates;
using VitalChoice.Infrastructure.Domain.Transfer.Orders;
using VitalChoice.Infrastructure.Domain.Transfer.Shipping;
using VitalChoice.Workflow.Base;

namespace VitalChoice.Infrastructure.Domain.Transfer.Contexts
{
    public class OrderRefundDataContext : ComputableDataContext
    {
        public OrderRefundDataContext()
        {
            Messages = new List<MessageInfo>();
        }

        public OrderRefundDynamic Order { get; set; }

        public decimal ShippingTotal { get; set; }

        public decimal TotalShipping { get; set; }

        public decimal ProductsSubtotal { get; set; }

        public decimal DiscountTotal { get; set; }

        public decimal DiscountedSubtotal { get; set; }

        public string DiscountMessage { get; set; }

        public decimal TaxTotal { get; set; }

        public decimal Total { get; set; }

        public bool ShippingRefunded { get; set; }

        public decimal ManualRefundOverride { get; set; }

        public ICollection<RefundSkuOrdered> RefundSkus { get; set; }

        public ICollection<RefundOrderToGiftCertificateUsed> RefundOrderToGiftCertificates { get; set; }

        public ICollection<MessageInfo> Messages { get; set; }
    }
}