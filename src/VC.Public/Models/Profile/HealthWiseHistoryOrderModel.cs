using System;
using VitalChoice.Ecommerce.Domain.Entities.Orders;

namespace VC.Public.Models.Profile
{
    public class HealthWiseHistoryOrderModel
    {
		public int Id { get; set; }

        public decimal DiscountedSubtotal { get; set; }

        public DateTime DateCreated { get; set; }

        public OrderStatus? OrderStatus { get; set; }

        public OrderStatus? POrderStatus { get; set; }

        public OrderStatus? NPOrderStatus { get; set; }
    }
}
