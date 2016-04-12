using System;
using System.Collections.Generic;
using System.Linq;
using VitalChoice.Ecommerce.Domain.Entities.Orders;
using VitalChoice.Infrastructure.Domain.Entities;

namespace VitalChoice.Infrastructure.Domain.Transfer.Orders
{
    public class OrdersAgentReportTeamItem
    {
        public OrdersAgentReportTeamItem()
        {
            Agents=new List<OrdersAgentReportAgentItem>();
        }

        public int? IdAdminTeam { get; set; }

        public string AdminTeamName { get; set; }

        public ICollection<OrdersAgentReportAgentItem> Agents { get; set; }

        public int OrdersCount { get; set; }

        public decimal TotalOrdersAmount { get; set; }

        public decimal AverageOrdersAmount { get; set; }

        public decimal LowestOrderAmount { get; set; }

        public decimal HighestOrderAmount { get; set; }

        public int ReshipsCount { get; set; }

        public int RefundsCount { get; set; }

        public decimal AgentOrdersPercent { get; set; }

        public decimal AverageOrdersAmountDifference { get; set; }
    }
}