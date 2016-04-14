using System;
using System.Collections.Generic;
using System.Linq;
using VitalChoice.Ecommerce.Domain.Entities.Orders;
using VitalChoice.Infrastructure.Domain.Entities;

namespace VitalChoice.Infrastructure.Domain.Transfer.Orders
{
    public class OrdersAgentReport
    {
        public OrdersAgentReport()
        {
            Periods=new List<OrdersAgentReportPeriodItem>();
        }

        public IList<OrdersAgentReportPeriodItem> Periods { get; set; }

        public FrequencyType FrequencyType { get; set; }

        public int? IdAdminTeam { get; set; }

        public int? IdAdmin { get; set; }
    }
}