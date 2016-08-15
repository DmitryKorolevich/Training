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

namespace VitalChoice.Infrastructure.Domain.Entities.Orders
{
    public class OrderGiftListImportItem : OrderBaseImportItem
    {
        [Map]
        [Required]
        [Display(Name = "First Name")]
        [MaxLength(BaseAppConstants.DEFAULT_TEXT_FIELD_MAX_SIZE)]
        public string FirstName { get; set; }

        [Map]
        [Required]
        [Display(Name = "Last Name")]
        [MaxLength(BaseAppConstants.DEFAULT_TEXT_FIELD_MAX_SIZE)]
        public string LastName { get; set; }

        [Map]
        [Display(Name = "Company")]
        [MaxLength(BaseAppConstants.DEFAULT_TEXT_FIELD_MAX_SIZE)]
        public string Company { get; set; }

        [Map]
        [Required]
        [Display(Name = "Address 1")]
        [MaxLength(BaseAppConstants.DEFAULT_TEXT_FIELD_MAX_SIZE)]
        public string Address1 { get; set; }

        [Map]
        [Display(Name = "Address 2")]
        [MaxLength(BaseAppConstants.DEFAULT_TEXT_FIELD_MAX_SIZE)]
        public string Address2 { get; set; }

        [Map]
        [Required]
        [Display(Name = "City")]
        [MaxLength(BaseAppConstants.DEFAULT_TEXT_FIELD_MAX_SIZE)]
        public string City { get; set; }

        [Required]
        [Display(Name = "State")]
        [MaxLength(BaseAppConstants.DEFAULT_TEXT_FIELD_MAX_SIZE)]
        public string State { get; set; }

        [Map]
        public int? IdState { get; set; }

        [Required]
        [Display(Name = "Country")]
        [MaxLength(BaseAppConstants.DEFAULT_TEXT_FIELD_MAX_SIZE)]
        public string Country { get; set; }

        [Map]
        public int? IdCountry { get; set; }

        [Map]
        [Required]
        [Display(Name = "Postal Code")]
        [MaxLength(BaseAppConstants.DEFAULT_TEXT_FIELD_MAX_SIZE)]
        public string Zip { get; set; }

        [Map]
        [Required]
        [Display(Name = "Phone")]
        [MaxLength(BaseAppConstants.DEFAULT_TEXT_FIELD_MAX_SIZE)]
        public string Phone { get; set; }

        [Map]
        [Required]
        [EmailAddress]
        [Display(Name = "Email")]
        [MaxLength(BaseAppConstants.DEFAULT_TEXT_FIELD_MAX_SIZE)]
        public string Email { get; set; }

        public ICollection<OrderSkuImportItem> Skus { get; set; }

        [Map]
        [Display(Name = "Order Notes")]
        [MaxLength(BaseAppConstants.DEFAULT_TEXT_FIELD_MAX_SIZE)]
        public string OrderNotes { get; set; }

        [Map]
        [Display(Name = "Greeting")]
        [MaxLength(BaseAppConstants.DEFAULT_TEXT_FIELD_MAX_SIZE)]
        public string GiftMessage { get; set; }

        [Map]
        [Display(Name = "Ship Date")]
        public DateTime? ShipDelayDate { get; set; }

        public override void SetFields(OrderDynamic order, CustomerPaymentMethodDynamic paymentMethod)
        {
            base.SetFields(order, paymentMethod);

            order.PaymentMethod = new OrderPaymentMethodDynamic()
            {
                IdObjectType = (int)PaymentMethodType.NoCharge,
                Address = paymentMethod.Address
            };
            order.PaymentMethod.Data.PaymentComment = "Gift List Upload";
            order.PaymentMethod.Address.Id = 0;
            order.Skus = this?.Skus.Select(p => new SkuOrdered()
            {
                Sku = new SkuDynamic() { Code = p.SKU },
                Quantity = p.QTY,
            }).ToList();

            order.Data.KeyCode = "GIFT LIST";
            order.OrderStatus = OrderStatus.OnHold;
            order.Data.GiftOrder = true;
            order.Data.OrderType = (int)SourceOrderType.Phone;
            order.Data.OrderNotes = OrderNotes;
            order.Data.GiftMessage = GiftMessage;
            order.Data.ShipDelayType = ShipDelayDate.HasValue ? ShipDelayType.EntireOrder : ShipDelayType.None;
            order.Data.ShipDelayDate = ShipDelayDate;
        }
    }
}
