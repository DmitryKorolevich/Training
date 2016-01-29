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
    public class OrderDropShipImportItem : OrderBaseImportItem
    {
        [Map]
        [Required]
        [Display(Name = "po")]
        [MaxLength(BaseAppConstants.DEFAULT_TEXT_FIELD_MAX_SIZE)]
        public string PoNumber { get; set; }

        [Map]
        [Display(Name = "Order Notes")]
        [MaxLength(BaseAppConstants.DEFAULT_TEXT_FIELD_MAX_SIZE)]
        public string OrderNotes { get; set; }

        [Map]
        [Display(Name = "Ship Date")]
        public DateTime? ShipDelayDate { get; set; }

        public override void SetFields(OrderDynamic order)
        {
            base.SetFields(order);
            order.Data.KeyCode = "DROPSHIP";
            order.OrderStatus = OrderStatus.Processed;
            order.Data.OrderType = (int)SourceOrderType.Web;
            order.Data.PoNumber = PoNumber;
            order.Data.OrderNotes = OrderNotes;
            order.Data.ShipDelayType = ShipDelayType.EntireOrder;
            order.Data.ShipDelayDate = ShipDelayDate;
        }
    }
}
