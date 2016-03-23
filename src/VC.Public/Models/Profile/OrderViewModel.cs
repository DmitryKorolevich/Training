using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using VC.Public.Models.Cart;
using VitalChoice.Ecommerce.Domain.Attributes;
using VitalChoice.Ecommerce.Domain.Entities;
using VitalChoice.Ecommerce.Domain.Entities.Orders;
using VitalChoice.Infrastructure.Domain.Transfer.Shipping;

namespace VC.Public.Models.Profile
{
    public class GCInvoiceEntity
    {
        public string Code { get; set; }

        public decimal Amount { get; set; }
    }

    public class OrderViewModel
    {
        [Map]
        public int Id { get; set; }

        [Map]
        public int IdCustomer { get; set; }

        [Map]
        public DateTime DateCreated { get; set; }

        public AddressModel Shipping { get; set; }

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
        public bool GiftOrder { get; set; }

        [Map]
        public bool MailOrder { get; set; }

        [Map]
        public string PoNumber { get; set; }

        [Map]
        public string KeyCode { get; set; }

        [Map]
        public int? AutoShipFrequency { get; set; }

        [Map]
        public ShipDelayType? ShipDelayType { get; set; }

        [Map]
        public DateTime? ShipDelayDateP { get; set; }

        [Map]
        public DateTime? ShipDelayDateNP { get; set; }

        [Map]
        public DateTime? ShipDelayDate { get; set; }

        [Map]
        public string OrderNotes { get; set; }

        [Map]
        public string GiftMessage { get; set; }

        [Map]
        public string DeliveryInstructions { get; set; }

        [Map]
        public decimal AlaskaHawaiiSurcharge { get; set; }

        [Map]
        public decimal CanadaSurcharge { get; set; }

        [Map]
        public decimal StandardShippingCharges { get; set; }

        [Map]
        public ShippingUpgradeOption? ShippingUpgradeP { get; set; }

        [Map]
        public ShippingUpgradeOption? ShippingUpgradeNP { get; set; }

        [Map]
        public decimal SurchargeOverride { get; set; }

        [Map]
        public decimal ShippingOverride { get; set; }

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
        
        public int? IdPaymentMethodType { get; set; }

        public IList<KeyValuePair<string, string>> BillToAddress { get; set; }

        public IList<KeyValuePair<string, string>> CreditCardDetails { get; set; }

        public IList<KeyValuePair<string, string>> ShipToAddress { get; set; }

        public IList<CartSkuModel> Skus { get; set; }

        public IList<CartSkuModel> PromoSkus { get; set; }

        public decimal ShippingSurcharge { get; set; }

        public decimal GiftCertificatesTotal { get; set; }

        public string DiscountCode { get; set; }

        public string DiscountCodeMessage { get; set; }

        public ICollection<GCInvoiceEntity> GCs { get; set; }

        //Only shipping with ovveride and without surcharge part
        public decimal TotalShipping { get; set; }

        public OrderViewModel()
        {
            BillToAddress = new List<KeyValuePair<string, string>>();
            CreditCardDetails = new List<KeyValuePair<string, string>>();
            ShipToAddress = new List<KeyValuePair<string, string>>();
            Skus = new List<CartSkuModel>();
            PromoSkus = new List<CartSkuModel>();
        }
    }
}
