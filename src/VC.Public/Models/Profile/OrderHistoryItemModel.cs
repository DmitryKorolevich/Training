using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VitalChoice.Domain.Entities.eCommerce.Orders;

namespace VC.Public.Models.Profile
{
    public class OrderHistoryItemModel
    {
		public int Id { get; set; }

		public decimal Total { get; set; }

		public DateTime DateCreated { get; set; }

		public OrderStatus OrderStatus { get; set; }
	}
}
