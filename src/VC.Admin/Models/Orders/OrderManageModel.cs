using System;
using System.Collections.Generic;
using VitalChoice.Validation.Models;
using VitalChoice.Validation.Attributes;
using VC.Admin.Validators.Order;
using VC.Admin.Models.Customer;
using VC.Admin.Models.Products;
using VitalChoice.Ecommerce.Domain.Attributes;
using VitalChoice.Ecommerce.Domain.Entities;
using VitalChoice.Ecommerce.Domain.Entities.Orders;
using VitalChoice.Infrastructure.Domain.Transfer.Orders;
using VitalChoice.Infrastructure.Domain.Transfer.Shipping;

namespace VC.Admin.Models.Orders
{
    public class SkuOrderedManageModel : BaseModel
    {
        public int? Id { get; set; }

        public string Code { get; set; }

        public int? IdProductType { get; set; }

        public string ProductName { get; set; }

        public decimal? Price { get; set; }

        public int? QTY { get; set; }

        public bool? Promo { get; set; }

        public decimal? Amount { get; set; }

        public bool AutoShipProduct { get; set; }

        public bool AutoShipFrequency1 { get; set; }

        public bool AutoShipFrequency2 { get; set; }

        public bool AutoShipFrequency3 { get; set; }

        public bool AutoShipFrequency6 { get; set; }

        public IList<string> Messages { get; set; }

        public SkuOrderedManageModel(SkuOrdered model)
        {
            if (model != null)
            {
                if (model.ProductWithoutSkus != null)
                {
                    ProductName = $"{model.ProductWithoutSkus.Name} {model.ProductWithoutSkus.SafeData.SubTitle} ({model.Sku.SafeData.QTY})";
                    IdProductType = model.ProductWithoutSkus.IdObjectType;
                }
                QTY = model.Quantity;
                Price = model.Amount;
                Amount = Price * QTY;

                if (model.Sku != null)
                {
                    Id = model.Sku.Id;
                    Code = model.Sku.Code;
                    AutoShipProduct = model.Sku.DictionaryData.ContainsKey("AutoShipProduct") ? model.Sku.Data.AutoShipProduct : false;
                    AutoShipFrequency1 = model.Sku.DictionaryData.ContainsKey("AutoShipFrequency1") ? model.Sku.Data.AutoShipFrequency1 : false;
                    AutoShipFrequency2 = model.Sku.DictionaryData.ContainsKey("AutoShipFrequency2") ? model.Sku.Data.AutoShipFrequency2 : false;
                    AutoShipFrequency3 = model.Sku.DictionaryData.ContainsKey("AutoShipFrequency3") ? model.Sku.Data.AutoShipFrequency3 : false;
                    AutoShipFrequency6 = model.Sku.DictionaryData.ContainsKey("AutoShipFrequency6") ? model.Sku.Data.AutoShipFrequency6 : false;
                }
                Messages = model.Messages;
            }
        }
    }

    public class PromoSkuOrderedManageModel : SkuOrderedManageModel
    {
        public bool IsEnabled { get; set; }

        public bool IsAllowDisable { get; set; }

        public PromoSkuOrderedManageModel(SkuOrdered model) : base(model)
        {
            if (model != null)
            {
                IsEnabled = true;
                IsAllowDisable = true;
            }
        }
    }

    [ApiValidator(typeof(OrderManageModelValidator))]
    public class OrderManageModel : BaseModel
    {
        [Map]
        public int Id { get; set; }

        [Map]
        public int IdCustomer { get; set; }

        [Map]
        public DateTime DateCreated { get; set; }

        [DirectLocalized("Customer")]
        [Map]
        public AddUpdateCustomerModel Customer { get; set; }
        
        //Only for adding a new one 
        public bool UpdateShippingAddressForCustomer {get; set;}

        //Only for adding a new one 
        public bool UpdateCardForCustomer { get; set; }

        //Only for adding a new one 
        public bool UpdateOACForCustomer { get; set; }

        //Only for adding a new one 
        public bool UpdateCheckForCustomer { get; set; }

        //Only for adding a new one 
        public bool UpdateWireTransferForCustomer { get; set; }

        //Only for adding a new one 
        public bool UpdateMarketingForCustomer { get; set; }

        //Only for adding a new one 
        public bool UpdateVCWellnessForCustomer { get; set; }

        //Only for edit
        [DirectLocalized("Shipping")]
        public AddressModel Shipping { get; set; }

        //Only for edit
        [DirectLocalized("Credit Card")]
        public CreditCardModel CreditCard { get; set; }

        //Only for edit
        [DirectLocalized("Check")]
        public CheckPaymentModel Check { get; set; }

        //Only for edit
        [DirectLocalized("On Approved Credit")]
        public OacPaymentModel Oac { get; set; }

        //Only for edit
        [DirectLocalized("Wire Transfer")]
        public WireTransferPaymentModel WireTransfer { get; set; }

        //Only for edit
        [DirectLocalized("Marketing")]
        public MarketingPaymentModel Marketing { get; set; }

        //Only for edit
        [DirectLocalized("VC Wellness Employee Program")]
        public VCWellnessEmployeeProgramPaymentModel VCWellness { get; set; }

        [DirectLocalized("Payment Method")]
        public int? IdPaymentMethodType { get; set; }

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

        public OrderStatus CombinedEditOrderStatus { get; set; }

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
        public decimal? SurchargeOverride { get; set; }

        [Map]
        public decimal? ShippingOverride { get; set; }

        [Map]
        public decimal ShippingTotal { get; set; }

        [Map]
        public PreferredShipMethod? PreferredShipMethod { get; set; }

        [Map]
        public decimal ProductsSubtotal { get; set; }

        public string DiscountCode { get; set; }

        public IList<GCListItemModel> GCs { get; set; }

        public decimal DiscountedSubtotal { get; set; }

        public string DiscountMessage { get; set; }

        [Map]
        public decimal DiscountTotal { get; set; }

        [Map]
        public decimal TaxTotal { get; set; }

        [Map]
        public decimal Total { get; set; }

        [Map]
        public bool IgnoneMinimumPerishableThreshold { get; set; }

        [Map]
        public IList<SkuOrderedManageModel> SkuOrdereds { get; set; }

        [Map]
        public IList<PromoSkuOrderedManageModel> PromoSkus { get; set; }

        public bool SignUpNewsletter { get; set; }

        public bool ShouldSplit { get; set; }

        public OrderManageModel()
        {
            GCs = new List<GCListItemModel>();
            SkuOrdereds = new List<SkuOrderedManageModel>();
            PromoSkus = new List<PromoSkuOrderedManageModel>();
        }

    }
}