using System.Collections.Generic;
using System.Threading.Tasks;
using VitalChoice.Ecommerce.Domain.Attributes;
using VitalChoice.Ecommerce.Domain.Dynamic;
using VitalChoice.Ecommerce.Domain.Entities.Affiliates;
using VitalChoice.Ecommerce.Domain.Entities.GiftCertificates;
using VitalChoice.Ecommerce.Domain.Entities.Orders;
using VitalChoice.Infrastructure.Domain.Transfer.GiftCertificates;
using VitalChoice.Infrastructure.Domain.Transfer.Orders;

namespace VitalChoice.Infrastructure.Domain.Dynamic
{
    public sealed class OrderRefundDynamic : MappedObject
    {
        public OrderRefundDynamic()
        {
            RefundSkus = new List<RefundSkuOrdered>();
            RefundOrderToGiftCertificates = new List<RefundOrderToGiftCertificateUsed>();
        }

        public int? IdAddedBy { get; set; }

        public OrderStatus? OrderStatus { get; set; }

        public CustomerDynamic Customer { get; set; }

        public OrderDynamic OriginalOrder { get; set; }

        public OrderPaymentMethodDynamic PaymentMethod { get; set; }

        public AddressDynamic ShippingAddress { get; set; }
        
        public DiscountDynamic Discount { get; set; }

        public decimal Total { get; set; }

        public decimal ProductsSubtotal { get; set; }

        public decimal TaxTotal { get; set; }

        public decimal ShippingTotal { get; set; }

        public decimal DiscountTotal { get; set; }

        public int IdOrderSource { get; set; }

        public ICollection<RefundSkuOrdered> RefundSkus { get; set; }

        public ICollection<RefundOrderToGiftCertificateUsed> RefundOrderToGiftCertificates { get; set; }
    }
}