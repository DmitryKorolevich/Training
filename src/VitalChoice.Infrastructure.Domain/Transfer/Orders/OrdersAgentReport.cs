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

        public ICollection<OrdersAgentReportPeriodItem> Periods { get; set; }
    }
}