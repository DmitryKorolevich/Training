using System.Collections.Generic;
using VitalChoice.Ecommerce.Domain.Attributes;
using VitalChoice.Ecommerce.Domain.Dynamic;
using VitalChoice.Ecommerce.Domain.Entities.Affiliates;
using VitalChoice.Ecommerce.Domain.Entities.Orders;
using VitalChoice.Infrastructure.Domain.Transfer.Orders;

namespace VitalChoice.Infrastructure.Domain.Dynamic
{
    public sealed class OrderDynamic : MappedObject
    {
        public OrderDynamic()
        {
            Skus = new List<SkuOrdered>();
            GiftCertificates = new List<GiftCertificateInOrder>();
        }

        public int? IdAddedBy { get; set; }

        public OrderStatus OrderStatus { get; set; }

        //[NotLoggedInfo]
        public CustomerDynamic Customer { get; set; }

        public ICollection<SkuOrdered> Skus { get; set; }

        public DiscountDynamic Discount { get; set; }

        public ICollection<GiftCertificateInOrder> GiftCertificates { get; set; }

        public OrderPaymentMethodDynamic PaymentMethod { get; set; }

        public AddressDynamic ShippingAddress { get; set; }

        public decimal Total { get; set; }

        public decimal ProductsSubtotal { get; set; }

        public decimal TaxTotal { get; set; }

        public decimal ShippingTotal { get; set; }

        public decimal DiscountTotal { get; set; }

        public AffiliateOrderPayment AffiliateOrderPayment { get; set; }

        public bool IsHealthwise { get; set; }
    }
}