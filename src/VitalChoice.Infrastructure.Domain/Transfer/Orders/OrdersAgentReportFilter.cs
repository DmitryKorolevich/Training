using System;
using System.Collections.Generic;
using VitalChoice.Ecommerce.Domain.Entities.Orders;
using VitalChoice.Infrastructure.Domain.Entities;

namespace VitalChoice.Infrastructure.Domain.Transfer.Orders
{
    public class OrdersAgentReportFilter
	{
        public DateTime From { get; set; }

        public DateTime To { get; set; }

        public FrequencyType FrequencyType { get; set; }

        public ICollection<int> IdAdminTeams { get; set; }

        public int? IdAdmin { get; set; }
    }
}