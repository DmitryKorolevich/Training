using System;
using System.Collections.Generic;
using VitalChoice.Validation.Models;
using VitalChoice.Ecommerce.Domain.Entities.Orders;
using VitalChoice.Infrastructure.Domain.ServiceBus.DataContracts;
using VitalChoice.Infrastructure.Domain.Transfer.Orders;

namespace VC.Admin.Models.Orders
{
    public class OrderExportRequestModel
    {
        public OrderExportRequestModel()
        {
            ExportedOrders=new List<OrderExportItemResult>();
        }

        public DateTime DateCreated { get; set; }

        public string AgentId { get; set; }

        public int TotalCount { get; set; }

        public ICollection<OrderExportItemResult> ExportedOrders { get; set; }
    }
}