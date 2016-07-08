using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using VitalChoice.Ecommerce.Domain.Attributes;
using VitalChoice.Ecommerce.Domain.Entities;
using VitalChoice.Ecommerce.Domain.Entities.Orders;
using VitalChoice.Ecommerce.Domain.Exceptions;
using VitalChoice.Infrastructure.Domain.Constants;
using VitalChoice.Infrastructure.Domain.Dynamic;

namespace VitalChoice.Infrastructure.Domain.Entities.Orders
{
    public class OrderGiftListImportItem : OrderBaseImportItem
    {
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
