using System.Collections.Generic;
using VitalChoice.Domain.Entities;
using VitalChoice.Domain.Entities.eCommerce.Customers;
using VitalChoice.Domain.Entities.eCommerce.GiftCertificates;
using VitalChoice.Domain.Entities.eCommerce.Orders;
using VitalChoice.Domain.Entities.eCommerce.Products;
using VitalChoice.DynamicData.Base;

namespace VitalChoice.DynamicData.Entities
{
    public sealed class OrderDynamic : MappedObject
    {
        public OrderStatus OrderStatus { get; set; }

        public int IdCustomer { get; set; }

        public Customer Customer { get; set; }

        public ICollection<OrderToSku> Skus { get; set; }

        public int? IdDiscount { get; set; }

        public DiscountDynamic Discount { get; set; }

        public ICollection<OrderToGiftCertificate> GiftCertificates { get; set; }

        public int? IdPaymentMethod { get; set; }

        public OrderPaymentMethodDynamic PaymentMethod { get; set; }

        public int? IdShippingAddress { get; set; }

        public OrderAddressDynamic Address { get; set; }

        public decimal Total { get; set; }

        public decimal ProductsSubtotal { get; set; }

        public decimal TaxTotal { get; set; }

        public decimal ShippingTotal { get; set; }

        public decimal DiscountTotal { get; set; }
    }
}