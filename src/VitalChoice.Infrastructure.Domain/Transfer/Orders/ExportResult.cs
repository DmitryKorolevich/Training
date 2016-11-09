using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VitalChoice.Infrastructure.Domain.ServiceBus.DataContracts;

namespace VitalChoice.Infrastructure.Domain.Transfer.Orders
{
    public class ExportResult
    {
        public ExportResult(string agentId, int totalCount)
        {
            AgentId = agentId;
            TotalCount = totalCount;
        }

        public DateTime DateStarted { get; set; }
        public string AgentId { get; }
        public int TotalCount { get; }
        public List<OrderExportItemResult> ExportedOrders { get; } = new List<OrderExportItemResult>();
    }
}