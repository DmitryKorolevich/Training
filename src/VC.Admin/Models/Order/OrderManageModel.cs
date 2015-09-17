using System;
using System.Collections.Generic;
using System.Linq;
using VC.Admin.Validators.Product;
using VitalChoice.Domain.Entities;
using VitalChoice.Domain.Entities.Content;
using VitalChoice.Validation.Models;
using VitalChoice.Validation.Attributes;
using VitalChoice.Validation.Models.Interfaces;
using VitalChoice.DynamicData.Attributes;
using VitalChoice.DynamicData;
using VitalChoice.Domain.Entities.eCommerce.Products;
using VitalChoice.Domain.Entities.Localization.Groups;
using VC.Admin.Validators.Order;
using VitalChoice.Domain.Entities.eCommerce.Orders;
using VC.Admin.Models.Product;
using VitalChoice.Domain.Exceptions;
using VitalChoice.DynamicData.Entities.Transfer;
using VitalChoice.DynamicData.Entities;
using VC.Admin.Models.Customer;

namespace VC.Admin.Models.Order
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

        public IList<string> Messages { get; set; }

        public SkuOrderedManageModel(SkuOrdered dynamic)
        {
            if (dynamic != null)
            {
                Code = dynamic.Sku.Code;
                ProductName = dynamic.ProductWithoutSkus.Name;
                IdProductType = dynamic.ProductWithoutSkus.IdObjectType;
                QTY = dynamic.Quantity;
                Price = dynamic.Amount;
                Amount = Price * QTY;
                Id = dynamic.Sku.Id;
                Messages = dynamic.Messages;
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
        
        [DirectLocalized("Payment Method")]
        public int? IdPaymentMethodType { get; set; }

        [Map]
        public int? IdObjectType { get; set; }

        [Map]
        public RecordStatusCode StatusCode { get; set; }

        [Map]
        public OrderStatus OrderStatus { get; set; }

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
        public int? ShipDelayType { get; set; }

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
        public int? ShippingUpgradeP { get; set; }

        [Map]
        public int? ShippingUpgradeNP { get; set; }

        [Map]
        public decimal SurchargeOverride { get; set; }

        [Map]
        public decimal ShippingOverride { get; set; }

        [Map]
        public decimal ShippingTotal { get; set; }

        [Map]
        public int? PreferredShipMethod { get; set; }

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
        public IList<SkuOrderedManageModel> SkuOrdereds { get; set; }

        public bool SignUpNewsletter { get; set; }

        public OrderManageModel()
        {
            GCs = new List<GCListItemModel>();
            SkuOrdereds = new List<SkuOrderedManageModel>();
        }

    }
}