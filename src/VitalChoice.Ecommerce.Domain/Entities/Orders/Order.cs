using System.Collections.Generic;
using VitalChoice.Ecommerce.Domain.Entities.Addresses;
using VitalChoice.Ecommerce.Domain.Entities.Affiliates;
using VitalChoice.Ecommerce.Domain.Entities.Base;
using VitalChoice.Ecommerce.Domain.Entities.Customers;
using VitalChoice.Ecommerce.Domain.Entities.Discounts;
using VitalChoice.Ecommerce.Domain.Entities.GiftCertificates;
using VitalChoice.Ecommerce.Domain.Entities.Payment;

namespace VitalChoice.Ecommerce.Domain.Entities.Orders
{
    public class Order : DynamicDataEntity<OrderOptionValue, OrderOptionType>
    {
        public int? IdAddedBy { get; set; }

        public OrderStatus? OrderStatus { get; set; }

        public OrderStatus? POrderStatus { get; set; }

        public OrderStatus? NPOrderStatus { get; set; }

        public int IdCustomer { get; set; }

        public Customer Customer { get; set; }

        public ICollection<OrderToSku> Skus { get; set; }

        public ICollection<OrderToPromo> PromoSkus { get; set; }

        public int? IdDiscount { get; set; }

        public ICollection<OrderToGiftCertificate> GiftCertificates { get; set; }

        public int? IdPaymentMethod { get; set; }

        public int? IdShippingAddress { get; set; }

        public OrderAddress ShippingAddress { get; set; }

        public OrderPaymentMethod PaymentMethod { get; set; }

        public decimal Total { get; set; }

        public decimal ProductsSubtotal { get; set; }

        public decimal TaxTotal { get; set; }

        public decimal ShippingTotal { get; set; }

        public decimal DiscountTotal { get; set; }

        public Discount Discount { get; set; }

        public AffiliateOrderPayment AffiliateOrderPayment { get; set; }
    }
}