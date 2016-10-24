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
    public class OrderInfoItem
    {
        public int Id { get; set; }

        public OrderType IdObjectType { get; set; }

        public OrderStatus? OrderStatus { get; set; }

        public OrderStatus? POrderStatus { get; set; }

        public OrderStatus? NPOrderStatus { get; set; }

        [Map("OrderType")]
        public int? IdOrderSource { get; set; }

        [Map]
        public string OrderNotes { get; set; }

        public int? IdPaymentMethod { get; set; }

        public DateTime DateCreated { get; set; }

        [Map("ShipDelayDate")]
        public DateTime? DateShipped { get; set; }

        [Map("ShipDelayDateP")]
        public DateTime? PDateShipped { get; set; }

        [Map("ShipDelayDateNP")]
        public DateTime? NPDateShipped { get; set; }

        public decimal Total { get; set; }

        public int? IdEditedBy { get; set; }

        public string EditedByAgentId { get; set; }

        public DateTime DateEdited { get; set; }

        public int? IdAddedBy { get; set; }

        public string AddedByAgentId { get; set; }

        [Map]
        public int? POrderType { get; set; }

        public int IdCustomerType { get; set; }

        public int IdCustomer { get; set; }

        public string Company { get; set; }

        public string Customer { get; set; }

        public string StateCode { get; set; }

        public string ShipTo { get; set; }
        
        public int? PreferredShipMethod { get; set; }

        [Map]
        public bool Healthwise { get; set; }
    }
}
