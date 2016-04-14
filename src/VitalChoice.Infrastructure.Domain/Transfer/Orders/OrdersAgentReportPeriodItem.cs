using System;
using System.Collections.Generic;
using System.Linq;
using VitalChoice.Ecommerce.Domain.Entities.Orders;
using VitalChoice.Infrastructure.Domain.Entities;

namespace VitalChoice.Infrastructure.Domain.Transfer.Orders
{
    public class OrdersAgentReportPeriodItem
    {
        public OrdersAgentReportPeriodItem()
        {
            Teams = new List<OrdersAgentReportTeamItem>();
        }

        public DateTime From { get; set; }

        public DateTime To { get; set; }

        public ICollection<OrdersAgentReportTeamItem> Teams { get; set; }

        public int OrdersCount { get; set; }

        public decimal TotalOrdersAmount { get; set; }

        public decimal AverageOrdersAmount { get; set; }

        public decimal LowestOrderAmount { get; set; }

        public decimal HighestOrderAmount { get; set; }

        public int ReshipsCount { get; set; }

        public int RefundsCount { get; set; }

        public decimal AgentOrdersPercent { get; set; }

        public decimal AverageOrdersAmountDifference { get; set; }

        //For all orders with created without agents
        public int AllOrdersCount { get; set; }

        public decimal AllTotalOrdersAmount { get; set; }

        public decimal AllAverageOrdersAmount { get; set; }

        public decimal AllLowestOrderAmount { get; set; }

        public decimal AllHighestOrderAmount { get; set; }

        public int AllReshipsCount { get; set; }

        public int AllRefundsCount { get; set; }
    }
}