using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using VitalChoice.Ecommerce.Domain.Attributes;
using VitalChoice.Ecommerce.Domain.Entities;
using VitalChoice.Ecommerce.Domain.Entities.Orders;
using VitalChoice.Ecommerce.Domain.Entities.Payment;
using VitalChoice.Ecommerce.Domain.Exceptions;
using VitalChoice.Infrastructure.Domain.Constants;
using VitalChoice.Infrastructure.Domain.Dynamic;
using VitalChoice.Infrastructure.Domain.Transfer.Orders;
using VitalChoice.Infrastructure.Domain.Transfer.Shipping;

namespace VitalChoice.Infrastructure.Domain.Entities.Orders
{
    public class DropShipAAFESShipImportItem : OrderBaseImportItem
    {
        [Map]
        [Display(Name = "First Name")]
        [MaxLength(BaseAppConstants.DEFAULT_TEXT_FIELD_MAX_SIZE)]
        public string FirstName { get; set; }

        [Map]
        [Display(Name = "Last Name")]
        [MaxLength(BaseAppConstants.DEFAULT_TEXT_FIELD_MAX_SIZE)]
        public string LastName { get; set; }

        [Map]
        [Required]
        [Display(Name = "SHIPTO_COMPANY")]
        [MaxLength(BaseAppConstants.DEFAULT_TEXT_FIELD_MAX_SIZE)]
        public string Company { get; set; }

        [Map]
        [Required]
        [Display(Name = "SHIPTO_REF1")]
        [MaxLength(BaseAppConstants.DEFAULT_TEXT_FIELD_MAX_SIZE)]
        public string Address1 { get; set; }

        [Map]
        [Display(Name = "SHIPTO_REF2")]
        [MaxLength(BaseAppConstants.DEFAULT_TEXT_FIELD_MAX_SIZE)]
        public string Address2 { get; set; }

        [Map]
        [Required]
        [Display(Name = "SHIPTO_CITY")]
        [MaxLength(BaseAppConstants.DEFAULT_TEXT_FIELD_MAX_SIZE)]
        public string City { get; set; }

        [Required]
        [Display(Name = "SHIPTO_STATE")]
        [MaxLength(BaseAppConstants.DEFAULT_TEXT_FIELD_MAX_SIZE)]
        public string State { get; set; }

        [Map]
        public int? IdState { get; set; }

        [Required]
        [Display(Name = "SHIPTO_COUNTRY")]
        [MaxLength(BaseAppConstants.DEFAULT_TEXT_FIELD_MAX_SIZE)]
        public string Country { get; set; }

        [Map]
        public int? IdCountry { get; set; }

        [Map]
        [Required]
        [Display(Name = "SHIPTO_ZIP")]
        [MaxLength(BaseAppConstants.DEFAULT_TEXT_FIELD_MAX_SIZE)]
        public string Zip { get; set; }

        [Map]
        [Required]
        [Display(Name = "SHIPTO_PHONE")]
        [MaxLength(BaseAppConstants.DEFAULT_TEXT_FIELD_MAX_SIZE)]
        public string Phone { get; set; }
        
        [Map]
        [Required]
        [Display(Name = "PO_NO")]
        [MaxLength(BaseAppConstants.DEFAULT_TEXT_FIELD_MAX_SIZE)]
        public string PoNumber { get; set; }

        [Map]
        [Required]
        [Display(Name = "ORDER_NO")]
        [MaxLength(BaseAppConstants.DEFAULT_TEXT_FIELD_MAX_SIZE)]
        public string OrderNotes { get; set; }

        [Map]
        [Display(Name = "GREETING")]
        [MaxLength(BaseAppConstants.DEFAULT_TEXT_FIELD_MAX_SIZE)]
        public string GiftMessage { get; set; }

        [Map]
        [Required]
        [Display(Name = "SHIP_DATE")]
        public DateTime? ShipDelayDate { get; set; }

        [Map]
        [Required]
        [Display(Name = "VND_SKU")]
        [MaxLength(BaseAppConstants.DEFAULT_TEXT_FIELD_MAX_SIZE)]
        public string Sku { get; set; }

        [Map]
        [Required]
        [Display(Name = "QTY")]
        [MaxLength(BaseAppConstants.DEFAULT_TEXT_FIELD_MAX_SIZE)]
        public int QTY { get; set; }


        public override void SetFields(OrderDynamic order, CustomerPaymentMethodDynamic paymentMethod)
        {
            base.SetFields(order, paymentMethod);

            order.PaymentMethod = new OrderPaymentMethodDynamic()
            {
                IdObjectType = (int)PaymentMethodType.Oac,
                Address = paymentMethod.Address
            };
            order.PaymentMethod.Data.Terms = paymentMethod.Data.Terms;
            order.PaymentMethod.Data.Fob = paymentMethod.Data.Fob;
            order.PaymentMethod.Address.Id = 0;

            order.Data.KeyCode = "AAFES ORDER";
            order.OrderStatus = OrderStatus.Processed;
            order.Data.OrderType = (int)SourceOrderType.Web;
            order.Data.PoNumber = PoNumber;
            order.Data.OrderNotes = OrderNotes;
            order.Data.ShipDelayType = ShipDelayDate.HasValue ? ShipDelayType.EntireOrder : ShipDelayType.None;
            order.Data.ShipDelayDate = ShipDelayDate;
            if (!string.IsNullOrEmpty(GiftMessage))
            {
                order.Data.GiftMessage = GiftMessage;
                order.Data.GiftOrder = true;
            }
            order.Data.ShippingUpgradeP = ShippingUpgradeOption.Overnight;
            order.Data.ShippingUpgradeNP = ShippingUpgradeOption.Overnight;
        }
    }
}
