using System;
using VitalChoice.Ecommerce.Domain.Entities.Orders;
using VitalChoice.Infrastructure.Domain.Entities;

namespace VitalChoice.Infrastructure.Domain.Transfer.Orders
{
    public class OrderForAgentReport
    {
        public Order Order { get; set; }

        public SourceOrderType? OrderType { get; set; }
    }
}