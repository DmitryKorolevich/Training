using System;
using VitalChoice.Ecommerce.Domain.Entities.Orders;
using VitalChoice.Infrastructure.Domain.Entities;

namespace VitalChoice.Infrastructure.Domain.Transfer.Orders
{
    public class OrdersAgentReportAgentItem
    {
        public int IdAdmin { get; set; }

        public string AgentId { get; set; }

        public int OrdersCount { get; set; }

        public decimal TotalOrdersAmount { get; set; }

        public decimal AverageOrdersAmount { get; set; }

        public decimal LowestOrderAmount { get; set; }

        public decimal HighestOrderAmount { get; set; }

        public int ReshipsCount { get; set; }

        public int RefundsCount { get; set; }
    }
}