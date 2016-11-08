using System;
using System.Linq;
using System.Collections.Generic;
using VitalChoice.Validation.Models;
using VitalChoice.Validation.Attributes;
using VC.Admin.Validators.Order;
using VC.Admin.Models.Customer;
using VitalChoice.Ecommerce.Domain.Attributes;
using VitalChoice.Ecommerce.Domain.Entities;
using VitalChoice.Ecommerce.Domain.Entities.Orders;
using VitalChoice.Infrastructure.Domain.Transfer.Orders;
using VitalChoice.Infrastructure.Domain.Transfer.Shipping;
using VitalChoice.Infrastructure.Domain.Dynamic;
using VitalChoice.Ecommerce.Domain.Entities.Customers;
using VitalChoice.Ecommerce.Domain.Entities.Payment;
using VitalChoice.Infrastructure.Domain.Entities.Users;
using VitalChoice.Interfaces.Services;
using VitalChoice.DynamicData.Interfaces;
using VitalChoice.Ecommerce.Domain.Entities.Addresses;
using System.Threading.Tasks;
using VC.Admin.Models.Customers;
using VitalChoice.Interfaces.Services.Settings;
using VitalChoice.Business.Helpers;
using VitalChoice.Infrastructure.Domain.Constants;

namespace VC.Admin.Models.Orders
{
    public class OrderInvoiceModel : BaseModel
    {
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

        public bool ShowVitalChoiceTaxId { get; set; }

        public string InvoiceTypeName { get; set; }

        public string IdEditAgent { get; set; }

        public AddressModel BillingAddress { get; set; }

        public AddressModel ShippingAddress { get; set; }

        public string PaymentTypeName { get; set; }

        public string CreditCardInformationRow { get; set; }

        public bool ShowWholesaleShortView { get; set; }

        public bool ShowWholesaleNormalView { get; set; }

        public string OACFOB { get; set; }

        public string PreferredShipMethodName { get; set; }

        public string OACTerms { get; set; }

        public ICollection<SkuOrderedManageModel> SkuOrdereds { get; set; }

        public ICollection<SkuOrderedManageModel> PromoSkuOrdereds { get; set; }

        public bool ShowTrackingView { get; set; }

        public DateTime? DateShipped {get;set; }

        public DateTime? PDateShipped { get; set; }

        public DateTime? NPDateShipped { get; set; }

        public string ShipVia { get; set; }

        public string PShipVia { get; set; }

        public string NPShipVia { get; set; }

        public ICollection<TrackingInvoiceItemModel> TrackingEntities { get; set; }

        public ICollection<TrackingInvoiceItemModel> PTrackingEntities { get; set; }

        public ICollection<TrackingInvoiceItemModel> NPTrackingEntities { get; set; }

        public ICollection<GCInvoiceItemModel> GCs { get; set; }

        public decimal? RefundedManualOverride { get; set; }

        public decimal ShippingSurcharge { get; set; }

        //Only shipping with ovveride and without surcharge part
        public decimal TotalShipping { get; set; }

        public string PDFUrl { get; set; }

        public OrderInvoiceModel()
        {
            SkuOrdereds = new List<SkuOrderedManageModel>();
            PromoSkuOrdereds = new List<SkuOrderedManageModel>();
            TrackingEntities = new List<TrackingInvoiceItemModel>();
            GCs = new List<GCInvoiceItemModel>();
            TrackingEntities = new List<TrackingInvoiceItemModel>();
            PTrackingEntities = new List<TrackingInvoiceItemModel>();
            NPTrackingEntities = new List<TrackingInvoiceItemModel>();
        }
    }
}