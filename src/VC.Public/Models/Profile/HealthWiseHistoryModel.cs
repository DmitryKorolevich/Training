using System;
using System.Collections.Generic;
using VitalChoice.Ecommerce.Domain.Entities.Orders;

namespace VC.Public.Models.Profile
{
    public class HealthWiseHistoryModel
    {
		public int Count { get; set; }

		public decimal AverageAmount { get; set; }

        public DateTime EndDate { get; set; }

        public ICollection<HealthWiseHistoryOrderModel> Items { get; set; }
    }
}
