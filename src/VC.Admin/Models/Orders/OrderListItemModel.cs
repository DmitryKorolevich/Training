using System;
using VitalChoice.Validation.Models;
using VitalChoice.Ecommerce.Domain.Entities.Orders;
using VitalChoice.Infrastructure.Domain.Transfer.Orders;

namespace VC.Admin.Models.Orders
{
    public class OrderListItemModel : BaseModel
    {
        public int Id { get; set; }

        public OrderStatus OrderStatus { get; set; }

        public int IdOrderSource { get; set; }

        public string OrderNotes { get; set; }

        public int? IdPaymentMethod { get; set; }

        public DateTime DateCreated { get; set; }

        public DateTime? DateShipped { get; set; }

        public decimal Total { get; set; }

        public int? IdEditedBy { get; set; }

        public string EditedByAgentId { get; set; }

        public DateTime DateEdited { get; set; }

        public int? POrderType { get; set; }

        public int? IdShippingMethod { get; set; }

        public int IdCustomerType { get; set; }

        public int IdCustomer { get; set; }

        public string Company { get; set; }

        public string Customer { get; set; }

        public string StateCode { get; set; }

        public string ShipTo { get; set; }

        public OrderListItemModel(VOrder item)
        {
            if(item!=null)
            {
                Id = item.Id;
                OrderStatus = item.OrderStatus;
                IdOrderSource = item.IdOrderSource ?? 1;
                OrderNotes = item.OrderNotes;
                IdPaymentMethod = item.IdPaymentMethod;
                DateCreated = item.DateCreated;
                DateShipped = item.DateShipped;
                Total = item.Total;
                IdEditedBy = item.IdEditedBy;
                EditedByAgentId = item.EditedByAgentId;
                DateEdited = item.DateEdited;
                POrderType = item.POrderType;
                IdShippingMethod = item.IdShippingMethod;
                IdCustomerType = item.IdCustomerType;
                IdCustomer = item.IdCustomer;
                Company = item.Company;
                Customer = item.Customer;
                StateCode = item.StateCode;
                ShipTo = item.ShipTo;
            }
        }
    }
}