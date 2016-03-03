using System;
using System.Collections.Generic;
using VitalChoice.Ecommerce.Domain.Attributes;
using VitalChoice.Ecommerce.Domain.Entities;
using VitalChoice.Ecommerce.Domain.Entities.Orders;

namespace VitalChoice.Ecommerce.Domain.Mail
{
    public class OrderConfirmationEmail : EmailTemplateDataModel
    {
        public OrderConfirmationEmail()
        {
            Skus = new List<SkuEmailItem>();
            PromoSkus = new List<SkuEmailItem>();
        }

        public string PublicHost { get; set; }

        [Map]
        public int Id { get; set; }

        [Map]
        public int IdCustomer { get; set; }

        [Map]
        public DateTime DateCreated { get; set; }

        [Map]
        public int? IdObjectType { get; set; }

        [Map]
        public RecordStatusCode StatusCode { get; set; }

        [Map]
        public OrderStatus? OrderStatus { get; set; }

        [Map]
        public OrderStatus? POrderStatus { get; set; }

        [Map]
        public OrderStatus? NPOrderStatus { get; set; }

        [Map]
        public ShipDelayType? ShipDelayType { get; set; }

        [Map]
        public DateTime? ShipDelayDateP { get; set; }

        [Map]
        public DateTime? ShipDelayDateNP { get; set; }

        [Map]
        public DateTime? ShipDelayDate { get; set; }

        [Map]
        public int? AutoShipFrequency { get; set; }

        public string AutoShipFrequencyProductName { get; set; }
        
        [Map]
        public string GiftMessage { get; set; }

        [Map]
        public string DeliveryInstructions { get; set; }

        public AddressEmailItem BillToAddress { get; set; }

        public AddressEmailItem ShipToAddress { get; set; }

        public IList<SkuEmailItem> Skus { get; set; }

        public IList<SkuEmailItem> PromoSkus { get; set; }

        [Map]
        public decimal ShippingTotal { get; set; }

        [Map]
        public decimal ProductsSubtotal { get; set; }

        [Map]
        public decimal DiscountTotal { get; set; }

        [Map]
        public decimal TaxTotal { get; set; }

        [Map]
        public decimal Total { get; set; }

        public decimal GiftCertificatesTotal { get; set; }

        public string PaymentTypeMessage { get; set; }
    }
}