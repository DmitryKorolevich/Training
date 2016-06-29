using System;
using System.Collections.Generic;
using System.Linq;
using VitalChoice.Ecommerce.Domain.Entities.Orders;
using VitalChoice.Infrastructure.Domain.Entities;

namespace VitalChoice.Infrastructure.Domain.Transfer.Orders
{
    public class OrdersAgentReportExportItem
    { 
        public string Agent { get; set; }

        public string AgentName { get; set; }

        public string OrdersCount { get; set; }

        public string TotalOrdersAmount { get; set; }

        public string AverageOrdersAmount { get; set; }

        public string LowestOrderAmount { get; set; }

        public string HighestOrderAmount { get; set; }

        public string ReshipsCount { get; set; }

        public string RefundsCount { get; set; }
    }
}