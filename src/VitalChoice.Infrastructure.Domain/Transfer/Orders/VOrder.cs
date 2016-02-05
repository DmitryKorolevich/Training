using System;
using VitalChoice.Ecommerce.Domain;
using VitalChoice.Ecommerce.Domain.Entities.Orders;

namespace VitalChoice.Infrastructure.Domain.Transfer.Orders
{
    public class VOrder : Entity
    {
        public string IdString { get; set; }

        public OrderStatus? OrderStatus { get; set; }

        public OrderStatus? POrderStatus { get; set; }

        public OrderStatus? NPOrderStatus { get; set; }

        public int? IdOrderSource { get; set; }

        public string SIdOrderSource { get; set; }

        public string OrderNotes { get; set; }

        public int? IdPaymentMethod { get; set; }

        public DateTime DateCreated { get; set; }

        public DateTime? DateShipped { get; set; }

        public DateTime? PDateShipped { get; set; }

        public DateTime? NPDateShipped { get; set; }

        public decimal Total { get; set; }

        public int? IdEditedBy { get; set; }

        public string EditedByAgentId { get; set; }

        public DateTime DateEdited { get; set; }

        public int? POrderType { get; set; }

        public string SPOrderType { get; set; }

        public int? IdShippingMethod { get; set; }

        public int IdCustomerType { get; set; }

        public int IdCustomer { get; set; }

        public string Company { get; set; }

        public string Customer { get; set; }

        public string StateCode { get; set; }

        public string ShipTo { get; set; }

        public bool Healthwise { get; set; }
    }
}