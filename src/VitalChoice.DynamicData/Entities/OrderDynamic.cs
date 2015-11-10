using System.Collections.Generic;
using Newtonsoft.Json;
using VitalChoice.Domain.Attributes;
using VitalChoice.Domain.Entities;
using VitalChoice.Domain.Entities.eCommerce.Customers;
using VitalChoice.Domain.Entities.eCommerce.GiftCertificates;
using VitalChoice.Domain.Entities.eCommerce.Orders;
using VitalChoice.Domain.Entities.eCommerce.Products;
using VitalChoice.Domain.Transfer.Orders;
using VitalChoice.DynamicData.Base;
using VitalChoice.DynamicData.Entities.Transfer;
using VitalChoice.Domain.Entities.eCommerce.Affiliates;

namespace VitalChoice.DynamicData.Entities
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

        [NotLoggedInfo]
        public OrderPaymentMethodDynamic PaymentMethod { get; set; }

        public AddressDynamic ShippingAddress { get; set; }

        public decimal Total { get; set; }

        public decimal ProductsSubtotal { get; set; }

        public decimal TaxTotal { get; set; }

        public decimal ShippingTotal { get; set; }

        public decimal DiscountTotal { get; set; }

        public AffiliateOrderPayment AffiliateOrderPayment { get; set; }
    }
}