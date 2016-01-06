using System;
using VitalChoice.Validation.Models;
using VitalChoice.Ecommerce.Domain.Entities.Orders;
using VitalChoice.Infrastructure.Domain.Transfer.Orders;

namespace VC.Admin.Models.Orders
{
    public class GCOrderListItemModel : BaseModel
    {
        public int Id { get; set; }

        public OrderStatus OrderStatus { get; set; }

        public int IdOrderSource { get; set; }

        public DateTime DateCreated { get; set; }

        public decimal Total { get; set; }

        public decimal GCAmountUsed { get; set; }

        public int? IdEditedBy { get; set; }

        public string EditedBy { get; set; }

        public DateTime DateEdited { get; set; }

        public int IdCustomer { get; set; }

        public string CustomerFirstName { get; set; }

        public string CustomerLastName { get; set; }

        public string CustomerShipFirstName { get; set; }

        public string CustomerShipLastName { get; set; }

        public GCOrderListItemModel(GCOrderItem item)
        {
            if (item != null)
            {
                Id = item.Order.Id;
                OrderStatus = item.Order.OrderStatus;
                IdOrderSource = item.Order.SafeData.OrderType ?? 1;
                DateCreated = item.Order.DateCreated;
                Total = item.Order.Total;
                GCAmountUsed = item.GCAmountUsed;
                IdEditedBy = item.Order.IdEditedBy;
                EditedBy = item.EditedBy;
                DateEdited = item.Order.DateEdited;
                IdCustomer = item.Order.Customer.Id;
                var address = item.Order?.PaymentMethod?.Address;
                if (address == null)
                {
                    address = item.Order?.ShippingAddress;
                }
                if (address != null)
                {
                    CustomerFirstName = address.SafeData.FirstName;
                    CustomerLastName = address.SafeData.LastName;
                }

                address = item.Order?.ShippingAddress;
                if (address != null)
                {
                    CustomerShipFirstName = address.SafeData.FirstName;
                    CustomerShipLastName = address.SafeData.LastName;
                }
                //ShipTo = item.ShipTo;
            }
        }
    }
}