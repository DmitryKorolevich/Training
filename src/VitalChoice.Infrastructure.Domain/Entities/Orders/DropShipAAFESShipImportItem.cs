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
    public class DropShipAAFESShipImportItem : OrderBaseImportItem
    {
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
            //order.Skus = this?.Skus.Select(p => new SkuOrdered()
            //{
            //    Sku = new SkuDynamic() { Code = p.SKU },
            //    Quantity = p.QTY,
            //}).ToList();

            order.Data.KeyCode = "AAFES ORDER'";
            order.OrderStatus = OrderStatus.Processed;
            order.Data.OrderType = (int)SourceOrderType.Web;
            //order.Data.PoNumber = PoNumber;
            //order.Data.OrderNotes = OrderNotes;
            //order.Data.ShipDelayType = ShipDelayDate.HasValue ? ShipDelayType.EntireOrder : ShipDelayType.None;
            //order.Data.ShipDelayDate = ShipDelayDate;
        }
    }
}
